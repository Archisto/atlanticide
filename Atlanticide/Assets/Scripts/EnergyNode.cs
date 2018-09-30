using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class EnergyNode : LevelObject
    {
        public int currentCharges;

        [SerializeField, Range(1, 20)]
        protected int _maxCharges = 1;

        [SerializeField]
        protected int _startCharges = 1;

        [SerializeField]
        private bool _active = true;

        protected KeyCodeSwitch _keyCodeSwitch;
        protected PlayerCharacter _closestPlayer;
        private bool _activeByDefault;

        public bool Active
        {
            get
            {
                return _active;
            }
            protected set
            {
                _active = value;
            }
        }

        public virtual bool MaxCharge
        {
            get { return currentCharges == _maxCharges; }
        }

        public virtual bool ZeroCharge
        {
            get { return currentCharges == 0; }
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected virtual void Start()
        {
            if (_startCharges > _maxCharges)
            {
                _startCharges = _maxCharges;
            }

            currentCharges = _startCharges;
            _activeByDefault = _active;
            Active = _active;
            _keyCodeSwitch = GetComponent<KeyCodeSwitch>();
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        protected virtual void Update()
        {
            if (!World.Instance.GamePaused)
            {
                if (Active)
                {
                    UpdateClosestEnergyCollector();
                }
                else
                {
                    UpdateActiveState();
                }
            }
        }

        protected void UpdateActiveState()
        {
            if (_keyCodeSwitch != null)
            {
                Active = _keyCodeSwitch.Activated;
            }
        }

        protected void UpdateClosestEnergyCollector()
        {
            PlayerCharacter newClosestPlayer = GameManager.Instance.GetClosestAblePlayer(transform.position);

            bool differentPlayer = (newClosestPlayer != _closestPlayer);
            bool newPlayerOK = (newClosestPlayer != null && newClosestPlayer.EnergyCollector != null);

            if (differentPlayer && newPlayerOK)
            {
                float distance = Vector3.Distance
                    (transform.position, newClosestPlayer.EnergyCollector.transform.position);
                if (distance <= World.Instance.energyCollectRadius)
                {
                    if (_closestPlayer != null)
                    {
                        _closestPlayer.UpdateClosestEnergyNode(null);
                    }

                    _closestPlayer = newClosestPlayer;
                    _closestPlayer.UpdateClosestEnergyNode(this);
                }
            }
        }

        public void RemoveClosestPlayer()
        {
            _closestPlayer = null;
        }

        public virtual bool GainCharge()
        {
            if (Active && currentCharges < _maxCharges)
            {
                currentCharges++;
                return true;
            }

            return false;
        }

        public virtual bool LoseCharge()
        {
            if (Active && currentCharges > 0)
            {
                currentCharges--;
                return true;
            }

            return false;
        }

        public virtual void SetActive(bool active)
        {
            Active = active;

            if (!Active)
            {
                RemoveClosestPlayer();
            }
        }

        public override void ResetObject()
        {
            base.ResetObject();
            Active = _activeByDefault;
            currentCharges = _startCharges;
        }

        /// <summary>
        /// Draws gizmos.
        /// </summary>
        protected virtual void OnDrawGizmos()
        {
            if (Active)
            {
                Gizmos.color = (_closestPlayer != null ? Color.yellow : Color.black);
                Gizmos.DrawWireSphere(transform.position, World.Instance.energyCollectRadius);

                Gizmos.color = (MaxCharge ? Color.green : Color.black);
                Utils.DrawProgressBarGizmo(transform.position + Vector3.back * 1f,
                    ((float) currentCharges / _maxCharges), Gizmos.color, Color.yellow);
            }
        }
    }
}
