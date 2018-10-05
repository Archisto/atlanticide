using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// A score pickup that is collected by walking over it.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Pickup : LevelObject
    {
        [SerializeField]
        private int _score = 100;

        protected PickupExpansion _expansion;

        public bool IsCollected { get; protected set; }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _expansion = GetComponent<PickupExpansion>();
            _defaultPosition = transform.position;
            ResetObject();
        }

        public void SetDefaultPosition(Vector3 position)
        {
            _defaultPosition = position;
        }

        /// <summary>
        /// Handles colliding with the player characters.
        /// </summary>
        /// <param name="collision">The collision</param>
        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (!IsCollected)
            {
                PlayerCharacter pc = collision.gameObject.GetComponent<PlayerCharacter>();
                if (pc != null)
                {
                    Collect(pc);
                }
            }
        }

        /// <summary>
        /// Gives score and destroys the pickup.
        /// </summary>
        /// <param name="character">A player character</param>
        public virtual void Collect(PlayerCharacter character)
        {
            GameManager.Instance.UpdateScore(_score);
            IsCollected = true;

            if (_expansion != null)
            {
                _expansion.OnPickupCollected();
            }

            DestroyObject();
        }

        /// <summary>
        /// Destroys the pickup.
        /// </summary>
        public override void DestroyObject()
        {
            gameObject.SetActive(false);
            base.DestroyObject();
        }

        /// <summary>
        /// Resets the pickup.
        /// </summary>
        public override void ResetObject()
        {
            IsCollected = false;
            SetToDefaultPosition();
            gameObject.SetActive(true);
            base.ResetObject();
        }
    }
}
