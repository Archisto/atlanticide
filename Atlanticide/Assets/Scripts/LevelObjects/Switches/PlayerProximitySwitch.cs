using UnityEngine;

namespace Atlanticide
{
    public class PlayerProximitySwitch : Switch
    {
        [SerializeField]
        private float _range = 3f;

        [SerializeField]
        private bool _allPlayersNeeded;

        public float Range
        {
            get { return _range; }
            set { _range = value; }
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        protected override void UpdateObject()
        {
            if (!Activated || !_permanent)
            {
                CheckPlayerProximity();
            }

            base.UpdateObject();
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
            if (_drawGizmos)
            {
                base.OnDrawGizmos();
                Gizmos.DrawWireSphere(transform.position, _range);
            }
        }
    }
}
