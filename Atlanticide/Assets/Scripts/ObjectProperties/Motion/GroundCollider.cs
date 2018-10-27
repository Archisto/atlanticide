using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    [RequireComponent(typeof(Collider))]
    public class GroundCollider : MonoBehaviour
    {
        private const string SlopeKey = "Slope";

        public bool onGround = true;
        public bool onMovingPlatform;

        [Header("OBJECT (choose one)")]

        [SerializeField]
        private LevelObject _levelObj;

        [SerializeField]
        private GameCharacter _character;

        [Header("CONFIG")]

        [SerializeField]
        private LayerMask _platformMask;

        [SerializeField]
        private bool _pivotPointOnFoot;

        [SerializeField]
        private float _riseSpeed = 5f;

        [SerializeField]
        private float _maxFallDistance = 20f;

        [SerializeField]
        private float _hoverOffset;

        [SerializeField, Range(0.05f, 1f)]
        private float _jumpDisableTime = 0.1f;

        private GameObject _rootObj;
        private PlayerCharacter _player;
        private Vector3 _objectSize;
        private bool _usedToBeOnGround;
        private bool _isRising;
        private float _headHeightFromPosition;
        private float _groundHitDist;
        private float _startRisingDist;
        private float _stopRisingDist;
        private bool _fallenOffMap;
        private Vector3 _topOfHead;
        private Vector3 _foot;
        private Vector3 _oldPosition;
        private float _jumpDisableElapsedTime;

        private Vector3 _oldMovingPlatformPos;

        // Testing
        //private float _groundHeightDiff;

        public float DistanceFallen { get; set; }

        public bool AbleToJump
        {
            get { return _jumpDisableElapsedTime < _jumpDisableTime; }
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            InitObject();
            _objectSize = GetComponent<Collider>().bounds.size;
            _headHeightFromPosition = (_pivotPointOnFoot ? 1f : 0.5f) * _objectSize.y;
            _groundHitDist = 0.5f;
            _startRisingDist = 0.98f * _objectSize.y;
            _stopRisingDist = 1f * _objectSize.y;
            UpdateTopOfHeadAndFootPositions();
        }

        private void InitObject()
        {
            if (_character != null)
            {
                _player = _character as PlayerCharacter;
            }

            if (_levelObj != null || _character != null)
            {
                if (_levelObj != null)
                {
                    _rootObj = _levelObj.gameObject;
                }
                else
                {
                    _rootObj = _character.gameObject;
                }
            }
            else
            {
                // NOTE:
                // If the object is the child of a game object
                // just for keeping the scene hierarchy tidy
                // (e.g. Environment object), this may break the game!
                // Please always set _levelObj or _character.
                _rootObj = transform.root.gameObject;
                Debug.LogWarning("Object type unknown. The root object is now "
                    + _rootObj.name + ".");
            }
        }

        private void UpdateTopOfHeadAndFootPositions()
        {
            _topOfHead = _rootObj.transform.position +
                Vector3.up * _headHeightFromPosition;
            _foot = _topOfHead + Vector3.down * _objectSize.y;
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (!World.Instance.GamePaused)
            {
                UpdateTopOfHeadAndFootPositions();

                if (CheckIfObjAvailable())
                {
                    UpdateFallingAndRising();
                    _oldPosition = transform.position;
                }

                UpdateTopOfHeadAndFootPositions();

                if (_player != null && _player.ID == 1)
                {
                    Debug.Log("DistFallen: " + DistanceFallen);
                }
            }
        }

        private void UpdateFallingAndRising()
        {
            // TODO: Fix being lowered into the ground if the character's right side
            // hangs off a ledge while colliding with a wall in front. 

            // Rising if inside the ground
            if (_isRising)
            {
                Rise(_riseSpeed);
                StartOrStopRising(false);

                if (DistanceFallen > 0f)
                {
                    DistanceFallen = 0f;
                }
            }
            // Falling or staying on ground
            else
            {
                onGround = CheckIfObjOnGround();
                StartOrStopRising(true);
                //if (!onGround) Debug.Log(name + " onGround: " + onGround);
                if (onGround)
                {
                    //Debug.Log("On ground");
                    UpdateOnGround();
                }
                else
                {
                    //Debug.Log("Off ground");
                    UpdateOffGround();
                }

                _usedToBeOnGround = onGround;
            }
        }

        private void UpdateOnGround()
        {
            if (DistanceFallen > 0f)
            {
                DistanceFallen = 0f;
            }

            if (!_usedToBeOnGround)
            {
                _jumpDisableElapsedTime = 0f;
            }

            if (_rootObj.transform.position != _oldPosition)
            {
                SmoothMove();
            }
        }

        private void UpdateOffGround()
        {
            if (_usedToBeOnGround)
            {
                onMovingPlatform = false;
            }

            if (!_isRising)
            {
                Fall();

                if (AbleToJump)
                {
                    _jumpDisableElapsedTime += World.Instance.DeltaTime;
                }
            }
        }

        public void JumpOffGround()
        {
            onGround = false;
            onMovingPlatform = false;
            _jumpDisableElapsedTime = _jumpDisableTime;
        }

        private bool CheckIfObjOnGround()
        {
            Vector3 p0 = _foot + Vector3.up * _groundHitDist;
            Vector3 p1 = _foot + new Vector3(-0.5f * _objectSize.x, _groundHitDist, 0.5f * _objectSize.z);
            Vector3 p2 = _foot + new Vector3(-0.5f * _objectSize.x, _groundHitDist, 0.5f * _objectSize.z);
            Vector3 p3 = _foot + new Vector3(0.5f * _objectSize.x, _groundHitDist, 0.5f * _objectSize.z);
            Vector3 p4 = _foot + new Vector3(0.5f * _objectSize.x, _groundHitDist, -0.5f * _objectSize.z);
            RaycastHit hit;
            bool touchingPlatform =
                Physics.Raycast(new Ray(p0, Vector3.down), out hit, _groundHitDist + _hoverOffset, _platformMask) ||
                Physics.Raycast(new Ray(p1, Vector3.down), out hit, _groundHitDist + _hoverOffset, _platformMask) ||
                Physics.Raycast(new Ray(p2, Vector3.down), out hit, _groundHitDist + _hoverOffset, _platformMask) ||
                Physics.Raycast(new Ray(p3, Vector3.down), out hit, _groundHitDist + _hoverOffset, _platformMask) ||
                Physics.Raycast(new Ray(p4, Vector3.down), out hit, _groundHitDist + _hoverOffset, _platformMask);

            if (touchingPlatform && hit.transform.tag.Equals("Moving"))
            {
                Vector3 newMovingPlatformPos = hit.transform.position;

                if (onMovingPlatform)
                {
                    _rootObj.transform.position += (newMovingPlatformPos - _oldMovingPlatformPos);
                }

                _oldMovingPlatformPos = hit.transform.position;
                onMovingPlatform = true;
            }

            //Debug.Log("Hit " + touchingPlatform);
            return touchingPlatform;
        }

        private bool ShouldRise()
        {
            return Physics.Raycast(new Ray(_topOfHead, Vector3.down),
                _startRisingDist, _platformMask);
        }

        private bool ShouldStopRising()
        {
            return !Physics.Raycast(new Ray(_topOfHead, Vector3.down),
                _stopRisingDist, _platformMask);
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

        public float GroundHeightDifferenceFromPos(Vector3 position)
        {
            // Checks half the object's height up from the given position
            // to half the object's height down from the given position

            Vector3 raisedPosition = position + Vector3.up * _objectSize.y * 0.5f;

            RaycastHit hit;
            bool touchingPlatform =
                Physics.Raycast(new Ray(raisedPosition, Vector3.down), out hit, _objectSize.y, _platformMask);

            if (touchingPlatform)
            {
                //Debug.Log("Hit " + hit.transform.name);

                // Positive value for higher ground,
                // negative for lower
                return hit.point.y - position.y;
            }
            else
            {
                // The height difference is "big"
                return -10;
            }
        }

        /// <summary>
        /// Returns the ground height difference between <paramref name="position"/>
        /// and the object's current position if the ground is tagged as slope.
        /// If there's no ground, the ground is not tagged as slope,
        /// or the height difference is big, returns -10.
        /// </summary>
        /// <param name="position">A position different from current</param>
        /// <returns>Ground height difference</returns>
        public float GroundHeightDifference(Vector3 position)
        {
            position.y = _foot.y + _objectSize.y * 0.5f;

            RaycastHit hit;
            bool touchingPlatform =
                Physics.Raycast(new Ray(position, Vector3.down), out hit, _objectSize.y, _platformMask);

            // TODO: Remove SlopeKey!
            if (touchingPlatform && hit.transform.tag.Equals(SlopeKey))
            {
                // Positive value for higher ground,
                // negative for lower
                return hit.point.y - _foot.y;
            }
            else
            {
                // The height difference is "big"
                return -10;
            }
        }

        public float GroundHeightDifference(Vector3 position, float maxDropDist)
        {
            position.y = _foot.y + _objectSize.y * 0.5f;

            RaycastHit hit;
            bool touchingPlatform = Physics.Raycast(new Ray(position, Vector3.down),
                out hit, maxDropDist + (_objectSize.y / 2), _platformMask);

            if (touchingPlatform)
            {
                // Positive value for higher ground,
                // negative for lower
                return hit.point.y - _foot.y;
            }
            else
            {
                // The height difference is "big"
                return -1 * maxDropDist - 1f;
            }
        }

        private void Fall()
        {
            if (_isRising)
            {
                return;
            }

            // TODO: This is a placeholder
            float fallAmount =
                (World.Instance.gravity + 2 * DistanceFallen) * World.Instance.DeltaTime;

            Vector3 newPosition = _rootObj.transform.position - Vector3.up * fallAmount;
            float groundHeightDifference = GroundHeightDifferenceFromPos(newPosition);

            if (groundHeightDifference < 0f)
            {
                _rootObj.transform.position = newPosition;
                //Debug.Log("Falling " + fallAmount);
            }
            else
            {
                _rootObj.transform.position = newPosition +
                    Vector3.up * groundHeightDifference;
            }

            DistanceFallen += fallAmount;
            if (DistanceFallen >= _maxFallDistance)
            {
                _fallenOffMap = true;
                Debug.Log(_rootObj.name + " fell off map");

                if (_character != null)
                {
                    _character.Kill();
                }
            }
        }

        public void Rise(float speed)
        {
            float riseAmount = speed * World.Instance.DeltaTime;
            _rootObj.transform.position += Vector3.up * riseAmount;
        }

        private void StartOrStopRising(bool startRising)
        {
            if (startRising)
            {
                if (ShouldRise())
                {
                    _isRising = true;
                    _jumpDisableElapsedTime = 0f;
                }
            }
            else
            {
                if (ShouldStopRising())
                {
                    _isRising = false;
                }
            }
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
                    groundHeightDiff < 0.15f * _objectSize.y &&
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
                _rootObj.transform.position = newPosition;
                return true;
            }

            return false;
        }

        public void ResetGroundCollider()
        {
            DistanceFallen = 0f;
            _fallenOffMap = false;
            onMovingPlatform = false;
            _jumpDisableElapsedTime = 0f;
        }

        /// <summary>
        /// Draws gizmos.
        /// </summary>
        protected virtual void OnDrawGizmos()
        {
            if (_rootObj != null)
            {
                // Testing
                Gizmos.color = (AbleToJump ? Color.cyan : Color.red);
                if (_player != null && _player.ID == 0)
                {
                    Gizmos.DrawLine(_foot, _foot + Vector3.down * 1.5f);
                }

                // Max rise distance
                //Vector3 topOfHead = _obj.transform.position +
                //    Vector3.up * _headHeightFromPosition;
                //Gizmos.DrawLine(topOfHead, topOfHead + Vector3.down * _maxRiseDist);

                //Gizmos.DrawLine(topOfHead, Vector3.down * _minRiseDist);

                // Object dimensions
                Gizmos.color = Color.blue;
                Vector3 p1 = _foot + new Vector3(-0.5f * _objectSize.x, 0f, -0.5f * _objectSize.z);
                Vector3 p2 = _foot + new Vector3(-0.5f * _objectSize.x, 0f, 0.5f * _objectSize.z);
                Vector3 p3 = _foot + new Vector3(0.5f * _objectSize.x, 0f, 0.5f * _objectSize.z);
                Vector3 p4 = _foot + new Vector3(0.5f * _objectSize.x, 0f, -0.5f * _objectSize.z);
                Gizmos.DrawLine(p1, p2);
                Gizmos.DrawLine(p2, p3);
                Gizmos.DrawLine(p3, p4);
                Gizmos.DrawLine(p4, p1);
            }
        }
    }
}
