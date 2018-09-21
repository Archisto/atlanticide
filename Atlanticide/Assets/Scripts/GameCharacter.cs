using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Atlanticide
{
    public class GameCharacter : MonoBehaviour
    {
        [SerializeField]
        protected float _speed;

        [SerializeField]
        protected float _turningSpeed;

        [SerializeField]
        protected int _maxHitpoints = 3;

        protected int _hitpoints;
        protected bool _isRising;
        protected float _distFallen;
        private Vector3 _respawnPosition;
        protected GameObject _myWall;
        private Vector3 _characterSize;
        private float _groundHitDist;
        private float _wallHitDist;
        private LayerMask _platformLayerMask;
        private LayerMask _wallLayerMask;
        private Telegrabable _telegrabability;

        public bool IsInvulnerable { get; set; }

        public bool IsImmobile { get; set; }

        public bool IsDead { get; protected set; }

        public bool Telegrabbed
        {
            get
            {
                return _telegrabability != null && _telegrabability.Telegrabbed;
            }
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected virtual void Start()
        {
            _hitpoints = _maxHitpoints;
            _respawnPosition = transform.position;
            _characterSize = GetComponent<Renderer>().bounds.size;
            _groundHitDist = _characterSize.y / 2;
            _wallHitDist = _characterSize.x / 2;
            _platformLayerMask = LayerMask.GetMask("Platform");
            _wallLayerMask = LayerMask.GetMask("Wall");
            _telegrabability = GetComponent<Telegrabable>();
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        protected virtual void Update()
        {
            if (IsDead)
            {
                return;
            }

            if (!Telegrabbed)
            {
                if (_isRising)
                {
                    Rise(5f);

                    float maxRiseDist = 0.99f * _characterSize.y;
                    if (!Physics.Raycast(GetTopOfHeadDownRay(), maxRiseDist, _platformLayerMask))
                    {
                        _isRising = false;
                    }
                }
                else
                {
                    CheckGroundCollision(transform.position, true);
                }
            }

            Vector3? newPosition = GetPositionOffWall(transform.position, transform.position);
            if (newPosition != null)
            {
                transform.position = newPosition.Value;
            }
        }

        protected virtual void LookTowards(Vector3 direction)
        {
            direction = new Vector3(direction.x, 0, direction.y);
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }

        protected virtual void RotateTowards(Vector3 direction)
        {
            direction = new Vector3(direction.x, 0, direction.y);
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            Quaternion newRotation = Quaternion.Lerp(transform.rotation, targetRotation, _turningSpeed);
            transform.rotation = newRotation;
        }

        protected bool CheckGroundCollision(Vector3 position, bool currPos)
        {
            Vector3 p1 = position + new Vector3(-0.5f * _characterSize.x, 0, 0.5f * _characterSize.z);
            Vector3 p2 = position + new Vector3(-0.5f * _characterSize.x, 0, 0.5f * _characterSize.z);
            Vector3 p3 = position + new Vector3(0.5f * _characterSize.x, 0, 0.5f * _characterSize.z);
            Vector3 p4 = position + new Vector3(0.5f * _characterSize.x, 0, -0.5f * _characterSize.z);
            RaycastHit hit;
            bool touchingPlatform =
                Physics.Raycast(new Ray(p1, Vector3.down), out hit, _groundHitDist, _platformLayerMask) ||
                Physics.Raycast(new Ray(p2, Vector3.down), out hit, _groundHitDist, _platformLayerMask) ||
                Physics.Raycast(new Ray(p3, Vector3.down), out hit, _groundHitDist, _platformLayerMask) ||
                Physics.Raycast(new Ray(p4, Vector3.down), out hit, _groundHitDist, _platformLayerMask);

            if (touchingPlatform)
            {
                if (currPos)
                {
                    _distFallen = 0;

                    float minRiseDist = 0.8f * _characterSize.y;
                    if (Physics.Raycast(GetTopOfHeadDownRay(), minRiseDist, _platformLayerMask))
                    {
                        _isRising = true;
                    }
                }

                return true;
            }

            if (currPos)
            {
                Fall();
            }

            return false;
        }

        protected Vector3? GetPositionOffWall(Vector3 oldPosition, Vector3 position)
        {
            Vector3 result = position;
            RaycastHit hit;
            bool touchingWall =
                Physics.Raycast(new Ray(position + Vector3.back * _wallHitDist, Vector3.forward), out hit, 2 * _wallHitDist, _wallLayerMask) ||
                Physics.Raycast(new Ray(position + Vector3.right * _wallHitDist, Vector3.left), out hit, 2 * _wallHitDist, _wallLayerMask) ||
                Physics.Raycast(new Ray(position + Vector3.left * _wallHitDist, Vector3.right), out hit, 2 * _wallHitDist, _wallLayerMask) ||
                Physics.Raycast(new Ray(position + Vector3.forward * _wallHitDist, Vector3.back), out hit, 2 * _wallHitDist, _wallLayerMask);

            if (touchingWall && hit.transform.gameObject != _myWall)
            {
                Vector3 hitDirection = hit.point - position;

                if (hitDirection.x != 0)
                {
                    //result.x = oldPosition.x;
                    result.x -= hitDirection.x;
                }
                if (hitDirection.z != 0)
                {
                    //result.z = oldPosition.z;
                    result.z -= hitDirection.z;
                }

                return result;
            }
            else
            {
                return null;
            } 
        }

        private Ray GetTopOfHeadDownRay()
        {
            return new Ray(transform.position + new Vector3(0, 0.5f * _characterSize.y, 0), Vector3.down);
        }

        protected virtual void Fall()
        {
            if (_isRising)
            {
                return;
            }

            float fallDistance =
                World.Instance.gravity * Time.deltaTime + 0.05f * _distFallen;
            Vector3 newPosition = transform.position;
            newPosition.y -= fallDistance;
            _distFallen += fallDistance;
            transform.position = newPosition;

            if (_distFallen > 20)
            {
                Die();
            }
        }

        protected virtual void Rise(float speed)
        {
            Vector3 newPosition = transform.position;
            newPosition.y += speed * Time.deltaTime;
            transform.position = newPosition;
        }

        /// <summary>
        /// Makes the character take damage.
        /// </summary>
        /// <param name="damage">The damage amount</param>
        /// <returns>Does the character die.</returns>
        public virtual bool TakeDamage(int damage)
        {
            _hitpoints -= damage;

            if (_hitpoints <= 0)
            {
                _hitpoints = 0;
                Die();
                return true;
            }

            return false;
        }

        protected virtual void Die()
        {
            IsDead = true;
            _telegrabability.Telegrabbed = false;
            Debug.Log(name + " died.");
        }

        public virtual void Respawn()
        {
            IsDead = false;
            _hitpoints = _maxHitpoints;
            _isRising = false;
            _distFallen = 0;
            transform.position = _respawnPosition;
            Debug.Log(name + " respawned.");
        }

        protected virtual void OnDrawGizmos()
        {
            // Character dimensions
            Gizmos.color = Color.blue;
            Vector3 p1 = transform.position + -0.5f * _characterSize;
            Vector3 p2 = transform.position + new Vector3(-0.5f * _characterSize.x, -0.5f * _characterSize.y, 0.5f * _characterSize.z);
            Vector3 p3 = transform.position + new Vector3(0.5f * _characterSize.x, -0.5f * _characterSize.y, 0.5f * _characterSize.z);
            Vector3 p4 = transform.position + new Vector3(0.5f * _characterSize.x, -0.5f * _characterSize.y, -0.5f * _characterSize.z);
            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p3);
            Gizmos.DrawLine(p3, p4);
            Gizmos.DrawLine(p4, p1);
        }
    }
}
