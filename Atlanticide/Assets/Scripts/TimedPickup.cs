using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// A pickup that vanishes if it hasn't been collected in time.
    /// </summary>
    public class TimedPickup : Pickup
    {
        [SerializeField]
        private float _lifeTime = 10f;

        private float _elapsedTime;

        /// <summary>
        /// Resets the pickup.
        /// </summary>
        public override void ResetPickup()
        {
            base.ResetPickup();
            _elapsedTime = 0;
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime >= _lifeTime)
            {
                Destroy();
            }
        }

        /// <summary>
        /// Draws gizmos.
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + Vector3.up * 1.5f, 0.5f * (1 - (_elapsedTime / _lifeTime)));
        }
    }
}
