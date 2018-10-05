using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class ScoreSwitch : GradualSwitch
    {
        [SerializeField]
        private int _scoreLimit = 100;

        [SerializeField]
        private bool _zeroTarget;

        private float _oldScore;
        private bool _firstCheck;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected override void Start()
        {
            base.Start();

            if (_scoreLimit == 0)
            {
                _zeroTarget = true;
                _scoreLimit = 100;
            }

            _firstCheck = true;
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        protected override void UpdateObject()
        {
            if (!Activated || !_permanent)
            {
                UpdateProgress();
            }

            base.UpdateObject();
        }

        /// <summary>
        /// Updates the switch's progress. The switch is activated
        /// if the score limit (or possibly zero) is reached.
        /// </summary>
        private void UpdateProgress()
        {
            int score = GameManager.Instance.CurrentScore;

            if (score != _oldScore || _firstCheck)
            {
                float progress = (float) score / _scoreLimit;

                if (_zeroTarget)
                {
                    progress = 1 - progress;
                }

                SetProgress(progress);
                _oldScore = score;
                _firstCheck = false;
            }
        }
    }
}
