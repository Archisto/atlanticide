using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class GradualSwitch : Switch
    {
        [SerializeField]
        private bool _loop;

        public float Progress { get; private set; }

        /// <summary>
        /// Sets the progress.
        /// </summary>
        /// <param name="progress">A new value for progress</param>
        /// <returns>Is the switch activated.</returns>
        public bool SetProgress(float progress)
        {
            if (!Activated || !_permanent)
            {
                if (_loop)
                {
                    // Puts a number larger than 1 between 0 and 1,
                    // bias for exactly 1 rather than 0
                    if (progress > 1f)
                    {
                        progress -= (int) progress;
                        progress = (progress == 0f ? 1 : progress);
                    }

                    // Puts a negative number between 0 and 1
                    progress = (progress < 0f ?
                        progress - (int) progress + 1 :
                        progress);

                    Progress = progress;
                }
                else
                {
                    Progress = Mathf.Clamp01(progress);
                }

                Activated = (Progress == 1f);
            }

            return Activated;
        }

        /// <summary>
        /// Increases or decreases the progress by the adjustment.
        /// </summary>
        /// <param name="adjustment">An adjustment to the progress value</param>
        /// <returns>Is the switch activated.</returns>
        public bool AdjustProgress(float adjustment)
        {
            return SetProgress(Progress + adjustment);
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
            Vector3 pos3 = transform.position + new Vector3(length * (Progress - 0.5f),  0.5f * height, 0);
            Vector3 pos4 = transform.position + new Vector3(length * (Progress - 0.5f),  -0.5f * height, 0);
            Gizmos.DrawLine(pos1, pos2);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pos3, pos4);
        }
    }
}
