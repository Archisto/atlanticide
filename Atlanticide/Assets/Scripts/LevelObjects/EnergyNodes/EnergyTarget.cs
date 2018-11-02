using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class EnergyTarget : EnergyNode
    {
        public override bool IsValidEnergySource()
        {
            return false;
        }

        public override bool IsValidEnergyTarget()
        {
            return _unlimitedCapacity || !MaxCharge;
        }

        public override bool GainCharge()
        {
            if (_unlimitedCapacity)
            {
                if (Usable)
                {
                    Activate(true);
                    currentCharges++;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return base.GainCharge();
            }
        }

        public override int LoseCharge()
        {
            return 0;
        }

        protected override void OnDrawGizmos()
        {
            if (_unlimitedCapacity)
            {
                DrawRangeGizmos();
                Utils.DrawDotGizmos(transform.position + Vector3.up * 3f,
                    currentCharges, currentCharges, Color.yellow);
            }
            else
            {
                base.OnDrawGizmos();
            }
        }
    }
}
