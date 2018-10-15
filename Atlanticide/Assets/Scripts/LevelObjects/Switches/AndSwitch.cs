using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class AndSwitch : Switch
    {
        [SerializeField]
        private List<Switch> _switches;

        /// <summary>
        /// Updates the object.
        /// </summary>
        protected override void UpdateObject()
        {
            if (!Activated || !_permanent)
            {
                CheckSwitches();
            }

            base.UpdateObject();
        }

        /// <summary>
        /// Checks if all of the attached switches are active
        /// and updates this switch's activation.
        /// </summary>
        private void CheckSwitches()
        {
            bool result = true;
            foreach (Switch s in _switches)
            {
                if (!s.Activated)
                {
                    result = false;
                    break;
                }
            }

            Activated = result;
        }
    }
}
