using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class WallCollider : MonoBehaviour
    {
        private const string WallKey = "Wall";
        private const string PlayerKey = "Player";

        [SerializeField]
        private LayerMask _mask;

        private Vector3 _objectSize;
        private int _wallLayerNum;
        private Vector3[] _cardinalDirs;
        private PlayerCharacter _player;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _objectSize = GetComponent<Renderer>().bounds.size;
            _wallLayerNum = LayerMask.NameToLayer(WallKey);
            _cardinalDirs = Utils.Get4CardinalDirections();
            _player = GetComponent<PlayerCharacter>();
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (!World.Instance.GamePaused)
            {
                CheckWallCollisions();
            }
        }

        private void CheckWallCollisions()
        {
            // TODO: Fix being lowered into the ground if the character's right side
            // hangs off a ledge while colliding with a wall in front. 

            Vector3 movement = GetMovementOffWall(transform.position);
            transform.position += movement;
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
                    if (hit.collider != null && hit.collider.gameObject != gameObject &&
                        !hit.collider.gameObject.tag.Equals(PlayerKey))
                    {
                        bool wallHit = (hit.collider.gameObject.layer == _wallLayerNum);
                        directionMovement = hit.point - dir * (_objectSize.x * 0.5f) - position;

                        push = CheckPush(hit.collider.gameObject);

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
            Physics.Raycast(new Ray(position - direction * (_objectSize.x * 0.5f), direction),
                            out hit, _objectSize.x, mask);
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
