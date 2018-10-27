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
        private bool _locked;

        public bool Active { get; private set; }
        public bool Finished { get; private set; }

        /// <summary>
        /// Creates the timer.
        /// </summary>
        /// <param name="targetTime">The target time</param>
        /// <param name="pausable">Is the timer paused when
        /// the game is paused</param>
        public Timer(float targetTime, bool pausable)
        {
            Init(targetTime, pausable);
        }

        /// <summary>
        /// Initializes the timer.
        /// </summary>
        /// <param name="targetTime">The target time</param>
        /// <param name="pausable">Is the timer paused when
        /// the game is paused</param>
        public void Init(float targetTime, bool pausable)
        {
            this.targetTime = targetTime;
            _pausable = pausable;
        }

        /// <summary>
        /// Activates the timer.
        /// </summary>
        public void Activate()
        {
            if (!_locked)
            {
                Reset();
                Active = true;
            }
        }

        /// <summary>
        /// Activates the timer with the given progress (0 to 1).
        /// </summary>
        /// <param name="progress">Progress at the start</param>
        public void Activate(float progress)
        {
            if (!_locked)
            {
                Reset();
                Active = true;
                progress = Mathf.Clamp01(progress);
                _elapsedTime = progress * targetTime;
            }
        }

        /// <summary>
        /// Updates the timer and returns whether the time is up.
        /// </summary>
        /// <returns>Is the time up</returns>
        public bool Check()
        {
            if (Active)
            {
                _elapsedTime +=
                    (_pausable ? World.Instance.DeltaTime : Time.deltaTime);

                if (_elapsedTime >= targetTime)
                {
                    Finish();
                    return true;
                }
            }
            else if (Finished)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the elapsed time ratio.
        /// If the timer is finished, returns 1,
        /// and if the timer is inactive, returns 0.
        /// </summary>
        /// <returns>A float from 0 to 1</returns>
        public float GetRatio()
        {
            if (Finished || targetTime <= 0f)
            {
                return 1f;
            }
            else if (!Active)
            {
                return 0f;
            }
            else
            {
                return Mathf.Clamp01(_elapsedTime / targetTime);
            }
        }

        /// <summary>
        /// Sets the timer finished and deactivates it.
        /// </summary>
        public void Finish()
        {
            Finished = true;
            Active = false;
        }

        public void SetLock(bool locked)
        {
            _locked = locked;
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
