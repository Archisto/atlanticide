using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// A score pickup that is collected by walking over it.
    /// </summary>
    public class Pickup : MonoBehaviour
    {
        [SerializeField]
        private int _score = 100;

        protected bool _collected;
        private Vector3 _originalPosition;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _originalPosition = transform.position;
            ResetPickup();
        }

        /// <summary>
        /// Resets the pickup.
        /// </summary>
        public virtual void ResetPickup()
        {
            _collected = false;
            gameObject.SetActive(true);
            transform.position = _originalPosition;
        }

        /// <summary>
        /// Handles colliding with the player characters.
        /// </summary>
        /// <param name="collision">The collision</param>
        protected virtual void OnCollisionEnter(Collision collision)
        {
            PlayerCharacter pc = collision.gameObject.GetComponent<PlayerCharacter>();
            if (pc != null)
            {
                Collect(pc);
            }
        }

        /// <summary>
        /// Gives score and destroys the pickup.
        /// </summary>
        /// <param name="character">A player character</param>
        protected virtual void Collect(PlayerCharacter character)
        {
            GameManager.Instance.UpdateScore(_score);
            _collected = true;
            Destroy();
        }

        /// <summary>
        /// Destroys the pickup.
        /// </summary>
        protected virtual void Destroy()
        {
            gameObject.SetActive(false);
        }
    }
}
