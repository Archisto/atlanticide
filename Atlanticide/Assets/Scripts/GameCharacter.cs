using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Atlanticide
{
    public abstract class GameCharacter : MonoBehaviour
    {
        [SerializeField]
        protected float _speed;

        [SerializeField]
        protected float _turningSpeed;

        [SerializeField]
        protected int _maxHitpoints = 3;

        protected int _hitpoints;
        protected bool _isRising;
        protected bool _onGround;
        protected float _distFallen;
        protected Vector3 _characterSize;
        private float _groundHitDist;
        private float _minRiseDist;
        private float _maxRiseDist;
        private LayerMask _platformLayerMask;
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

        public Vector3 RespawnPosition { get; set; }

        public Vector3 Size { get { return _characterSize; } }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected virtual void Start()
        {
            ResetBaseValues();
            RespawnPosition = transform.position;
            _characterSize = GetComponent<Renderer>().bounds.size;
            _groundHitDist = _characterSize.y / 2;
            _minRiseDist = 0.80f * _characterSize.y;
            _maxRiseDist = 0.99f * _characterSize.y;
            _platformLayerMask = LayerMask.GetMask("Platform");
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
                    // Rising if inside the ground
                    Rise(5f);

                    if (!CheckSimpleGroundCollision())
                    {
                        _isRising = false;
                    }
                }
                else
                {
                    // Ground collisions
                    CheckGroundCollision(transform.position, true);
                }
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

        protected float GroundHeightDifference(Vector3 position)
        {
            position.y = transform.position.y;
            float groundY = transform.position.y - (_characterSize.y / 2);

            RaycastHit hit;
            bool touchingPlatform =
                Physics.Raycast(new Ray(position, Vector3.down), out hit, _characterSize.y, _platformLayerMask);

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

        protected bool CheckSimpleGroundCollision()
        {
            return Physics.Raycast(GetTopOfHeadDownRay(), _maxRiseDist, _platformLayerMask);
        }

        protected virtual bool CheckGroundCollision(Vector3 position, bool currPos)
        {
            // TODO: Fix falling when not supposed to.

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
                // Rise if currently inside the ground
                if (currPos)
                {
                    _distFallen = 0;

                    if (Physics.Raycast(GetTopOfHeadDownRay(), _minRiseDist, _platformLayerMask))
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

        private Ray GetTopOfHeadDownRay()
        {
            return new Ray(transform.position + new Vector3(0, 0.5f * _characterSize.y, 0), Vector3.down);
        }

        /// <summary>
        /// Makes the character fall.
        /// </summary>
        protected virtual void Fall()
        {
            if (_isRising)
            {
                return;
            }

            float fallSpeed =
                (World.Instance.gravity + 2 * _distFallen) * World.Instance.DeltaTime;
            Vector3 newPosition = transform.position;
            newPosition.y -= fallSpeed;
            _distFallen += fallSpeed;
            transform.position = newPosition;

            if (_distFallen > 20)
            {
                Die();
            }
        }

        /// <summary>
        /// Makes the character rise.
        /// </summary>
        protected virtual void Rise(float speed)
        {
            Vector3 newPosition = transform.position;
            newPosition.y += speed * World.Instance.DeltaTime;
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

        /// <summary>
        /// Kills the character.
        /// </summary>
        protected virtual void Die()
        {
            IsDead = true;
            CancelActions();
            _telegrabability.SetActive(false);
            Debug.Log(name + " died.");
        }

        /// <summary>
        /// Respawns the character.
        /// </summary>
        public virtual void Respawn()
        {
            ResetBaseValues();
            ResetPosition();
            gameObject.SetActive(true);
            _telegrabability.SetActive(true);
            Debug.Log(name + " respawned.");
        }

        /// <summary>
        /// Resets the character's base values.
        /// </summary>
        protected virtual void ResetBaseValues()
        {
            IsDead = false;
            _hitpoints = _maxHitpoints;
            _isRising = false;
            _distFallen = 0;
        }

        public void ResetPosition()
        {
            transform.position = RespawnPosition;
        }

        /// <summary>
        /// Cancels the character's current actions.
        /// </summary>
        public virtual void CancelActions()
        {
        }

        /// <summary>
        /// Draws gizmos.
        /// </summary>
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
