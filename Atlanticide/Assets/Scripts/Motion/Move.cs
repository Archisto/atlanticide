using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// Makes the object move.
    /// </summary>
    public class Move : MonoBehaviour
    {
        [SerializeField]
        private Vector3 _topSpeed;

        [SerializeField, Range(0.01f, 10f)]
        private float _acceleration;

        [SerializeField, Range(-10f, -0.01f)]
        private float _deceleration;

        [SerializeField]
        private float _decayDistance = 0;

        [SerializeField]
        private bool _active;

        [SerializeField]
        private bool _hasInstantTopSpeedByDefault;

        private float _movedDistance;

        /// <summary>
        /// The top speed vector.
        /// </summary>
        public Vector3 TopSpeed
        {
            get
            {
                return _topSpeed;
            }
            set
            {
                _topSpeed = value;
            }
        }

        public float DecayDistance
        {
            get
            {
                return _decayDistance;
            }
            set
            {
                _decayDistance = value;
            }
        }

        /// <summary>
        /// The current speed.
        /// </summary>
        public Vector3 CurrentSpeed { get; private set; }

        /// <summary>
        /// Is the object moving.
        /// </summary>
        public bool Moving { get; private set; }

        /// <summary>
        /// The speed factor.
        /// </summary>
        public float SpeedRatio { get; private set; }

        /// <summary>
        /// Has the object reached the target speed.
        /// </summary>
        public bool ReachedTargetSpeed { get; private set; }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            if (_active)
            {
                StartMoving(_hasInstantTopSpeedByDefault);
            }
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
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
        private void MoveObject()
        {
            Vector3 movement = CurrentSpeed * Time.deltaTime;
            _movedDistance += movement.magnitude;
            transform.position += movement;

            if (DecayDistance > 0 && _movedDistance >= DecayDistance)
            {
                _movedDistance = 0;
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Updates the current speed.
        /// Accelerates or decelerates until the top speed is reached.
        /// </summary>
        private void UpdateCurrentSpeed()
        {
            if (ReachedTargetSpeed)
            {
                return;
            }

            if (_active)
            {
                SpeedRatio += _acceleration * Time.deltaTime;

                if (SpeedRatio >= 1)
                {
                    SpeedRatio = 1;
                    CurrentSpeed = TopSpeed;
                    ReachedTargetSpeed = true;
                }
            }
            else
            {
                SpeedRatio += _deceleration * Time.deltaTime;

                if (SpeedRatio <= 0)
                {
                    SpeedRatio = 0;
                    CurrentSpeed = Vector3.zero;
                    ReachedTargetSpeed = true;
                    Moving = false;
                }
            }

            if (!ReachedTargetSpeed)
            {
                CurrentSpeed = SpeedRatio * TopSpeed;
            }
        }

        /// <summary>
        /// Starts moving the object. If <paramref name="instant"/>
        /// is true, the object instantly has the top speed.
        /// </summary>
        /// <param name="instant">Should the object instantly
        /// have the top speed</param>
        public void StartMoving(bool instant)
        {
            if (instant)
            {
                SpeedRatio = 1;
                CurrentSpeed = TopSpeed;
            }
            else
            {
                SpeedRatio = 0;
                CurrentSpeed = Vector3.zero;
            }

            ReachedTargetSpeed = instant;
            _movedDistance = 0;
            _active = true;
            Moving = true;
        }

        /// <summary>
        /// Decelerates the object until it stops moving.
        /// If <paramref name="instant"/> is true, the object instantly stops.
        /// </summary>
        /// <param name="instant">Should the object instantly stop</param>
        public void StopMoving(bool instant)
        {
            if (instant)
            {
                SpeedRatio = 0;
                CurrentSpeed = Vector3.zero;
                ReachedTargetSpeed = true;
                Moving = false;
            }

            ReachedTargetSpeed = instant;
            _active = false;
        }

        /// <summary>
        /// Sets default values when the object is reset in the editor.
        /// </summary>
        private void Reset()
        {
            _active = true;
            _acceleration = 1;
            _deceleration = -1;
        }
    }
}
