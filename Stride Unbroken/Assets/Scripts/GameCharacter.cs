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

        protected bool _isDead;
        private Vector3 _characterSize;
        private float _groundHitDist = 3f;
        private LayerMask _platformLayerMask;

        public string CharacterName { get; set; }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected virtual void Start()
        {
            _characterSize = GetComponent<Renderer>().bounds.size;
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
            float rotSpeed = 0.3f;
            direction = new Vector3(direction.x, 0, direction.y);
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            Quaternion newRotation = Quaternion.Lerp(transform.rotation, targetRotation, rotSpeed);
            transform.rotation = newRotation;
        }

        private bool CheckCollision()
        {
            Vector3 p1 = transform.position + new Vector3(-0.5f * _characterSize.x, 0, 0.5f * _characterSize.z);
            Vector3 p2 = transform.position + new Vector3(-0.5f * _characterSize.x, 0, 0.5f * _characterSize.z);
            Vector3 p3 = transform.position + new Vector3(0.5f * _characterSize.x, 0, 0.5f * _characterSize.z);
            Vector3 p4 = transform.position + new Vector3(0.5f * _characterSize.x, 0, -0.5f * _characterSize.z);
            Ray ray1 = new Ray(p1, Vector3.down);
            Ray ray2 = new Ray(p2, Vector3.down);
            Ray ray3 = new Ray(p3, Vector3.down);
            Ray ray4 = new Ray(p4, Vector3.down);
            RaycastHit hit;
            bool touchingPlatform =
                Physics.Raycast(ray1, out hit, _groundHitDist, _platformLayerMask) ||
                Physics.Raycast(ray2, out hit, _groundHitDist, _platformLayerMask) ||
                Physics.Raycast(ray3, out hit, _groundHitDist, _platformLayerMask) ||
                Physics.Raycast(ray4, out hit, _groundHitDist, _platformLayerMask);

            if (touchingPlatform)
            {
                Platform platform = hit.transform.GetComponent<Platform>();
                if (platform != null)
                {
                    return true;
                }
            }

            return false;
        }

        protected virtual void Die()
        {
            // TODO
            _isDead = true;
            Debug.Log(CharacterName + " died.");
            Respawn();
        }

        public virtual void Respawn()
        {
            _isDead = false;
            Debug.Log(CharacterName + " respawned.");
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
