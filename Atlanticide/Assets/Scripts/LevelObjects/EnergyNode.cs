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
        protected PlayerCharacter _energyCollectorPlayer;
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
        /// Updates the object.
        /// </summary>
        protected override void UpdateObject()
        {
            if (Active)
            {
                UpdateEnergyCollectorPlayer();
                UpdateEnergyCollectorTarget();
            }
            else
            {
                UpdateActiveState();
            }

            base.UpdateObject();
        }

        protected void UpdateActiveState()
        {
            if (_keyCodeSwitch != null)
            {
                Active = _keyCodeSwitch.Activated;
            }
        }

        public void UpdateEnergyCollectorPlayer()
        {
            if (_energyCollectorPlayer == null ||
                _energyCollectorPlayer.Tool != PlayerTool.EnergyCollector)
            {
                _energyCollectorPlayer =
                    GameManager.Instance.GetPlayerWithTool(PlayerTool.EnergyCollector, true);
            }
        }

        protected void UpdateEnergyCollectorTarget()
        {
            if (EnergyCollectorPlayerIsAvailable() &&
                _energyCollectorPlayer.EnergyCollector.Target != this)
            {
                float distance = Vector3.Distance
                    (transform.position, _energyCollectorPlayer.EnergyCollector.transform.position);
                if (distance <= World.Instance.energyCollectRadius)
                {
                    _energyCollectorPlayer.EnergyCollector.Target = this;
                }
            }
        }

        public bool EnergyCollectorPlayerIsAvailable()
        {
            return (_energyCollectorPlayer != null && !_energyCollectorPlayer.IsDead);
        }

        private void RemoveEnergyCollectorPlayer()
        {
            if (_energyCollectorPlayer.EnergyCollector.Target == this)
            {
                _energyCollectorPlayer.EnergyCollector.Target = null;
            }

            _energyCollectorPlayer = null;
        }

        //public void SetEnergyCollectorPlayer(PlayerCharacter player)
        //{
        //    if (player == null)
        //    {
        //        if (_energyCollectorPlayer != null)
        //        {
        //            _energyCollectorPlayer.UpdateClosestEnergyNode(null);
        //            _energyCollectorPlayer = null;
        //        }
        //    }
        //    else if (player.Tool == PlayerTool.EnergyCollector)
        //    {
        //        if (_energyCollectorPlayer != null)
        //        {
        //            _energyCollectorPlayer.UpdateClosestEnergyNode(null);
        //        }

        //        _energyCollectorPlayer = player;
        //    }
        //}

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
                RemoveEnergyCollectorPlayer();
            }
        }

        public override void ResetObject()
        {
            Active = _activeByDefault;
            currentCharges = _startCharges;
            base.ResetObject();
        }

        /// <summary>
        /// Draws gizmos.
        /// </summary>
        protected virtual void OnDrawGizmos()
        {
            if (Active)
            {
                Gizmos.color = Color.black;
                if (EnergyCollectorPlayerIsAvailable() &&
                    _energyCollectorPlayer.EnergyCollector.Target == this)
                {
                    Gizmos.color = Color.yellow;
                }

                Gizmos.DrawWireSphere(transform.position, World.Instance.energyCollectRadius);

                Gizmos.color = (MaxCharge ? Color.green : Color.black);
                Utils.DrawProgressBarGizmo(transform.position + Vector3.back * 1f,
                    ((float) currentCharges / _maxCharges), Gizmos.color, Color.yellow);
            }
        }
    }
}
