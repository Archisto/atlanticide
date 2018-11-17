using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class LinkPlayerSeeker : LevelObject, ILinkInteractable
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

        protected Timer _hitTimer;
        protected float _toughnessLeft;
        protected Vector3 _targetPosition;
        protected bool _moveForward;
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
            _defaultPosition = transform.position;
            _availableByDefault = available;
            _defaultSpeed = _speed;
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        protected override void UpdateObject()
        {
            if (AttachedToBeam)
            {
                if (!TargetLinkBeam.Active)
                {
                    AttachToBeam(false);
                    return;
                }

                _targetPosition = (_moveForward ?
                    TargetLinkBeam.TargetPosition : TargetLinkBeam.StartPosition);
                Vector3 oppositeEndPosition = (!_moveForward ?
                    TargetLinkBeam.TargetPosition : TargetLinkBeam.StartPosition);

                //Vector3 direction = (_targetPosition - transform.position).normalized;
                //Vector3 newPosition = transform.position + direction * World.Instance.DeltaTime;
                Vector3 newPosition = TargetLinkBeam.BeamCenter +
                    Vector3.Project(transform.position - TargetLinkBeam.BeamCenter,
                    (_targetPosition - TargetLinkBeam.BeamCenter));

                Vector3 direction = (_targetPosition - newPosition).normalized;
                newPosition += direction * _speed * World.Instance.DeltaTime;

                if (_lockAxisY)
                {
                    newPosition.y = transform.position.y;
                }
                transform.position = newPosition;

                if (Vector3.Distance(transform.position, _targetPosition) < _accuracy)
                {
                    ReachTarget();
                }
            }
            else if (_hitTimer.Active)
            {
                if (_hitTimer.Check())
                {
                    _hitTimer.Reset();
                }
            }

            base.UpdateObject();
        }

        protected virtual void AttachToBeam(bool attach, LinkBeam linkBeam = null)
        {
            if (AttachedToBeam != attach)
            {
                //if (attach)
                //{
                //    Debug.Log("Attached to " + linkBeam.Player.name + "'s beam");
                //}
                //else
                //{
                //    Debug.Log("Detached from " + TargetLinkBeam.Player.name + "'s beam");
                //}

                if (!attach && TargetLinkBeam != null)
                {
                    TargetLinkBeam.linkInteractables.Remove(this);
                }

                AttachedToBeam = attach;
                TargetLinkBeam = linkBeam;
                _toughnessLeft = _toughness;
                _speed = _defaultSpeed;
                _moveForward = true; // TODO
                _hitTimer.Reset();

                if (attach)
                {
                    linkBeam.linkInteractables.Add(this);
                }
            }
        }

        protected virtual void ReachTarget()
        {
            //Debug.Log(name + " reached its target");
            AttachToBeam(false);
            available = false;
        }

        public void SetDirection(bool towardsTarget)
        {
            _moveForward = towardsTarget;
        }

        public void ChangeDirection()
        {
            _moveForward = !_moveForward;
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
            _speed = Mathf.Abs(_defaultSpeed * speedModifier);
            SetDirection(speedModifier > 0f);
            return true;
        }

        public override void ResetObject()
        {
            AttachToBeam(false);
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
                Gizmos.DrawLine(transform.position, _targetPosition);
                Gizmos.DrawLine(transform.position, TargetLinkBeam.BeamCenter);
            }

            Gizmos.color = (AttachedToBeam ? Color.blue :
                (available ? Color.white : Color.grey));
            Gizmos.DrawWireSphere(transform.position, _accuracy);
        }
    }
}
