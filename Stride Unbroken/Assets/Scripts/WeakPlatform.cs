using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrideUnbroken
{
    public class WeakPlatform : Platform
    {
        [SerializeField]
        private int _hitPoints;

        private int _hitPointsLeft;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected override void ResetValues()
        {
            base.ResetValues();
            _hitPointsLeft = _hitPoints;
        }

        /// <summary>
        /// Handles logic when the platform is bounced on.
        /// </summary>
        public override void BouncedOn()
        {
            base.BouncedOn();

            _hitPointsLeft--;
            if (_hitPointsLeft == 0)
            {
                Break();
            }
        }

        /// <summary>
        /// Sets default values when the object is reset in the editor.
        /// </summary>
        private void Reset()
        {
            _hitPoints = 3;
        }
    }
}
