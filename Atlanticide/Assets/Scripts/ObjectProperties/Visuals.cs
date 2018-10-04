using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// Modifies the object's visuals.
    /// </summary>
    public abstract class Visuals : MonoBehaviour
    {
        protected Material _material;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            Renderer renderer = GetComponent<Renderer>();
            _material = new Material(renderer.material);
            renderer.material = _material;
        }
    }
}
