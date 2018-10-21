using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// A pickup that takes some time to collect.
    /// The player must stay within range until the time is up.
    /// </summary>
    public class SlowPickup : Pickup
    {
        [SerializeField, Range(0.5f, 30f)]
        private float _pickupTime = 1f;

        [SerializeField]
        private float _pickupMaxDistance = 1f;

        private bool _beingPickedUp;
        private PlayerCharacter _pcPickingUp;
        private float _pcPickingUpDist;
        private float _elapsedTime;

        /// <summary>
        /// Resets the pickup.
        /// </summary>
        public override void ResetObject()
        {
            base.ResetObject();
            _beingPickedUp = false;
            _pcPickingUp = null;
            _pcPickingUpDist = 0;
            _elapsedTime = 0;
        }

        /// <summary>
        /// Handles colliding with the player characters.
        /// </summary>
        /// <param name="collision">The collision</param>
        protected override void OnCollisionEnter(Collision collision)
        {
            if (!_beingPickedUp)
            {
                PlayerCharacter pc = collision.gameObject.GetComponent<PlayerCharacter>();
                if (pc != null)
                {
                    _beingPickedUp = true;
                    _pcPickingUp = pc;
                }
            }
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        protected override void UpdateObject()
        {
            if (_beingPickedUp)
            {
                _pcPickingUpDist = Vector3.Distance(transform.position, _pcPickingUp.transform.position);
                _elapsedTime += World.Instance.DeltaTime;
                if (_elapsedTime >= _pickupTime)
                {
                    Collect(_pcPickingUp);
                }
                else if (_pcPickingUpDist > _pickupMaxDistance)
                {
                    ResetObject();
                }
            }

            base.UpdateObject();
        }

        /// <summary>
        /// Draws gizmos.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (_beingPickedUp)
            {
                Gizmos.color = Color.Lerp(Color.yellow, Color.black, _pcPickingUpDist / _pickupMaxDistance);
                Gizmos.DrawLine(transform.position, _pcPickingUp.transform.position);

                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(transform.position + Vector3.up * 1.5f, 0.5f * (1 - (_elapsedTime / _pickupTime)));
            }
        }
    }
}
