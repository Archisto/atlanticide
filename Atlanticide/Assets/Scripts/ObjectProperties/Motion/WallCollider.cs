using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    [RequireComponent(typeof(Collider))]
    public class WallCollider : MonoBehaviour
    {
        private const string WallKey = "Wall";
        private const string PlayerKey = "Player";
        private const string NoCollisionKey = "NoCollision";

        [SerializeField]
        private LayerMask _mask;

        [SerializeField]
        private bool _positionIsOnGround;

        private Vector3 _objectSize;
        private int _wallLayerNum;
        private Vector3[] _cardinalDirs;
        //private Vector3[] _sideMidPoints;
        private PlayerCharacter _player;
        private GameObject _obj;
        private Vector3 _midPosition;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            // TODO: Fix being able to go inside the wall by moving diagonally into a corner.

            _obj = transform.root.gameObject;
            _objectSize = GetComponent<Collider>().bounds.size;
            _wallLayerNum = LayerMask.NameToLayer(WallKey);
            _cardinalDirs = Utils.Get4CardinalDirections();
            _player = _obj.GetComponent<PlayerCharacter>();
            UpdateMidPosition();
            //_sideMidPoints = new Vector3[]
            //{
            //    _midPosition + Vector3.forward * _objectSize.z * 0.5f,
            //    _midPosition + Vector3.back * _objectSize.z * 0.5f,
            //    _midPosition + Vector3.left * _objectSize.x * 0.5f,
            //    _midPosition + Vector3.right * _objectSize.x * 0.5f,
            //};
        }

        private void UpdateMidPosition()
        {
            if (_positionIsOnGround)
            {
                _midPosition = _obj.transform.position +
                    Vector3.up * _objectSize.y * 0.5f;
            }
            else
            {
                _midPosition = _obj.transform.position;
            }
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (!World.Instance.GamePaused)
            {
                UpdateMidPosition();
                CheckWallCollisions();
            }
        }

        private void CheckWallCollisions()
        {
            Vector3 movement = GetMovementOffWall(_midPosition);
            _obj.transform.position += movement;
        }

        private Vector3 GetMovementOffWall(Vector3 position)
        {
            Vector3 movement = Vector3.zero;

            if (!CheckClimb())
            {
                RaycastHit hit;
                bool xAxisChecked = false;
                bool zAxisChecked = false;
                bool push = false;

                foreach (Vector3 dir in _cardinalDirs)
                {
                    Vector3 directionMovement = Vector3.zero;

                    hit = HitObjectInPositionInDirection(position, dir, _mask);
                    GameObject hitObj = null;
                    Shield shield = null;
                    if (hit.collider != null)
                    {
                        hitObj = hit.collider.gameObject;
                        shield = hitObj.GetComponent<Shield>();
                    }

                    if (hitObj != null && hitObj != this.gameObject &&
                        !hitObj.tag.Equals(NoCollisionKey) && shield == null)
                    {
                        //if (hit.collider != null && hit.collider.gameObject != gameObject &&
                        //!hit.collider.gameObject.tag.Equals(PlayerKey) &&
                        //!hit.collider.gameObject.tag.Equals(NoCollisionKey))

                        bool wallHit = (hitObj.layer == _wallLayerNum);
                        directionMovement = hit.point - dir * (_objectSize.x * 0.5f) - position;

                        push = CheckPush(hitObj);

                        if (!push || wallHit)
                        {
                            if (dir.x != 0)
                            {
                                if (!xAxisChecked || wallHit)
                                {
                                    movement.x = directionMovement.x;
                                    xAxisChecked = true;
                                }
                            }
                            else
                            {
                                if (!zAxisChecked || wallHit)
                                {
                                    movement.z = directionMovement.z;
                                    zAxisChecked = true;
                                }
                            }
                        }
                    }
                }

                if (!push)
                {
                    EndPush();
                }
            }

            return movement;
        }

        private RaycastHit HitObjectInPositionInDirection(Vector3 position, Vector3 direction, int mask)
        {
            RaycastHit hit;
            Physics.Raycast(new Ray(position, direction),
                            out hit, _objectSize.x * 0.5f, mask);
            return hit;
        }

        private Vector3 GetPositionOffTarget(Vector3 direction, Vector3 target, float distance)
        {
            Vector3 posOffTarget = target - direction * distance;
            return posOffTarget;
        }

        private bool CheckPush(GameObject go)
        {
            bool result = false;

            if (_player != null)
            {
                Pushable pushable = go.GetComponent<Pushable>();
                if (pushable != null)
                {
                    _player.StartPush(pushable);
                    result = true;
                }
                else
                {
                    _player.EndPush();
                }
            }

            return result;
        }

        private void EndPush()
        {
            if (_player != null && _player.Pushing)
            {
                _player.EndPush();
            }
        }

        private bool CheckClimb()
        {
            return (_player != null && _player.Climbing);
        }
    }
}
