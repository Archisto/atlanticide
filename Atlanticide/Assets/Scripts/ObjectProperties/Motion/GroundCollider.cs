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
        private float _groundHitDist;
        private float _minRiseDist;
        private float _maxRiseDist;
        private bool _jumping;

        public float DistanceFallen { get; set; }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _objectSize = GetComponent<Collider>().bounds.size;
            _groundHitDist = _objectSize.y / 2;
            _minRiseDist = 0.80f * _objectSize.y;
            _maxRiseDist = 0.99f * _objectSize.y;
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            // TODO: Get _jumping from object

            if (!World.Instance.GamePaused)
            {
                CheckIfOnGround();

                if (!_onGround)
                {
                    Fall();
                }
            }
        }

        /// <summary>
        /// Source: GC
        /// </summary>
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

        /// <summary>
        /// Source: PC
        /// </summary>
        private void Fall2()
        {
            // TODO: Get from object
            bool climbing = false;

            if (!_jumping && !climbing)
            {
                Fall();
                _onGround = false;
            }
        }

        /// <summary>
        /// Source: GC
        /// </summary>
        private void Rise(float speed)
        {
            Vector3 newPosition = transform.position;
            newPosition.y += speed * World.Instance.DeltaTime;
            transform.position = newPosition;
        }

        private void CheckIfOnGround()
        {

        }

        /// <summary>
        /// Source: PC
        /// </summary>
        private void Jump()
        {
            //if ((!_jumping && _onGround) || Climbing)
            //{
            //    if (Climbing)
            //    {
            //        EndClimb();
            //    }
            //    if (Pushing)
            //    {
            //        EndPush();
            //    }

            //    _jumping = true;
            //    _jumpForce = _jumpHeight * 4;
            //    _onGround = false;
            //}
        }

        /// <summary>
        /// Source: GC
        /// </summary>
        private bool CheckGroundCollision(Vector3 position, bool currPos)
        {
            // TODO: Fix falling when not supposed to.

            Vector3 p1 = position + new Vector3(-0.5f * _objectSize.x, 0, 0.5f * _objectSize.z);
            Vector3 p2 = position + new Vector3(-0.5f * _objectSize.x, 0, 0.5f * _objectSize.z);
            Vector3 p3 = position + new Vector3(0.5f * _objectSize.x, 0, 0.5f * _objectSize.z);
            Vector3 p4 = position + new Vector3(0.5f * _objectSize.x, 0, -0.5f * _objectSize.z);
            RaycastHit hit;
            bool touchingPlatform =
                Physics.Raycast(new Ray(p1, Vector3.down), out hit, _groundHitDist, _platformMask) ||
                Physics.Raycast(new Ray(p2, Vector3.down), out hit, _groundHitDist, _platformMask) ||
                Physics.Raycast(new Ray(p3, Vector3.down), out hit, _groundHitDist, _platformMask) ||
                Physics.Raycast(new Ray(p4, Vector3.down), out hit, _groundHitDist, _platformMask);

            if (touchingPlatform)
            {
                // Rise if currently inside the ground
                if (currPos)
                {
                    DistanceFallen = 0f;

                    if (Physics.Raycast(GetTopOfHeadDownRay(), _minRiseDist, _platformMask))
                    {
                        _isRising = true;
                    }
                }

                return true;
            }
            else
            {
                _onGround = false;
            }

            if (currPos && !_onGround)
            {
                //Debug.Log("Fall");
                Fall();
            }

            return false;
        }

        /// <summary>
        /// Source: PC
        /// </summary>
        /// <param name="position"></param>
        /// <param name="currPos"></param>
        /// <returns></returns>
        private bool CheckGroundCollision2(Vector3 position, bool currPos)
        {
            bool result = CheckGroundCollision(position, currPos);

            if (result && !_jumping)
            {
                _onGround = true;
            }

            return result;
        }

        /// <summary>
        /// Source: PC - Move()
        /// </summary>
        private void SmoothMove()
        {
            Vector3 newPosition = transform.position;
            Vector3 movement = Vector3.zero; // TODO: Difference of new and old pos

            if (_isRising || _jumping)
            {
                transform.position = newPosition;
            }
            else
            {
                float groundHeightDiff = GroundHeightDifference(newPosition);

                // If the slope is too steep upwards, the character doesn't move
                if (groundHeightDiff < 0.5f * _objectSize.y)
                {
                    // If the slope is too steep upwards or downwards, the height difference is ignored.
                    // Slopes that are too steep upwards are handled with the Rise method.
                    // Super minimal height differences are also ignored.
                    if (groundHeightDiff > -0.1f * _objectSize.y &&
                        groundHeightDiff < 0.2f * _objectSize.y &&
                        (groundHeightDiff < -0.0001f * _objectSize.y ||
                        groundHeightDiff > 0.0001f * _objectSize.y))
                    {
                        movement.y = groundHeightDiff;
                        float maxGroundHeiDiff = (groundHeightDiff > 0 ? 0.2f : -0.1f) * _objectSize.y;
                        float ratio = Utils.ReverseRatio(groundHeightDiff, 0, maxGroundHeiDiff);
                        ratio = (ratio < 0.4f ? 0.4f : ratio);

                        // Very steep slope: groundHeightDiff = +-0.03
                        //float slopeSpeedDampening = Utils.Ratio(Mathf.Abs(groundHeightDiff), 0, 1f);
                        //float slopeSpeedDampening = (Mathf.Abs(groundHeightDiff) > 0.07f ? 0.25f : 0.1f);
                        //float slopeSpeedDampening = 0f;

                        movement.x = movement.x * ratio;
                        //movement.x = Utils.WeighValue(movement.x, 0, slopeSpeedDampening);
                        movement.z = movement.z * ratio;
                        //movement.z = Utils.WeighValue(movement.z, 0, slopeSpeedDampening);

                        newPosition = transform.position + movement;

                        //newPosition =
                        //    transform.position +
                        //    new Vector3(direction.x, groundHeightDiff, direction.y).normalized * _speed * World.Instance.DeltaTime;
                        //newPosition.y = transform.position.y + groundHeightDiff;

                        _onGround = true;
                    }

                    //transform.position = GetPositionOffWall(transform.position, newPosition);
                    transform.position = newPosition;
                }
            }
        }

        /// <summary>
        /// Source: GC
        /// </summary>
        protected float GroundHeightDifference(Vector3 position)
        {
            position.y = transform.position.y;
            float groundY = transform.position.y - (_objectSize.y / 2);

            RaycastHit hit;
            bool touchingPlatform =
                Physics.Raycast(new Ray(position, Vector3.down), out hit, _objectSize.y, _platformMask);

            if (touchingPlatform)
            {
                // Positive value for higher ground,
                // negative for lower
                return hit.point.y - groundY;
            }
            else
            {
                // The height difference is "big"
                return -10f;
            }
        }

        /// <summary>
        /// Source: GC
        /// </summary>
        protected bool CheckSimpleGroundCollision()
        {
            return Physics.Raycast(GetTopOfHeadDownRay(), _maxRiseDist, _platformMask);
        }

        /// <summary>
        /// Source: GC
        /// </summary>
        private Ray GetTopOfHeadDownRay()
        {
            return new Ray(transform.position + new Vector3(0, 0.5f * _objectSize.y, 0), Vector3.down);
        }

        public void ResetGroundCollider()
        {
            DistanceFallen = 0f;
        }

        /// <summary>
        /// Draws gizmos.
        /// </summary>
        protected virtual void OnDrawGizmos()
        {
            // Object dimensions
            Gizmos.color = Color.blue;
            Vector3 p1 = transform.position + -0.5f * _objectSize;
            Vector3 p2 = transform.position + new Vector3(-0.5f * _objectSize.x, -0.5f * _objectSize.y, 0.5f * _objectSize.z);
            Vector3 p3 = transform.position + new Vector3(0.5f * _objectSize.x, -0.5f * _objectSize.y, 0.5f * _objectSize.z);
            Vector3 p4 = transform.position + new Vector3(0.5f * _objectSize.x, -0.5f * _objectSize.y, -0.5f * _objectSize.z);
            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p3);
            Gizmos.DrawLine(p3, p4);
            Gizmos.DrawLine(p4, p1);
        }
    }
}
