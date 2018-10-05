using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class GroundCollider : MonoBehaviour
    {
        [SerializeField]
        private LayerMask _platformMask;

        private Vector3 _objectSize;
        private bool _onGround;
        private bool _isRising;

        public float DistanceFallen { get; set; }

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
            if (_isRising)
            {
                return;
            }

            float fallSpeed =
                (World.Instance.gravity + 2 * DistanceFallen) * World.Instance.DeltaTime;
            Vector3 newPosition = transform.position;
            newPosition.y -= fallSpeed;
            DistanceFallen += fallSpeed;
            transform.position = newPosition;
        }

        private void Rise(float speed)
        {
            Vector3 newPosition = transform.position;
            newPosition.y += speed * World.Instance.DeltaTime;
            transform.position = newPosition;
        }

        private void CheckIfOnGround()
        {

        }

        //private bool CheckGroundCollision(Vector3 position, bool currPos)
        //{
        //    // TODO: Fix falling when not supposed to.

        //    Vector3 p1 = position + new Vector3(-0.5f * _characterSize.x, 0, 0.5f * _characterSize.z);
        //    Vector3 p2 = position + new Vector3(-0.5f * _characterSize.x, 0, 0.5f * _characterSize.z);
        //    Vector3 p3 = position + new Vector3(0.5f * _characterSize.x, 0, 0.5f * _characterSize.z);
        //    Vector3 p4 = position + new Vector3(0.5f * _characterSize.x, 0, -0.5f * _characterSize.z);
        //    RaycastHit hit;
        //    bool touchingPlatform =
        //        Physics.Raycast(new Ray(p1, Vector3.down), out hit, _groundHitDist, _platformLayerMask) ||
        //        Physics.Raycast(new Ray(p2, Vector3.down), out hit, _groundHitDist, _platformLayerMask) ||
        //        Physics.Raycast(new Ray(p3, Vector3.down), out hit, _groundHitDist, _platformLayerMask) ||
        //        Physics.Raycast(new Ray(p4, Vector3.down), out hit, _groundHitDist, _platformLayerMask);

        //    if (touchingPlatform)
        //    {
        //        // Rise if currently inside the ground
        //        if (currPos)
        //        {
        //            _distFallen = 0;

        //            if (Physics.Raycast(GetTopOfHeadDownRay(), _minRiseDist, _platformLayerMask))
        //            {
        //                _isRising = true;
        //            }
        //        }

        //        return true;
        //    }
        //    else
        //    {
        //        _onGround = false;
        //    }

        //    if (currPos && !_onGround)
        //    {
        //        //Debug.Log("Fall");
        //        Fall();
        //    }

        //    return false;
        //}
    }
}
