using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class LinkMovable : LevelObject, ILinkInteractable
    {
        public bool available = true;

        [SerializeField, Range(1f, 30f)]
        private float _toughness = 1f;

        [SerializeField]
        private float _hitInterval = 0.3f;

        [SerializeField]
        protected float _speed = 1f;

        [SerializeField]
        protected float _accuracy = 0.1f;

        [SerializeField]
        protected bool _lockAxisY;

        [Header("AUDIO")]

        [SerializeField]
        protected bool _playSoundWhenAttachedToBeam = true;

        [SerializeField]
        protected bool _playSoundWhenDetachedFromBeam = true;

        [SerializeField]
        protected Sound _attachSound = Sound.Taking_Energy;

        [SerializeField]
        protected Sound _detachSound = Sound.Using_Energy;

        [SerializeField]
        protected float _volumeFactor = 1f;

        protected Destructible _destructible;
        protected Timer _hitTimer;
        protected float _toughnessLeft;
        private bool _beamCenterReached;
        private bool _availableByDefault;
        private float _defaultSpeed;

        public bool AttachedToBeam { get; private set; }

        public LinkBeam TargetLinkBeam { get; set; }

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
        protected virtual void Start()
        {
            _toughnessLeft = _toughness;
            _hitTimer = new Timer(_hitInterval, true);
            _destructible = GetComponent<Destructible>();
            _defaultPosition = transform.position;
            _availableByDefault = available;
            _defaultSpeed = _speed;
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        protected override void UpdateObject()
        {
            if (!available)
            {
                return;
            }

            if (AttachedToBeam)
            {
                if (!TargetLinkBeam.Active)
                {
                    AttachToBeam(false);
                }
                else if (_beamCenterReached)
                {
                    UpdatePositionAtBeamCenter();
                }
                else
                {
                    SeekBeamCenter();
                }
            }
            else if (_hitTimer.Active)
            {
                if (_hitTimer.Check())
                {
                    _hitTimer.Reset();
                }
            }

            CheckDestructible();
            base.UpdateObject();
        }

        private void UpdatePositionAtBeamCenter()
        {
            transform.position = TargetLinkBeam.BeamCenter;
        }

        private void SeekBeamCenter()
        {
            Vector3 newPosition = TargetLinkBeam.BeamCenter +
                    Vector3.Project(transform.position - TargetLinkBeam.BeamCenter,
                    (TargetLinkBeam.BeamCenter - TargetLinkBeam.StartPosition));

            Vector3 direction = (TargetLinkBeam.BeamCenter - newPosition).normalized;
            newPosition += direction * _speed * World.Instance.DeltaTime;

            if (_lockAxisY)
            {
                newPosition.y = transform.position.y;
            }
            transform.position = newPosition;

            if (Vector3.Distance(transform.position, TargetLinkBeam.BeamCenter)
                < _accuracy)
            {
                ReachBeamCenter(true);
            }
        }

        protected virtual void AttachToBeam(bool attach, LinkBeam linkBeam = null)
        {
            if (AttachedToBeam != attach)
            {
                if (!attach && TargetLinkBeam != null)
                {
                    TargetLinkBeam.linkInteractables.Remove(this);
                    FallToGround();
                }

                AttachedToBeam = attach;
                TargetLinkBeam = linkBeam;
                _toughnessLeft = _toughness;
                _speed = _defaultSpeed;
                _hitTimer.Reset();

                if (attach)
                {
                    linkBeam.linkInteractables.Add(this);

                    if (_playSoundWhenAttachedToBeam)
                    {
                        SFXPlayer.Instance.Play(_attachSound, volumeFactor: _volumeFactor);
                    }
                }
                else if (_playSoundWhenDetachedFromBeam)
                {
                    SFXPlayer.Instance.Play(_detachSound, volumeFactor: _volumeFactor);
                }
            }
        }

        protected void ReachBeamCenter(bool reach)
        {
            if (reach)
            {
                Debug.Log(name + " reached its target");
            }

            _beamCenterReached = reach;
        }

        private void FallToGround()
        {
            // TODO
            available = false;
            _destructible.DestroyObject();
        }

        private void CheckDestructible()
        {
            if (_destructible.Destroyed)
            {
                AttachToBeam(false);
                available = false;
            }
        }

        public virtual bool TryInteract(LinkBeam linkBeam)
        {
            if (available && !AttachedToBeam && !_hitTimer.Active)
            {
                _toughnessLeft -= linkBeam.Strength;
                if (_toughnessLeft <= 0f)
                {
                    _toughnessLeft = 0f;
                    AttachToBeam(true, linkBeam);
                    return true;
                }
                else
                {
                    _hitTimer.Activate();
                }
            }

            return false;
        }

        public virtual bool TryInteractInstant(LinkBeam linkBeam)
        {
            if (linkBeam.Strength >= _toughness)
            {
                _toughnessLeft = 0f;
                AttachToBeam(true, linkBeam);
                return true;
            }

            return false;
        }

        public bool GivePulse(LinkBeam linkBeam, float speedModifier)
        {
            return false;
        }

        public override void ResetObject()
        {
            AttachToBeam(false);
            ReachBeamCenter(false);
            _toughnessLeft = _toughness;
            _hitTimer.Reset();
            available = _availableByDefault;
            _speed = _defaultSpeed;
            SetToDefaultPosition();
            base.ResetObject();
        }

        private void OnDrawGizmos()
        {
            if (available && !AttachedToBeam)
            {
                Color color = (MaxToughness ? Color.green : Color.yellow);
                Utils.DrawProgressBarGizmo(transform.position + Vector3.back * 1f,
                    ToughnessRatio, color, Color.black);
            }
            else if (AttachedToBeam)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, TargetLinkBeam.BeamCenter);
            }

            Gizmos.color = (AttachedToBeam ? Color.blue :
                (available ? Color.white : Color.grey));
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 1.5f, _accuracy);
        }
    }
}
