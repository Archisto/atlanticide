using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class EnergyTarget : EnergyNode
    {
        public override bool LoseCharge()
        {
            return false;
        }
    }
}
