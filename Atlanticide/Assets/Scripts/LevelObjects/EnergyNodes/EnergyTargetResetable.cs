using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class EnergyTargetResetable : EnergyTarget
    {
        /// <summary>
        /// Updates the object.
        /// </summary>
        protected override void UpdateObject()
        {
            base.UpdateObject();

            if (currentCharges > 0 && !World.Instance.EmittingEnergy)
            {
                SetEnergyChargesToZero();
            }
        }
    }
}
