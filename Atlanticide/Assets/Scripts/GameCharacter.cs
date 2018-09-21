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

        protected bool _isDead;
        protected bool _isRising;
        protected float _distFallen;
        protected GameObject _myWall;
        private Vector3 _characterSize;
        private float _groundHitDist;
        private float _wallHitDist;
        private LayerMask _platformLayerMask;
        private LayerMask _wallLayerMask;
        private Telegrabable _telegrabability;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected virtual void Start()
        {
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
            if (_isDead)
            {
                return;
            }

            if (_telegrabability == null || !_telegrabability.telegrabbed)
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
                    CheckGroundCollision();
                }
            }

            Vector3? newPosition = GetPositionOffWall(transform.position, transform.position);
            if (newPosition != null)
            {
                Debug.Log("touch");
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

        protected bool CheckGroundCollision()
        {
            Vector3 p1 = transform.position + new Vector3(-0.5f * _characterSize.x, 0, 0.5f * _characterSize.z);
            Vector3 p2 = transform.position + new Vector3(-0.5f * _characterSize.x, 0, 0.5f * _characterSize.z);
            Vector3 p3 = transform.position + new Vector3(0.5f * _characterSize.x, 0, 0.5f * _characterSize.z);
            Vector3 p4 = transform.position + new Vector3(0.5f * _characterSize.x, 0, -0.5f * _characterSize.z);
            RaycastHit hit;
            bool touchingPlatform =
                Physics.Raycast(new Ray(p1, Vector3.down), out hit, _groundHitDist, _platformLayerMask) ||
                Physics.Raycast(new Ray(p2, Vector3.down), out hit, _groundHitDist, _platformLayerMask) ||
                Physics.Raycast(new Ray(p3, Vector3.down), out hit, _groundHitDist, _platformLayerMask) ||
                Physics.Raycast(new Ray(p4, Vector3.down), out hit, _groundHitDist, _platformLayerMask);

            if (touchingPlatform)
            {
                _distFallen = 0;

                float minRiseDist = 0.8f * _characterSize.y;
                if (Physics.Raycast(GetTopOfHeadDownRay(), minRiseDist, _platformLayerMask))
                {
                    _isRising = true;
                }

                return true;
            }

            Fall();
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

            Debug.Log("Dist fallen: " + _distFallen);

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

        protected virtual void Die()
        {
            // TODO
            _isDead = true;
            Debug.Log(name + " died.");
            Respawn();
        }

        public virtual void Respawn()
        {
            _isDead = false;
            _isRising = false;
            _distFallen = 0;
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
