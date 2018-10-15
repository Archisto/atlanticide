using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// Adds features to the level object to which this component is attached.
    /// </summary>
    [RequireComponent(typeof(LevelObject))]
    public abstract class LevelObjectExpansion
        : MonoBehaviour, ILevelObjectExpansion
    {
        /// <summary>
        /// The expanded level object
        /// </summary>
        protected LevelObject _levelObj;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected virtual void Start()
        {
            _levelObj = GetComponent<LevelObject>();
            _levelObj.ObjectUpdated += OnObjectUpdated;
            _levelObj.ObjectDestroyed += OnObjectDestroyed;
            _levelObj.ObjectReset += OnObjectReset;
        }

        public virtual void OnObjectUpdated()
        {
        }

        public virtual void OnObjectDestroyed()
        {
        }

        public virtual void OnObjectReset()
        {
        }

        private void OnDestroy()
        {
            UnsubscribeObjectEvents();
        }

        private void UnsubscribeObjectEvents()
        {
            if (_levelObj != null)
            {
                _levelObj.ObjectUpdated -= OnObjectUpdated;
                _levelObj.ObjectDestroyed -= OnObjectDestroyed;
                _levelObj.ObjectReset -= OnObjectReset;
            }
        }
    }
}
