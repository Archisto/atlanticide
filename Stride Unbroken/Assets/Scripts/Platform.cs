using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrideUnbroken
{
    public class Platform : MonoBehaviour
    {
        public int _timesBouncedOn;

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
            _timesBouncedOn = 0;
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

        /// <summary>
        /// Handles logic when the platform is bounced on.
        /// </summary>
        public virtual void BouncedOn()
        {
            _timesBouncedOn++;
        }

        protected virtual void Break()
        {
            IsBroken = true;
            gameObject.SetActive(false);
        }
    }
}
