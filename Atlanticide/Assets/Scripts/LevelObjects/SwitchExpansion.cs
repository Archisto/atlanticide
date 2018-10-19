using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// An expansion to a switch.
    /// </summary>
    [RequireComponent(typeof(Switch))]
    public abstract class SwitchExpansion : LevelObjectExpansion
    {
        protected Switch _switch;
        protected bool _activated;
        protected bool _switchActivated;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            _switch = GetComponent<Switch>();
        }

        /// <summary>
        /// Returns whether it is possible to check the
        /// switch and handle the switch expansion's logic.
        /// Checks the switch only if the switch
        /// expansion hasn't been activated.
        /// </summary>
        /// <returns>Bool</returns>
        protected virtual bool CanCheckSwitch()
        {
            return !_activated;
        }

        /// <summary>
        /// Checks the switch and handles the
        /// switch expansion's logic accordingly.
        /// </summary>
        public override void OnObjectUpdated()
        {
            if (CanCheckSwitch())
            {
                _switchActivated = _switch.Activated;

                if (_switchActivated && !_activated)
                {
                    Activate();
                }
                else if (!_switchActivated && _activated)
                {
                    Deactivate();
                }
            }
        }

        /// <summary>
        /// Activates the switch expansion.
        /// </summary>
        protected abstract void Activate();

        /// <summary>
        /// Deactivates the switch expansion.
        /// </summary>
        protected abstract void Deactivate();

        /// <summary>
        /// Resets the object to its default state.
        /// </summary>
        public override void OnObjectReset()
        {
            Deactivate();
        }
    }
}
