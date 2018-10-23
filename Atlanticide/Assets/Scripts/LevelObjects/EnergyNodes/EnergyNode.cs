using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class EnergyNode : LevelObject
    {
        [SerializeField]
        private bool _active = true;

        public int currentCharges;

        [SerializeField, Range(1, 20)]
        protected int _maxCharges = 1;

        [SerializeField]
        protected int _defaultCharges = 0;

        [Header("RANGE BOX")]

        [SerializeField]
        private bool _useRangeBox;

        [SerializeField]
        private Vector3 _boxCorner1 = Vector3.one * -1;

        [SerializeField]
        private Vector3 _boxCorner2 = Vector3.one;

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
            get {
                Debug.Log("zeroCharge: " + currentCharges);
                return currentCharges == 0; }
        }

        public bool HasJustBeenReset { get; protected set; }

        private Vector3 BoxCorner1 { get; set; }

        private Vector3 BoxCorner2 { get; set; }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected virtual void Start()
        {
            if (_defaultCharges > _maxCharges)
            {
                _defaultCharges = _maxCharges;
            }

            currentCharges = _defaultCharges;
            _activeByDefault = _active;
            Active = _active;
            _keyCodeSwitch = GetComponent<KeyCodeSwitch>();

            // Gives BoxCorner1 the smaller axis values
            // and BoxCorner2 the larger ones
            if (_useRangeBox)
            {
                float minX = Mathf.Min(_boxCorner1.x, _boxCorner2.x);
                float maxX = Mathf.Max(_boxCorner1.x, _boxCorner2.x);
                float minY = Mathf.Min(_boxCorner1.y, _boxCorner2.y);
                float maxY = Mathf.Max(_boxCorner1.y, _boxCorner2.y);
                float minZ = Mathf.Min(_boxCorner1.z, _boxCorner2.z);
                float maxZ = Mathf.Max(_boxCorner1.z, _boxCorner2.z);
                BoxCorner1 = transform.position + new Vector3(minX, minY, minZ);
                BoxCorner2 = transform.position + new Vector3(maxX, maxY, maxZ);
            }
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

            HasJustBeenReset = false;

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
                _energyCollectorPlayer = GameManager.Instance.
                    GetPlayerWithTool(PlayerTool.EnergyCollector, true);
            }
        }

        protected void UpdateEnergyCollectorTarget()
        {
            if (EnergyCollectorPlayerIsAvailable() &&
                _energyCollectorPlayer.EnergyCollector.Target != this)
            {
                if (PositionWithinRange(_energyCollectorPlayer.
                        EnergyCollector.transform.position))
                {
                    _energyCollectorPlayer.EnergyCollector.Target = this;
                }
            }
        }

        public bool PositionWithinRange(Vector3 position)
        {
            if (_useRangeBox)
            {
                return Utils.WithinRangeBox(position, BoxCorner1, BoxCorner2);
            }
            else
            {
                float distance = Vector3.Distance(transform.position, position);
                return distance <= World.Instance.energyCollectRadius;
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

        public void SetEnergyChargesToZero()
        {
            currentCharges = 0;
            HasJustBeenReset = true;
        }

        public override void ResetObject()
        {
            Active = _activeByDefault;
            currentCharges = _defaultCharges;
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

                if (_useRangeBox)
                {
                    if (!Application.isPlaying)
                    {
                        BoxCorner1 = transform.position + _boxCorner1;
                        BoxCorner2 = transform.position + _boxCorner2;
                    }

                    Utils.DrawBoxGizmo(BoxCorner1, BoxCorner2);
                }
                else
                {
                    Gizmos.DrawWireSphere
                        (transform.position, World.Instance.energyCollectRadius);
                }
                
                Gizmos.color = (MaxCharge ? Color.green : Color.black);
                Utils.DrawProgressBarGizmo(transform.position + Vector3.back * 1f,
                    ((float) currentCharges / _maxCharges), Gizmos.color, Color.yellow);
            }
        }
    }
}
