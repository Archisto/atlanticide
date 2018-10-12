using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class GroundCollider : MonoBehaviour
    {
        public bool onGround = true;

        [SerializeField]
        private LayerMask _platformMask;

        [SerializeField]
        private float _riseSpeed = 5f;

        [SerializeField]
        private float _maxFallDistance = 20f;

        private Vector3 _objectSize;
        private bool _usedToBeOnGround;
        private bool _isRising;
        private float _groundHitDist;
        private float _minRiseDist;
        private float _maxRiseDist;
        private bool _fallenOffMap;
        private Vector3 _oldPosition;
        private LevelObject _levelObj;
        private GameCharacter _character;
        private PlayerCharacter _player;

        public float DistanceFallen { get; set; }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _levelObj = GetComponent<LevelObject>();
            if (_levelObj == null)
            {
                _character = GetComponent<GameCharacter>();

                if (_character != null)
                {
                    _player = _character as PlayerCharacter;
                }
            }

            if (_levelObj == null && _character == null)
            {
                Debug.LogWarning("Object type unknown.");
            }

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
            if (!World.Instance.GamePaused &&
                CheckIfObjAvailable())
            {
                UpdateFallingAndRising();
                _oldPosition = transform.position;
            }
        }

        private void UpdateFallingAndRising()
        {
            // TODO: Fix shaking up and down (Rise, Fall and falsely not being on ground).
            // TODO: Fix being lowered into the ground if the character's right side
            // hangs off a ledge while colliding with a wall in front. 

            // Rising if inside the ground
            if (_isRising)
            {
                Rise(_riseSpeed);
                StartOrStopRising(false);
            }
            // Falling or staying on ground
            else
            {
                onGround = CheckIfObjOnGround();
                StartOrStopRising(true);
                if (!onGround) Debug.Log("onGround: " + onGround);
                if (!onGround && !_isRising)
                {
                    Fall();
                }
                else
                {
                    if (!_usedToBeOnGround)
                    {
                        DistanceFallen = 0f;
                    }

                    if (transform.position != _oldPosition)
                    {
                        SmoothMove();
                    }
                }

                _usedToBeOnGround = onGround;
            }
        }

        private bool CheckIfObjOnGround()
        {
            Vector3 p1 = transform.position + new Vector3(-0.5f * _objectSize.x, 0, 0.5f * _objectSize.z);
            Vector3 p2 = transform.position + new Vector3(-0.5f * _objectSize.x, 0, 0.5f * _objectSize.z);
            Vector3 p3 = transform.position + new Vector3(0.5f * _objectSize.x, 0, 0.5f * _objectSize.z);
            Vector3 p4 = transform.position + new Vector3(0.5f * _objectSize.x, 0, -0.5f * _objectSize.z);
            RaycastHit hit;
            bool touchingPlatform =
                Physics.Raycast(new Ray(p1, Vector3.down), out hit, _groundHitDist, _platformMask) ||
                Physics.Raycast(new Ray(p2, Vector3.down), out hit, _groundHitDist, _platformMask) ||
                Physics.Raycast(new Ray(p3, Vector3.down), out hit, _groundHitDist, _platformMask) ||
                Physics.Raycast(new Ray(p4, Vector3.down), out hit, _groundHitDist, _platformMask);

            return touchingPlatform;
        }

        private bool CheckIfObjOnGroundSimple()
        {
            return Physics.Raycast(GetTopOfHeadDownRay(), _maxRiseDist, _platformMask);
        }

        private bool CheckIfObjInsideGround()
        {
            return Physics.Raycast(GetTopOfHeadDownRay(), _minRiseDist, _platformMask);
        }

        private bool CheckIfObjAvailable()
        {
            bool result = false;

            if (_fallenOffMap)
            {
                result = false;
            }
            else if (_levelObj != null)
            {
                result = true;
            }
            else if (_character != null)
            {
                bool dead = _character.IsDead;
                bool climbing = false;
                bool jumping = false;

                if (_player != null)
                {
                    climbing = _player.Climbing;
                    jumping = _player.Jumping;
                }

                result = !dead && !climbing && !jumping;
            }

            return result;
        }

        public float GroundHeightDifference(Vector3 position)
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
                return -10;
            }
        }

        private void Fall()
        {
            if (_isRising)
            {
                return;
            }

            float fallAmount =
                (World.Instance.gravity + 2 * DistanceFallen) * World.Instance.DeltaTime;
            transform.position -= Vector3.up * fallAmount;

            DistanceFallen += fallAmount;
            if (DistanceFallen >= _maxFallDistance)
            {
                _fallenOffMap = true;
                Debug.Log(name + " fell off map");

                if (_character != null)
                {
                    _character.Kill();
                }
            }
        }

        public void Rise(float speed)
        {
            float riseAmount = speed * World.Instance.DeltaTime;
            transform.position += Vector3.up * riseAmount;
        }

        private void StartOrStopRising(bool startRising)
        {
            if (startRising)
            {
                if (CheckIfObjInsideGround())
                {
                    _isRising = true;
                }
            }
            else
            {
                if (!CheckIfObjOnGroundSimple())
                {
                    _isRising = false;
                }
            }
        }

        private Ray GetTopOfHeadDownRay()
        {
            return new Ray(transform.position + new Vector3(0, 0.5f * _objectSize.y, 0), Vector3.down);
        }

        private bool SmoothMove()
        {
            Vector3 newPosition = transform.position;
            Vector3 movement = transform.position - _oldPosition;
            float groundHeightDiff = GroundHeightDifference(transform.position);

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

                    newPosition = _oldPosition + movement;

                    //newPosition =
                    //    transform.position +
                    //    new Vector3(direction.x, groundHeightDiff, direction.y).normalized * _speed * World.Instance.DeltaTime;
                    //newPosition.y = transform.position.y + groundHeightDiff;

                    onGround = true;
                }

                //transform.position = GetPositionOffWall(transform.position, newPosition);
                transform.position = newPosition;
                return true;
            }

            return false;
        }

        public void ResetGroundCollider()
        {
            DistanceFallen = 0f;
            _fallenOffMap = false;
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
