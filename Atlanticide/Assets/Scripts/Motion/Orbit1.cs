using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// Makes the object orbit around a target point.
    /// </summary>
    public class Orbit1 : MonoBehaviour
    {
        [SerializeField]
        private Vector3 _targetPoint;

        [SerializeField]
        private Vector3 _axis = Vector3.right;

        [SerializeField]
        private float _orbitTime = 1f;

        [SerializeField]
        private float _radius = 1f;

        [SerializeField]
        private bool _lookAtTarget;

        [SerializeField]
        private bool _active = true;

        private float _angle;
        private float _elapsedTime;
        private bool _secondHalf;

        private void Start()
        {
        }

        /// <summary>
        /// Is the object moving.
        /// </summary>
        public bool Moving
        {
            get { return _active; }
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (Moving && _orbitTime > 0f)
            {
                MoveObject();
            }
        }

        /// <summary>
        /// Moves the object.
        /// </summary>
        private void MoveObject()
        {
            _elapsedTime += World.Instance.DeltaTime;
            float ratio = _elapsedTime / _orbitTime;
            if (ratio >= 1f)
            {
                _elapsedTime -= _orbitTime;
                ratio = (ratio > 1f ? ratio - 1 : ratio);
                _secondHalf = !_secondHalf;
            }

            Vector3 side1 = _targetPoint - _axis * _radius;
            Vector3 side2 = _targetPoint + _axis * _radius;

            Vector3 newPosition = Vector3.Slerp(side1, side2, 0.5f * ratio + (_secondHalf ? 0.5f : 0f));
            transform.position = newPosition;
        }

        /// <summary>
        /// Starts moving the object.
        /// </summary>
        public void StartMoving()
        {
            _active = true;
            _secondHalf = false;
        }

        /// <summary>
        /// Stops the object.
        /// </summary>
        public void StopMoving()
        {
            _active = false;
        }
    }
}
