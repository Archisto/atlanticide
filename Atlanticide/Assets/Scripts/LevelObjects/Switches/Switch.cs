using System;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// A tool for creating puzzles. Makes things happen based on player actions.
    /// </summary>
    public abstract class Switch : LevelObject
    {
        [SerializeField,
            Tooltip("Does the switch once activated stay active forever.")]
        protected bool _permanent;

        private bool _permanentByDefault;

        public bool Activated { get; protected set; }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected virtual void Start()
        {
            _permanentByDefault = _permanent;
        }

        /// <summary>
        /// Forces the switch to activate or deactivate.
        /// Works even on permanently activated switches.
        /// Please note that the switch's logic may
        /// re(de)activate it right after.
        /// </summary>
        /// <param name="active">Should the switch be activated</param>
        public void Activate(bool active)
        {
            Activated = active;
        }

        /// <summary>
        /// Sets the switch permanent or not permanent.
        /// </summary>
        /// <param name="permanent">Should the switch be permanent.</param>
        public void SetPermanent(bool permanent)
        {
            _permanent = permanent;
        }

        /// <summary>
        /// Resets the switch.
        /// </summary>
        public override void ResetObject()
        {
            Activated = false;
            _permanent = _permanentByDefault;
            base.ResetObject();
        }

        /// <summary>
        /// Draws gizmos.
        /// </summary>
        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = (Activated ? Color.green : Color.black);
        }
    }
}
