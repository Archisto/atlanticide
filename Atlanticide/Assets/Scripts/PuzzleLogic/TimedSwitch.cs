using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class TimedSwitch : Switch
    {
        [SerializeField]
        private float _duration = 1;

        private float _elapsedTime;

        public float Progress { get; private set; }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (!Activated || !_permanent)
            {
                UpdateTime();
            }
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

            _elapsedTime += Time.deltaTime;
            Progress = (_elapsedTime / _duration);
            if (_elapsedTime >= _duration)
            {
                _elapsedTime = 0f;
                Progress = 1f;
                Activated = true;
            }
        }

        /// <summary>
        /// Draws gizmos.
        /// </summary>
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            // Draws a progress bar
            float length = 2;
            float height = 0.5f;
            Vector3 pos1 = transform.position + Vector3.right * -0.5f * length;
            Vector3 pos2 = transform.position + Vector3.right * 0.5f * length;
            Vector3 pos3 = transform.position + new Vector3(length * (Progress - 0.5f), 0.5f * height, 0);
            Vector3 pos4 = transform.position + new Vector3(length * (Progress - 0.5f), -0.5f * height, 0);
            Gizmos.DrawLine(pos1, pos2);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pos3, pos4);
        }
    }
}
