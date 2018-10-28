using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class Activatable : LevelObject
    {
        [Header("ACTIVATION TYPE")]

        [SerializeField]
        private bool _usingKey;

        [SerializeField]
        private int _keyCode;

        [SerializeField]
        Switch _switch;

        [SerializeField]
        EnergyTarget _energyTarget;

        [SerializeField]
        private bool _needsMaxCharge;

        public bool Activated { get; private set; }

        /// <summary>
        /// Updates the object.
        /// </summary>
        protected override void UpdateObject() 
        {
            if (_usingKey)
            {
                Activated = CheckKey();
            }
            else if (_switch != null)
            {
                Activated = _switch.Activated;
            }
            else if (_energyTarget != null)
            {
                if (_energyTarget.Usable)
                {
                    Activated = (_needsMaxCharge ?
                        _energyTarget.MaxCharge : _energyTarget.ZeroCharge);
                }
            }

            base.UpdateObject();
        }

        /// <summary>
        /// Iterates through each owned key code and if a
        /// matching key code is found, returns true.
        /// </summary>
        /// <returns>Is the key code owned</returns>
        private bool CheckKey()
        {
            foreach (int ownedKeyCode in World.Instance.keyCodes)
            {
                if (ownedKeyCode == _keyCode)
                {
                    return true;
                }
            }

            return false;
        }

        public override void ResetObject()
        {
            Activated = false;
            base.ResetObject();
        }
    }
}
