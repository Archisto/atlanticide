using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// An energy target which resets its charges if
    /// the charging stops midway to max charge.
    /// </summary>
    public class EnergyTargetResetable : EnergyTarget
    {
        [Header("RESETTING")]

        [SerializeField]
        private float _timeUntilReset;

        private int _oldCharges;
        private Timer _resetTimer;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            _resetTimer = new Timer(_timeUntilReset, true);
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        protected override void UpdateObject()
        {
            base.UpdateObject();

            // Resets charges if charging stops for a set duration
            if (!_resetTimer.Active)
            {
                if (currentCharges > 0 && !MaxCharge &&
                    !World.Instance.EmittingEnergy)
                {
                    _resetTimer.Activate();
                    _oldCharges = currentCharges;
                }
            }
            else
            {
                if (currentCharges > _oldCharges)
                {
                    _resetTimer.Reset();
                }
                else if (_resetTimer.Check())
                {
                    SetEnergyChargesToZero();
                    _resetTimer.Reset();
                }

                _oldCharges = currentCharges;
            }
        }

        /// <summary>
        /// Resets the object.
        /// </summary>
        public override void ResetObject()
        {
            base.ResetObject();
            _oldCharges = currentCharges;
            _resetTimer.Reset();
        }
    }
}
