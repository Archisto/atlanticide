using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class Timer
    {
        public float targetTime;

        private float _elapsedTime;
        private bool _pausable;
        private bool _oneShot;

        public bool Active { get; private set; }
        public bool Finished { get; private set; }

        /// <summary>
        /// Creates the timer.
        /// </summary>
        /// <param name="targetTime">The target time</param>
        /// <param name="pausable">Is the timer paused when
        /// the game is paused</param>
        /// <param name="oneShot">Will the timer stay activated
        /// after the time is up until reset</param>
        public Timer(float targetTime, bool pausable, bool oneShot)
        {
            Init(targetTime, pausable, oneShot);
        }

        /// <summary>
        /// Initializes the timer.
        /// </summary>
        /// <param name="targetTime">The target time</param>
        /// <param name="pausable">Is the timer paused when
        /// the game is paused</param>
        /// <param name="oneShot">Will the timer stay activated
        /// after the time is up until reset</param>
        public void Init(float targetTime, bool pausable, bool oneShot)
        {
            this.targetTime = targetTime;
            _pausable = pausable;
            _oneShot = oneShot;

            if (targetTime <= 0f)
            {
                Debug.LogError("Target time must be a positive value.");
            }
        }

        public void Activate()
        {
            Active = true;
        }

        /// <summary>
        /// Updates the timer.
        /// </summary>
        /// <returns>Is the time up</returns>
        public bool Update()
        {
            if (Active)
            {
                _elapsedTime +=
                    (_pausable ? World.Instance.DeltaTime : Time.deltaTime);

                if (_elapsedTime >= targetTime)
                {
                    if (!_oneShot)
                    {
                        Finished = true;
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the elapsed time ratio.
        /// If the timer is not active, returns 0.
        /// </summary>
        /// <returns>A float from 0 to 1</returns>
        public float GetRatio()
        {
            if (!Active)
            {
                return 0f;
            }

            return Mathf.Clamp01(_elapsedTime / targetTime);
        }

        /// <summary>
        /// Resets the timer.
        /// </summary>
        public void Reset()
        {
            Active = false;
            Finished = false;
            _elapsedTime = 0f;
        }
    }
}
