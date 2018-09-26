using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// Makes the object move.
    /// </summary>
    public class Move : Motion
    {
        [SerializeField]
        private Vector3 _topSpeed;

        [SerializeField]
        private float _decayDistance = 0f;

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
        /// Moves the object.
        /// </summary>
        protected override void MoveObject()
        {
            Vector3 movement = CurrentSpeed * World.Instance.DeltaTime;
            _movedDistance += movement.magnitude;
            transform.position += movement;

            if (DecayDistance > 0f && _movedDistance >= DecayDistance)
            {
                _movedDistance = 0f;
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Updates the current speed.
        /// Accelerates or decelerates until the top speed is reached.
        /// </summary>
        protected override void UpdateCurrentSpeed()
        {
            if (ReachedTargetSpeed)
            {
                return;
            }

            if (_active)
            {
                SpeedRatio += _acceleration * World.Instance.DeltaTime;

                if (SpeedRatio >= 1f)
                {
                    SpeedRatio = 1f;
                    CurrentSpeed = TopSpeed;
                    ReachedTargetSpeed = true;
                }
            }
            else
            {
                SpeedRatio += _deceleration * World.Instance.DeltaTime;

                if (SpeedRatio <= 0f)
                {
                    SpeedRatio = 0f;
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
        public override void StartMoving(bool instant)
        {
            CurrentSpeed = (instant ? TopSpeed : Vector3.zero);
            base.StartMoving(instant);
        }

        /// <summary>
        /// Decelerates the object until it stops moving.
        /// If <paramref name="instant"/> is true, the object instantly stops.
        /// </summary>
        /// <param name="instant">Should the object instantly stop</param>
        public override void StopMoving(bool instant)
        {
            if (instant)
            {
                CurrentSpeed = Vector3.zero;
            }

            base.StopMoving(instant);
        }
    }
}
