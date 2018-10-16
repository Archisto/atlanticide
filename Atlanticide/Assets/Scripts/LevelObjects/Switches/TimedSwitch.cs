using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class TimedSwitch : Switch
    {
        [SerializeField]
        private float _duration = 1f;

        private float _elapsedTime;

        public float Progress { get; private set; }

        /// <summary>
        /// Updates the object.
        /// </summary>
        protected override void UpdateObject()
        {
            if (!Activated || !_permanent)
            {
                UpdateTime();
            }

            base.UpdateObject();
        }

        /// <summary>
        /// Updates the elapsed time. If the time is up, activates the
        /// switch. If the switch is not permanent, it is deactivated
        /// and the clock reset on the next frame.
        /// </summary>
        private void UpdateTime()
        {
            if (Activated && _elapsedTime < 1f)
            {
                Progress = 0f;
                Activated = false;
            }

            _elapsedTime += World.Instance.DeltaTime;
            Progress = (_elapsedTime / _duration);
            if (_elapsedTime >= _duration)
            {
                _elapsedTime = 0f;
                Progress = 1f;
                Activated = true;
            }
        }

        /// <summary>
        /// Resets the switch.
        /// </summary>
        public override void ResetObject()
        {
            Progress = 0f;
            _elapsedTime = 0f;
            base.ResetObject();
        }

        /// <summary>
        /// Draws gizmos.
        /// </summary>
        protected override void OnDrawGizmos()
        {
            if (_drawGizmos)
            {
                base.OnDrawGizmos();
                Utils.DrawProgressBarGizmo(transform.position,
                    Progress, Gizmos.color, Color.yellow);
            }
        }
    }
}
