using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class Platform : MonoBehaviour
    {
        public bool IsBroken { get; protected set; }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected virtual void Start()
        {
            ResetValues();
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected virtual void ResetValues()
        {
            IsBroken = false;
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        protected virtual void Update()
        {
            if (!IsBroken)
            {

            }
        }

        protected virtual void Break()
        {
            IsBroken = true;
            gameObject.SetActive(false);
        }
    }
}
