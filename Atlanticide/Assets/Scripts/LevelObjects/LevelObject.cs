using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// A base class for all level objects. Controls their basic behaviour.
    /// </summary>
    public abstract class LevelObject : MonoBehaviour
    {
        public event Action ObjectUpdated;
        public event Action ObjectDestroyed;
        public event Action ObjectReset;

        protected Vector3 _defaultPosition;

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (!World.Instance.GamePaused)
            {
                UpdateObject();
            }
        }

        /// <summary>
        /// Updates the object once per frame when the game is not paused.
        /// </summary>
        protected virtual void UpdateObject()
        {
            if (ObjectUpdated != null)
            {
                ObjectUpdated();
            }
        }

        /// <summary>
        /// Deactivates the object.
        /// </summary>
        public virtual void DestroyObject()
        {
            if (ObjectDestroyed != null)
            {
                ObjectDestroyed();
            }
        }

        /// <summary>
        /// Resets the object to its default state.
        /// </summary>
        public virtual void ResetObject()
        {
            if (ObjectReset != null)
            {
                ObjectReset();
            }
        }

        protected void RaiseObjectDestroyed()
        {
            if (ObjectDestroyed != null)
            {
                ObjectDestroyed();
            }
        }

        /// <summary>
        /// Sets the object to its self-defined default position.
        /// </summary>
        public void SetToDefaultPosition()
        {
            transform.position = _defaultPosition;
        }
    }
}
