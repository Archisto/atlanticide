using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// Makes the object dissolve.
    /// </summary>
    public class Dissolve : Visuals
    {
        [SerializeField]
        private float _progress = 0f;

        public float Progress
        {
            get { return _progress; }
        }

        /// <summary>
        /// Sets the object's dissolve progress.
        /// </summary>
        /// <param name="progress">Dissolve progress
        /// from 0.0 to 1.0</param>
        public void SetProgress(float progress)
        {
            _progress = Mathf.Clamp01(progress);
            _material.SetFloat("_SliceAmount", _progress);
        }
    }
}
