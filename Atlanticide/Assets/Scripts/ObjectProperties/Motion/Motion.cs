using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// Base class for making an object move.
    /// </summary>
    public abstract class Motion : MonoBehaviour
    {
        [SerializeField, Range(0.01f, 10f)]
        protected float _acceleration;

        [SerializeField, Range(-10f, -0.01f)]
        protected float _deceleration;

        [SerializeField]
        protected bool _active;

        [SerializeField]
        protected bool _hasInstantTopSpeedByDefault;

        protected float _movedDistance;

        /// <summary>
        /// Is the object accelerating or in full speed.
        /// </summary>
        public bool Active
        {
            get { return _active; }
        }

        /// <summary>
        /// Is the object moving.
        /// </summary>
        public bool Moving { get; protected set; }

        /// <summary>
        /// The speed ratio.
        /// </summary>
        public float SpeedRatio { get; protected set; }

        /// <summary>
        /// Has the object reached the target speed.
        /// </summary>
        public bool ReachedTargetSpeed { get; protected set; }

        /// <summary>
        /// The total distance the object has moved.
        /// </summary>
        public float TotalMovedDistance { get; protected set; }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected virtual void Start()
        {
            if (_active)
            {
                StartMoving(_hasInstantTopSpeedByDefault);
            }
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        protected virtual void Update()
        {
            if (Moving)
            {
                UpdateCurrentSpeed();
                MoveObject();
            }
        }

        /// <summary>
        /// Moves the object.
        /// </summary>
        protected abstract void MoveObject();

        /// <summary>
        /// Updates the current speed.
        /// Accelerates or decelerates until the top speed is reached.
        /// </summary>
        protected abstract void UpdateCurrentSpeed();

        /// <summary>
        /// Starts moving the object. If <paramref name="instant"/>
        /// is true, the object instantly has the top speed.
        /// </summary>
        /// <param name="instant">Should the object instantly
        /// have the top speed</param>
        public virtual void StartMoving(bool instant)
        {
            SpeedRatio = (instant ? 1f : 0f);
            ReachedTargetSpeed = instant;
            _movedDistance = 0f;
            _active = true;
            Moving = true;
        }

        /// <summary>
        /// Decelerates the object until it stops moving.
        /// If <paramref name="instant"/> is true, the object instantly stops.
        /// </summary>
        /// <param name="instant">Should the object instantly stop</param>
        public virtual void StopMoving(bool instant)
        {
            if (instant)
            {
                SpeedRatio = 0f;
                ReachedTargetSpeed = true;
                Moving = false;
            }

            ReachedTargetSpeed = instant;
            _active = false;
        }

        public virtual void ResetMotion()
        {
            StopMoving(true);
            TotalMovedDistance = 0f;
        }

        /// <summary>
        /// Sets default values when the object is reset in the editor.
        /// </summary>
        protected virtual void Reset()
        {
            _active = true;
            _acceleration = 1f;
            _deceleration = -1f;
        }
    }
}
