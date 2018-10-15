using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class RotateAlternating : MonoBehaviour
    {
        [SerializeField]
        private float _halfSwingTime = 1f;

        [SerializeField, Range(0f, 1f)]
        private float _startRatio;

        [SerializeField]
        private Vector3 _maxPosRotation = Vector3.one;

        [SerializeField]
        private Vector3 _maxNegRotation = -1 * Vector3.one;

        [SerializeField]
        private bool _startInPosDirection = true;

        private Quaternion _defaultRotation;
        private Quaternion _startRotation;
        private Quaternion _targetRotation;
        private float _elapsedTime;
        private bool _reverse;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _defaultRotation = transform.localRotation;
            ResetRotation();
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (_halfSwingTime > 0f)
            {
                UpdateRotation();
            }
        }

        private void UpdateRotation()
        {
            float ratio = _elapsedTime / _halfSwingTime;
            _elapsedTime += World.Instance.DeltaTime;

            if (ratio >= 1f)
            {
                _elapsedTime = 0f;
                _reverse = !_reverse;
                transform.localRotation = _targetRotation;
                _startRotation = _targetRotation;
                UpdateTargetRotation();
            }
            else
            {
                transform.localRotation = Quaternion.Lerp(_startRotation, _targetRotation, ratio);
            }
        }

        private void UpdateTargetRotation()
        {
            _targetRotation = (_reverse ?
                Quaternion.Euler(_maxNegRotation) :
                Quaternion.Euler(_maxPosRotation));
        }

        public void ResetRotation()
        {
            transform.localRotation = _defaultRotation;
            _startRotation = _defaultRotation;
            _reverse = !_startInPosDirection;
            _elapsedTime = _halfSwingTime * _startRatio;
            UpdateTargetRotation();
        }
    }
}
