using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public abstract class LevelObject : MonoBehaviour, ILevelObject
    {
        protected Vector3 _defaultPosition;

        /// <summary>
        /// Deactivates the object.
        /// </summary>
        public virtual void Destroy()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Resets the object to its default state.
        /// </summary>
        public virtual void ResetObject()
        {
        }

        public void SetToDefaultPosition()
        {
            transform.position = _defaultPosition;
        }
    }
}
