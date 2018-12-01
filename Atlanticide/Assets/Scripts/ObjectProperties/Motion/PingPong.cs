using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class PingPong : MonoBehaviour
    {
        [SerializeField]
        private float _oneDirTime = 1f;

        [SerializeField, Range(0f, 1f)]
        private float _startRatio;

        [SerializeField]
        private Vector3 _position1 = Vector3.zero;

        [SerializeField]
        private Vector3 _position2 = Vector3.one;

        [SerializeField]
        private bool _changePosRelativeToDefaultPos;

        [SerializeField]
        private bool _sineWave;

        private Vector3 _defaultPosition;
        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private float _elapsedTime;
        private bool _reverse;
        private bool _starting = true;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _defaultPosition = transform.localPosition;
            ResetPosition();
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (_oneDirTime > 0f)
            {
                UpdateRotation();
            }
        }

        private void UpdateRotation()
        {
            float ratio = _elapsedTime / _oneDirTime;
            _elapsedTime += World.Instance.DeltaTime;

            if (ratio >= 1f)
            {
                _elapsedTime = 0f;
                _reverse = !_reverse;
                ChangePosition(_targetPosition);
                _startPosition = _targetPosition;
                UpdateTargetPosition();

                if (_starting)
                {
                    _starting = false;
                }
            }
            else
            {
                ChangePositionLerp(_startPosition, _targetPosition, ApplyModifiersToRatio(ratio));
            }
        }

        private float ApplyModifiersToRatio(float ratio)
        {
            // TODO: Fix

            if (_sineWave)
            {
                return Mathf.Sin(ratio);
            }
            else
            {
                return ratio;
            }
        }

        private void ChangePositionLerp(Vector3 start, Vector3 target, float ratio)
        {
            Vector3 position = Vector3.zero;

            if (_changePosRelativeToDefaultPos)
            {
                if (_starting)
                {
                    target += _defaultPosition;
                }
                else
                {
                    position = _defaultPosition;
                }
            }

            position += Vector3.Lerp
                (start, target, ApplyModifiersToRatio(ratio));
            transform.localPosition = position;
        }

        private void ChangePosition(Vector3 position)
        {
            if (_changePosRelativeToDefaultPos)
            {
                position += _defaultPosition;
            }
            
            transform.localPosition = position;
        }

        private void UpdateTargetPosition()
        {
            _targetPosition = (_reverse ? _position1 : _position2);
        }

        public void ResetPosition()
        {
            transform.localPosition = _defaultPosition;
            _reverse = false;
            _starting = true;
            _startPosition = _defaultPosition;
            UpdateTargetPosition();
            _elapsedTime = _oneDirTime * _startRatio;
        }
    }
}
