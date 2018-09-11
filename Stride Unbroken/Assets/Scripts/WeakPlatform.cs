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

        /// <summary>
        /// Draws gizmos.
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            float dotRadius = 0.2f;
            float dotSpacing = 0.1f;
            Vector3 position = transform.position;
            position.y += dotRadius + 0.2f;

            for (int i = 0; i < _hitPointsLeft; i++)
            {
                position.x = transform.position.x + i * (2 * dotRadius + dotSpacing);
                Gizmos.DrawSphere(position, dotRadius);
            }
        }
    }
}
