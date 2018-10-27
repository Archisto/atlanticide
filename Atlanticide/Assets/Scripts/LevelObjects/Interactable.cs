using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public abstract class Interactable : LevelObject
    {
        [SerializeField]
        private bool _replaceOtherInteractionTarget;

        [SerializeField]
        protected bool _removeInteractorAfterInteraction = true;

        [SerializeField]
        protected bool _showTargetIcon = true;

        protected bool _interactorIsValid;
        private PlayerCharacter _interactor;

        public PlayerCharacter Interactor
        {
            get
            {
                return _interactor;
            }
            set
            {
                if (value == null)
                {
                    _interactorIsValid = false;
                }
                else if (value.InteractionTarget != null)
                {
                    value.InteractionTarget.UnsetInteractorTarget(true);
                }

                _interactor = value;
            }
        }

        public virtual bool Available { get; set; }

        public int EnergyCost { get; protected set; }

        public virtual bool ShowTargetIcon { get { return _showTargetIcon; } }

        public bool IsPriorityTarget { get { return _replaceOtherInteractionTarget; } }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected virtual void Start()
        {
        }

        /// <summary>
        /// Makes the interactor player interact with this object.
        /// </summary>
        /// <returns>Was the interaction successful</returns>
        public abstract bool Interact();

        /// <summary>
        /// Returns whether this object can be set
        /// as the interactor's interaction target.
        /// </summary>
        /// <returns>Can this object be set as the
        /// interactor's interaction target</returns>
        protected bool CanInteractorTargetBeSet()
        {
            return Interactor != null && (IsPriorityTarget ?
                Interactor.InteractionTarget != this :
                Interactor.InteractionTarget == null);
        }

        /// <summary>
        /// Sets the interactor player's interaction
        /// target to be this object.
        /// </summary>
        public void SetInteractorTarget()
        {
            if (CanInteractorTargetBeSet())
            {
                Interactor.InteractionTarget = this;
            }
        }

        /// <summary>
        /// Makes the interactor player lose its connection to this object.
        /// It is also possible to set the interactor player null.
        /// </summary>
        /// <param name="removeInteractor">
        /// Should the interactor player be set null
        /// </param>
        public void UnsetInteractorTarget(bool removeInteractor)
        {
            if (Interactor != null)
            {
                if (Interactor.InteractionTarget == this)
                {
                    Interactor.InteractionTarget = null;
                }

                if (removeInteractor)
                {
                    Interactor = null;
                }
            }
            // TODO: Should this method ever be called without
            // checking if an interactor exists first?
            //else
            //{
            //    Debug.LogWarning("The interactor is already null.");
            //}
        }

        /// <summary>
        /// Tries to set the interactor player null after interaction.
        /// Called in the PlayerCharacter class.
        /// </summary>
        public virtual void TryRemoveInteractorAfterInteraction()
        {
            if (_removeInteractorAfterInteraction)
            {
                UnsetInteractorTarget(true);
            }
        }

        public override void ResetObject()
        {
            UnsetInteractorTarget(true);
            Interactor = null;
            base.ResetObject();
        }

        /// <summary>
        /// Draws interaction range gizmo.
        /// </summary>
        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = (_interactorIsValid ?
                Color.yellow : (IsPriorityTarget ? Color.black : new Color(0.4f, 0.4f, 0.4f, 1)));
            Gizmos.DrawWireSphere(transform.position, World.Instance.InteractRange);
        }
    }
}
