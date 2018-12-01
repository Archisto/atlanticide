using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// A pickup that takes some time or
    /// high enough collect strength to collect.
    /// Can be collected with the link beam.
    /// </summary>
    public class ToughPickup : Pickup, ILinkInteractable
    {
        [SerializeField]
        private bool _canBePickedUpByPlayer = true;

        [SerializeField, Range(1f, 30f)]
        private float _toughness = 1f;

        [SerializeField]
        private float _hitInterval = 0.3f;

        private Timer _hitTimer;
        private float _toughnessLeft;

        public float ToughnessRatio
        {
            get { return _toughnessLeft / _toughness; }
        }

        public bool MaxToughness
        {
            get { return _toughnessLeft == _toughness; }
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected override void Start()
        {
            _hitTimer = new Timer(_hitInterval, true);
            base.Start();
        }

        protected override void UpdateObject()
        {
            if (_hitTimer.Active)
            {
                if (_hitTimer.Check())
                {
                    _hitTimer.Reset();
                }
            }

            base.UpdateObject();
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="collision">The collision</param>
        protected override void OnCollisionEnter(Collision collision)
        {
            // Does nothing
        }

        /// <summary>
        /// Handles colliding with a player character.
        /// </summary>
        /// <param name="collision">The collision</param>
        protected void OnCollisionStay(Collision collision)
        {
            if (_canBePickedUpByPlayer &&
                !IsCollected && !_hitTimer.Active &&
                World.Instance.PlayerInteractStrength > 0f)
            {
                PlayerCharacter pc = collision.gameObject.
                    GetComponentInParent<PlayerCharacter>();
                if (pc != null)
                {
                    TryInteractPlayer(pc);
                }
            }
        }

        public bool TryInteractPlayer(PlayerCharacter player)
        {
            return TryInteract(player, World.Instance.PlayerInteractStrength);
        }

        public bool TryInteract(LinkBeam linkBeam)
        {
            return TryInteract(linkBeam.Player, linkBeam.Strength);
        }

        public bool TryInteract(PlayerCharacter player, float collectStrength)
        {
            if (!IsCollected && !_hitTimer.Active)
            {
                //Debug.Log("collectStrength: " + collectStrength);
                _toughnessLeft -= collectStrength;
                if (_toughnessLeft <= 0f)
                {
                    _toughnessLeft = 0f;
                    Collect(player);
                    return true;
                }
                else
                {
                    _hitTimer.Activate();
                }
            }

            return false;
        }

        public bool TryInteractInstant(LinkBeam linkBeam)
        {
            if (linkBeam.Strength >= _toughness)
            {
                _toughnessLeft = 0f;
                Collect(linkBeam.Player);
                return true;
            }

            return false;
        }

        public bool GivePulse(LinkBeam linkBeam, float speedModifier)
        {
            return false;
        }

        /// <summary>
        /// Resets the pickup.
        /// </summary>
        public override void ResetObject()
        {
            _toughnessLeft = _toughness;
            _hitTimer.Reset();
            base.ResetObject();
        }

        /// <summary>
        /// Draws gizmos.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!IsCollected)
            {
                Color color = (MaxToughness ? Color.green : Color.yellow);
                Utils.DrawProgressBarGizmo(transform.position + Vector3.back * 1f,
                    ToughnessRatio, color, Color.black);
            }
        }
    }
}
