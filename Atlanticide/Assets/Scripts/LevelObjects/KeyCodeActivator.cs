using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class KeyCodeActivator : SwitchExpansion
    {
        public int keyCode;

        [SerializeField]
        private bool _permanent;

        [SerializeField]
        private bool _allowDuplicateKeyCodes;

        protected override bool CanCheckSwitch()
        {
            return !_activated || !_permanent;
        }

        protected override void Activate()
        {
            _activated = World.Instance.
                TryActivateNewKeyCode(keyCode, _allowDuplicateKeyCodes);
        }

        protected override void Deactivate()
        {
            World.Instance.DeactivateKeyCode(keyCode);
            _activated = false;
        }

        /// <summary>
        /// Resets the object to its default state.
        /// </summary>
        public override void OnObjectReset()
        {
            _activated = false;
        }
    }
}
