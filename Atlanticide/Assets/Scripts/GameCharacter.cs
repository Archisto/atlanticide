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
        protected float _distFallen;
        private Vector3 _characterSize;
        private float _groundHitDist;
        private LayerMask _platformLayerMask;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected virtual void Start()
        {
            _characterSize = GetComponent<Renderer>().bounds.size;
            _groundHitDist = _characterSize.y / 2;
            _platformLayerMask = LayerMask.GetMask("Platform");
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
            Vector3 topOfHead = transform.position + new Vector3(0, 0.5f * _characterSize.y, 0);
            RaycastHit hit;
            bool touchingPlatform =
                Physics.Raycast(GetDownRay(p1), out hit, _groundHitDist, _platformLayerMask) ||
                Physics.Raycast(GetDownRay(p2), out hit, _groundHitDist, _platformLayerMask) ||
                Physics.Raycast(GetDownRay(p3), out hit, _groundHitDist, _platformLayerMask) ||
                Physics.Raycast(GetDownRay(p4), out hit, _groundHitDist, _platformLayerMask);

            if (touchingPlatform)
            {
                _distFallen = 0;

                if (Physics.Raycast(GetDownRay(topOfHead), 0.99f * _characterSize.y, _platformLayerMask))
                {
                    Rise(2f);
                }

                return true;
            }

            Fall();
            return false;
        }

        private Ray GetDownRay(Vector3 point)
        {
            return new Ray(point, Vector3.down);
        }

        protected virtual void Fall()
        {
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
