using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class Gravity : MonoBehaviour
    {
        [SerializeField]
        private LayerMask _platformMask;

        private Vector3 _objectSize;
        private bool _onGround;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _objectSize = GetComponent<Renderer>().bounds.size;
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (!World.Instance.GamePaused)
            {
                CheckIfOnGround();

                if (!_onGround)
                {
                    Fall();
                }
            }
        }

        private void Fall()
        {
            
        }

        private void CheckIfOnGround()
        {

        }
    }
}
