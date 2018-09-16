using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrideUnbroken
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
        private bool _active;

        [SerializeField]
        private bool _hasInstantTopSpeedByDefault;

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
        public float SpeedFactor { get; private set; }

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
            transform.position += CurrentSpeed * Time.deltaTime;
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
                SpeedFactor += _acceleration * Time.deltaTime;

                if (SpeedFactor >= 1)
                {
                    SpeedFactor = 1;
                    CurrentSpeed = _topSpeed;
                    ReachedTargetSpeed = true;
                }
            }
            else
            {
                SpeedFactor += _deceleration * Time.deltaTime;

                if (SpeedFactor <= 0)
                {
                    SpeedFactor = 0;
                    CurrentSpeed = Vector3.zero;
                    ReachedTargetSpeed = true;
                    Moving = false;
                }
            }

            if (!ReachedTargetSpeed)
            {
                CurrentSpeed = SpeedFactor * _topSpeed;
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
                SpeedFactor = 1;
                CurrentSpeed = _topSpeed;
            }
            else
            {
                SpeedFactor = 0;
                CurrentSpeed = Vector3.zero;
            }

            ReachedTargetSpeed = instant;
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
                SpeedFactor = 0;
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
