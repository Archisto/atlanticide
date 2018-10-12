using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class AbilityEnergy
    {
        private float _drainSpeed;
        private float _rechargeSpeed;
        private float _minRecharge;
        private bool _startEmpty;

        public float Energy { get; private set; }

        public bool Unusable { get; private set; }

        /// <summary>
        /// Creates the object.
        /// </summary>
        private AbilityEnergy(float drainSpeed,
            float rechargeSpeed, float minRecharge,
            bool startEmpty)
        {
            _drainSpeed = drainSpeed;
            _rechargeSpeed = rechargeSpeed;
            _minRecharge = minRecharge;
            _startEmpty = startEmpty;
            ResetAbilityEnergy();
        }

        private void DrainOrRecharge(bool drain)
        {
            // Drain
            if (drain)
            {
                Unusable = false;
                Energy -= _drainSpeed * World.Instance.DeltaTime;
                if (Energy <= 0f)
                {
                    SetEnergyEmpty();
                }
            }
            // Recharge
            else if (Energy < 1f)
            {
                Energy += _rechargeSpeed * World.Instance.DeltaTime;

                if (Unusable && Energy >= _minRecharge)
                {
                    Unusable = false;
                }
                if (Energy >= 1f)
                {
                    SetEnergyFull();
                }
            }
        }

        public void SetEnergyFull()
        {
            Energy = 1f;
            Unusable = false;
        }

        public void SetEnergyEmpty()
        {
            Energy = 0f;
            Unusable = true;
        }

        public void ResetAbilityEnergy()
        {
            Energy = (_startEmpty ? 0f : 1f);
        }
    }
}
