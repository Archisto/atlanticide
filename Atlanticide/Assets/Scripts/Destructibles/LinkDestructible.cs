using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public enum ToughnessRegenMode
    {
        Off = 0,
        Always = 1,
        NotDestroyed = 2
    }

    /// <summary>
    /// An object that can be destroyed with the link beam.
    /// </summary>
    public class LinkDestructible : LevelObject, ILinkInteractable
    {
        [SerializeField, Range(1f, 30f)]
        protected float _toughness = 10f;

        [SerializeField]
        protected float _hitInterval = 0.3f;

        [Header("TOUGHNESS REGEN")]

        [SerializeField]
        protected ToughnessRegenMode _regenMode;

        [SerializeField]
        protected float _regenAmount = 1f;

        [SerializeField]
        protected float _regenInterval = 0.3f;

        protected Timer _hitTimer;
        protected Timer _regenTimer;
        protected float _toughnessLeft;

        public float ToughnessRatio
        {
            get { return _toughnessLeft / _toughness; }
        }

        public bool MaxToughness
        {
            get { return _toughnessLeft == _toughness; }
        }

        public bool IsDestroyed { get; protected set; }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected virtual void Start()
        {
            _toughnessLeft = _toughness;
            _hitTimer = new Timer(_hitInterval, true);
            _regenTimer = new Timer(_regenInterval, true);
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
            else if (_toughnessLeft < _toughness &&
                _regenMode != ToughnessRegenMode.Off)
            {
                // TODO: Make it possible to be repaired
                // (Update() is not called when the GO is inactive)
                RegenToughness();
            }

            base.UpdateObject();
        }

        public virtual bool TryInteract(LinkBeam linkBeam)
        {
            if (!IsDestroyed && !_hitTimer.Active)
            {
                //Debug.Log("collectStrength: " + collectStrength);
                _toughnessLeft -= linkBeam.Strength;
                if (_toughnessLeft <= 0f)
                {
                    Destroy();
                    return true;
                }
                else
                {
                    _hitTimer.Activate();

                    if (_regenMode != ToughnessRegenMode.Off)
                    {
                        _regenTimer.Activate();
                    }
                }
            }

            return false;
        }

        public virtual bool TryInteractInstant(LinkBeam linkBeam)
        {
            if (!IsDestroyed && linkBeam.Strength >= _toughness)
            {
                Destroy();
                return true;
            }

            return false;
        }

        public virtual bool GivePulse(LinkBeam linkBeam, float speedModifier)
        {
            return false;
        }

        /// <summary>
        /// Regenerates toughness periodically.
        /// Repairs the object if ToughnessRegenMode is set to Always.
        /// Returns whether the maximum toughness has just been reached.
        /// </summary>
        /// <returns>Has the maximum toughness just been reached</returns>
        protected bool RegenToughness()
        {
            if (_toughnessLeft > 0f ||
                _regenMode == ToughnessRegenMode.Always)
            {
                if (!_regenTimer.Active || _regenTimer.Check())
                {
                    if (IsDestroyed)
                    {
                        Repair(_regenAmount);
                    }
                    else
                    {
                        _toughnessLeft += _regenAmount;
                    }

                    if (_toughnessLeft >= _toughness)
                    {
                        _toughnessLeft = _toughness;
                        _regenTimer.Reset();
                        return true;
                    }
                    else
                    {
                        _regenTimer.Activate();
                    }
                }
            }

            return false;
        }

        public virtual void Destroy()
        {
            IsDestroyed = true;
            _toughnessLeft = 0f;
            gameObject.SetActive(false);
            SFXPlayer.Instance.Play(Sound.Cyclops_Exploding, volumeFactor: 0.3f);
        }

        public virtual void Repair(float toughnessLeft)
        {
            if (IsDestroyed)
            {
                IsDestroyed = false;
                gameObject.SetActive(true);
                _toughnessLeft = toughnessLeft;
            }
        }

        /// <summary>
        /// Resets the pickup.
        /// </summary>
        public override void ResetObject()
        {
            _toughnessLeft = _toughness;
            _hitTimer.Reset();
            _regenTimer.Reset();
            gameObject.SetActive(true);
            base.ResetObject();
        }

        /// <summary>
        /// Draws gizmos.
        /// </summary>
        protected virtual void OnDrawGizmos()
        {
            if (!IsDestroyed)
            {
                Color color = (MaxToughness ? Color.green : Color.yellow);
                Utils.DrawProgressBarGizmo(transform.position + Vector3.back * 1f,
                    ToughnessRatio, color, Color.black);
            }
        }
    }
}
