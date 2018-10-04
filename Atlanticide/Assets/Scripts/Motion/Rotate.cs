using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// Makes the object move.
    /// </summary>
    public class Rotate : Motion
    {
        [SerializeField]
        private float _minRotationTime = 1f;

        [SerializeField]
        private float _maxRotationTime = 10f;

        [SerializeField]
        private Utils.Axis _axis;

        [SerializeField]
        private bool _clockwise = true;

        private float _startAngle;

        /// <summary>
        /// The current angle in degrees.
        /// </summary>
        public float CurrentAngle { get; private set; }

        public float CurrentRotationTime { get; private set; }

        protected override void Start()
        {
            base.Start();

            if (_minRotationTime > _maxRotationTime)
            {
                float temp = _minRotationTime;
                _minRotationTime = _maxRotationTime;
                _maxRotationTime = temp;
            }

            _startAngle = Utils.GetAngleOnAxis(_axis, transform.rotation.eulerAngles);
            CurrentAngle = _startAngle;
        }

        protected override void MoveObject()
        {
            if (CurrentRotationTime > 0f)
            {
                CurrentAngle += (_clockwise ? 1 : -1) * 360f * (World.Instance.DeltaTime / CurrentRotationTime);
                transform.rotation = Quaternion.Euler(Utils.GetRotationOnAxis(_axis, CurrentAngle));

                // Keeps track of full rotations
                _movedDistance = Mathf.Abs(CurrentAngle - _startAngle) / 360f;
            }
        }

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
                    CurrentRotationTime = _minRotationTime;
                    ReachedTargetSpeed = true;
                }
            }
            else
            {
                SpeedRatio += _deceleration * World.Instance.DeltaTime;

                if (SpeedRatio <= 0f)
                {
                    SpeedRatio = 0f;
                    CurrentRotationTime = _maxRotationTime;
                    ReachedTargetSpeed = true;
                    Moving = false;
                }
            }

            if (!ReachedTargetSpeed)
            {
                CurrentRotationTime = GetRotationTime(SpeedRatio);
            }
        }

        /// <summary>
        /// Starts rotating the object. If <paramref name="instant"/>
        /// is true, the object instantly has the minimum rotation time.
        /// </summary>
        /// <param name="instant">Should the object instantly
        /// have the minimum rotation time</param>
        public override void StartMoving(bool instant)
        {
            CurrentRotationTime = (instant ? GetRotationTime(1) : GetRotationTime(0));
            base.StartMoving(instant);
        }

        /// <summary>
        /// Decelerates the object until it has the maximum rotation
        /// time and then stops. If <paramref name="instant"/> is
        /// true, the object instantly stops.
        /// </summary>
        /// <param name="instant">Should the object instantly stop</param>
        public override void StopMoving(bool instant)
        {
            base.StopMoving(instant);
        }

        private float GetRotationTime(float ratio)
        {
            return _minRotationTime +
                (1 - ratio) * (_maxRotationTime - _minRotationTime);
        }

        /// <summary>
        /// Sets default values when the object is reset in the editor.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            _minRotationTime = 1f;
            _maxRotationTime = 10f;
        }
    }
}
