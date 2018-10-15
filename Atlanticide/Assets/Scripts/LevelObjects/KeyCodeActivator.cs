using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    [RequireComponent(typeof(Switch))]
    public class KeyCodeActivator : LevelObjectExpansion
    {
        public int keyCode;

        [SerializeField]
        private bool _permanent;

        [SerializeField]
        private bool _allowDuplicateKeyCodes;

        private Switch _switch;
        private bool _activated;
        private bool _keyCodeActive;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            _switch = GetComponent<Switch>();
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        public override void OnObjectUpdated()
        {
            if (!_keyCodeActive || !_permanent)
            {
                _activated = _switch.Activated;
                if (_activated && !_keyCodeActive)
                {
                    _keyCodeActive = World.Instance.
                        TryActivateNewKeyCode(keyCode, _allowDuplicateKeyCodes);
                }
                else if (!_activated && _keyCodeActive)
                {
                    World.Instance.DeactivateKeyCode(keyCode);
                    _keyCodeActive = false;
                }
            }
        }

        public override void OnObjectReset()
        {
            _keyCodeActive = false;
        }
    }
}
