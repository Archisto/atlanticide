using System;
using UnityEngine;

namespace Atlanticide
{
    public class LinkSwitch : Switch, ILinkInteractable
    {
        [SerializeField, Range(0f, 30f)]
        protected float _toughness;

        [SerializeField]
        private float _hitInterval = 0.3f;

        [Header("AUDIO")]

        [SerializeField]
        protected bool _playSoundWhenActivated = true;

        private bool _hasPlayedActivateSound;

        [SerializeField]
        protected bool _playSoundWhenDeactivated = true;

        [SerializeField]
        protected Sound _activateSound = Sound.Door_Open;

        [SerializeField]
        protected Sound _deactivateSound = Sound.Door_Close;

        [SerializeField]
        protected float _volumeFactor = 1f;

        protected Timer _hitTimer;

        protected override void Start()
        {
            base.Start();
            _hitTimer = new Timer(_hitInterval, true);
        }

        protected override void UpdateObject()
        {
            if (_hitTimer.Check())
            {
                _hitTimer.Reset();
                if (Activated && !_permanent)
                {
                    Activated = false;
                }
            }

            base.UpdateObject();
        }

        public virtual bool TryInteract(LinkBeam linkBeam)
        {
            if (linkBeam.Strength >= _toughness)
            {
                if ((!Activated || !_permanent) &&
                    !_hitTimer.Active)
                {
                    Activated = true;
                    _hitTimer.Activate();

                    if (_playSoundWhenActivated)
                    {
                        if (!_hasPlayedActivateSound)
                        {
                            SFXPlayer.Instance.Play(_activateSound, volumeFactor: _volumeFactor);

                            _hasPlayedActivateSound = true;
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        public virtual bool TryInteractInstant(LinkBeam linkBeam)
        {
            return TryInteract(linkBeam);
        }

        public virtual bool GivePulse(LinkBeam linkBeam, float speedModifier)
        {
            return false;
        }

        public override void ResetObject()
        {
            _hitTimer.Reset();
            _hasPlayedActivateSound = false;
            base.ResetObject();
        }

        /// <summary>
        /// Draws gizmos.
        /// </summary>
        protected override void OnDrawGizmos()
        {
            if (_drawGizmos)
            {
                base.OnDrawGizmos();
                Gizmos.DrawSphere(transform.position + Vector3.up * 2f, 0.5f);
            }
        }
    }
}
