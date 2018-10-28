using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class EnergySource : EnergyNode
    {
        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected override void Start()
        {
            _defaultCharges = _maxCharges;
            base.Start();
        }

        public override bool IsValidEnergySource()
        {
            return _unlimitedCapacity || !ZeroCharge;
        }

        public override bool IsValidEnergyTarget()
        {
            return false;
        }

        public override bool MaxCharge
        {
            get
            {
                return (_unlimitedCapacity ? true : base.MaxCharge);
            }
        }

        public override bool GainCharge()
        {
            return false;
        }

        public override bool LoseCharge()
        {
            if (_unlimitedCapacity)
            {
                if (Usable)
                {
                    Activate(true);
                }

                return Usable;
            }
            else
            {
                return base.LoseCharge();
            }
        }
    }
}
