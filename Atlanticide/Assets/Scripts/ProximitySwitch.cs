using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class ProximitySwitch : MonoBehaviour
    {
        [SerializeField]
        private float _range;

        [SerializeField]
        private bool _allPlayersNeeded;

        [SerializeField]
        private bool _permanent;

        public bool Activated { get; private set; }

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
        private void OnDrawGizmos()
        {
            Gizmos.color = (Activated ? Color.green : Color.black);
            Gizmos.DrawWireSphere(transform.position, _range);
        }
    }
}
