using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// Controls the object's transparency.
    /// </summary>
    public class Transparency : Visuals
    {
        [SerializeField]
        private float _alpha = 1f;

        public float Alpha
        {
            get { return _alpha; }
        }

        public bool IsInvisible
        {
            get { return _alpha == 0f; }
        }

        /// <summary>
        /// Sets the object's material's alpha.
        /// </summary>
        /// <param name="alpha">An alpha value</param>
        public void SetAlpha(float alpha)
        {
            _alpha = Mathf.Clamp01(alpha);
            Color newColor = _material.color;
            newColor.a = _alpha;
            _material.color = newColor;
        }
    }
}
