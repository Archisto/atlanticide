using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class ScorePlatform : Platform
    {
        [SerializeField]
        private int _scorePerHit;

        [SerializeField]
        private int _scoreHits;

        private int _scoreHitsLeft;
        private bool _active;

        /// <summary>
        /// Resets the platform's values to their defaults.
        /// </summary>
        protected override void ResetValues()
        {
            base.ResetValues();
            _scoreHitsLeft = _scoreHits;
            _active = (_scoreHitsLeft > 0);
        }

        /// <summary>
        /// Handles logic when the platform is bounced on.
        /// </summary>
        public void GiveScore()
        {
            if (_active)
            {
                GameManager.Instance.ChangeScore(_scorePerHit);

                if (_scoreHits > 0)
                {
                    _scoreHitsLeft--;
                    if (_scoreHitsLeft == 0)
                    {
                        _active = false;
                    }
                }
            }
        }

        /// <summary>
        /// Sets default values when the object is reset in the editor.
        /// </summary>
        private void Reset()
        {
            _scorePerHit = 100;
            _scoreHits = 3;
        }

        /// <summary>
        /// Draws gizmos.
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            float dotRadius = 0.2f;
            float dotSpacing = 0.1f;
            Vector3 position = transform.position;
            position.y += dotRadius + 0.2f;

            for (int i = 0; i < _scoreHitsLeft; i++)
            {
                position.x = transform.position.x + i * (2 * dotRadius + dotSpacing);
                Gizmos.DrawSphere(position, dotRadius);
            }
        }
    }
}
