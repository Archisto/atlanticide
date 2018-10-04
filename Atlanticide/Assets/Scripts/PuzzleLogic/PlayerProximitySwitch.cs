﻿using UnityEngine;

namespace Atlanticide
{
    public class PlayerProximitySwitch : Switch
    {
        [SerializeField]
        private float _range = 3f;

        [SerializeField]
        private bool _allPlayersNeeded;

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (!Activated || !_permanent)
            {
                CheckPlayerProximity();
            }
        }

        /// <summary>
        /// Checks if one or all players are within range
        /// and updates the switch's activation.
        /// </summary>
        private void CheckPlayerProximity()
        {
            Activated = GameManager.Instance.PlayersAreWithinRange
                (transform.position, _range, _allPlayersNeeded);
        }

        /// <summary>
        /// Draws gizmos.
        /// </summary>
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            Gizmos.DrawWireSphere(transform.position, _range);
        }
    }
}
