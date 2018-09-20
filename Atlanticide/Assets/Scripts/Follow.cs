using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// Makes the object follow another game object.
    /// </summary>
    public class Follow : MonoBehaviour
    {
        [SerializeField]
        private GameObject _objectToFollow;

        [SerializeField]
        private Vector3 _offset;

        [SerializeField]
        private bool _lockX;

        [SerializeField]
        private bool _lockY;

        [SerializeField]
        private bool _lockZ;

        private Vector3 _startPosition;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _startPosition = transform.position;
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (_objectToFollow != null)
            {
                Vector3 newPosition = _objectToFollow.transform.position;

                if (_lockX)
                {
                    newPosition.x = _startPosition.x;
                }
                if (_lockY)
                {
                    newPosition.y = _startPosition.y;
                }
                if (_lockZ)
                {
                    newPosition.z = _startPosition.z;
                }

                newPosition += _offset;
                transform.position = newPosition;
            }
        }
    }
}
