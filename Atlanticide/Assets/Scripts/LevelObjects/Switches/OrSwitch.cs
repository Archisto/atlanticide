using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class OrSwitch : Switch
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
        /// Checks if any of the attached switches are active
        /// and updates this switch's activation.
        /// </summary>
        private void CheckSwitches()
        {
            bool result = false;
            foreach (Switch s in _switches)
            {
                if (s.Activated)
                {
                    result = true;
                    break;
                }
            }

            Activated = result;
        }
    }
}
