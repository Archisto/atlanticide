﻿using UnityEngine;

namespace Atlanticide
{
    public class KeyCodeSwitch : Switch
    {
        [SerializeField]
        private int _keyCode;

        /// <summary>
        /// Updates the object.
        /// </summary>
        protected override void UpdateObject()
        {
            if (!Activated || !_permanent)
            {
                Activated = CheckKey();
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

        /// <summary>
        /// Draws gizmos.
        /// </summary>
        protected override void OnDrawGizmos()
        {
            if (_drawGizmos)
            {
                base.OnDrawGizmos();
                Gizmos.DrawSphere(transform.position, 0.5f);
            }
        }
    }
}
