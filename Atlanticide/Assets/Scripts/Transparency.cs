using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// Controls the object's transparency.
    /// </summary>
    public class Transparency : MonoBehaviour
    {
        [SerializeField]
        private float _alpha = 1f;

        private Material material;
        private Renderer _renderer;

        public float Alpha
        {
            get { return _alpha; }
        }

        public bool IsInvisible
        {
            get { return _alpha == 0f; }
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _renderer = GetComponent<Renderer>();
            material = new Material(_renderer.material);
        }

        /// <summary>
        /// Sets the object's material's transparency.
        /// </summary>
        /// <param name="alpha">An alpha value</param>
        public void SetAlpha(float alpha)
        {
            _alpha = Mathf.Clamp01(alpha);
            Color newColor = material.color;
            newColor.a = _alpha;
            material.color = newColor;
            _renderer.material = material;
        }
    }
}
