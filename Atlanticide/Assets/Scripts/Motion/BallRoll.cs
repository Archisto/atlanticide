using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// Makes the object roll.
    /// </summary>
    public class BallRoll : MonoBehaviour
    {
        [SerializeField]
        private float _speedModifier = 1f;

        [SerializeField]
        private float _ballRadius = 1f;

        [SerializeField]
        private bool _lockX;

        [SerializeField]
        private bool _lockY;

        [SerializeField]
        private bool _lockZ;

        private float _circumference;
        private Vector3 _oldPosition;
        private Vector3 _rotation;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _circumference = 2 * Mathf.PI * _ballRadius;
            _rotation = transform.rotation.eulerAngles;
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            // TODO: Fix. transform.right should always be towards the right
            // of the moving direction and the object should rotate around it.

            Vector3 movement = transform.position - _oldPosition;
            if (_speedModifier != 0f && _circumference > 0f && movement != Vector3.zero)
            {
                Vector3 rotationChange = Vector3.zero;
                float ratio = movement.magnitude / _circumference;
                Vector3 direction = movement.normalized;
                rotationChange = Vector3.right * ratio * 360f;

                //Debug.Log("ratio: " + ratio);
                //Debug.Log("rotationChange: " + rotationChange);

                if (_lockX)
                {
                    rotationChange.x = 0f;
                }
                if (_lockY)
                {
                    rotationChange.y = 0f;
                }
                if (_lockZ)
                {
                    rotationChange.z = 0f;
                }

                _oldPosition = transform.position;
                _rotation =  rotationChange;
                transform.rotation = Quaternion.Euler(_rotation);
            }
        }
    }
}
