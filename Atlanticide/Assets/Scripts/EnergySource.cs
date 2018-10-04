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
            _startCharges = _maxCharges;
            base.Start();
        }

        public override bool MaxCharge
        {
            get { return true; }
        }

        public override bool GainCharge()
        {
            return false;
        }

        public override bool LoseCharge()
        {
            if (Active)
            {
                return true;
            }

            return false;
        }
    }
}
