using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public abstract class Interactable : LevelObject
    {
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

                _interactor = value;
            }
        }

        public int EnergyCost { get; protected set; }

        public virtual bool ShowTargetIcon { get { return _showTargetIcon; } }

        /// <summary>
        /// Makes the interactor player interact with this object.
        /// </summary>
        /// <returns>Was the interaction successful</returns>
        public abstract bool Interact();

        /// <summary>
        /// Sets the interactor player's interaction
        /// target to be either this object or null.
        /// </summary>
        /// <param name="setThis">
        /// Should this object be set as the target
        /// </param>
        /// <param name="forgetInteractor">
        /// Should the interactor player be forgotten
        /// </param>
        public virtual void SetInteractorTarget(bool setThis, bool forgetInteractor = false)
        {
            if (Interactor != null)
            {
                if (setThis && Interactor.InteractionTarget != this)
                {
                    Interactor.InteractionTarget = this;
                }
                else if (!setThis && Interactor.InteractionTarget == this)
                {
                    Interactor.InteractionTarget = null;

                    if (forgetInteractor)
                    {
                        Interactor = null;
                    }
                }
            }
        }

        public override void ResetObject()
        {
            SetInteractorTarget(false, true);
            Interactor = null;
            base.ResetObject();
        }

        /// <summary>
        /// Draws gizmos.
        /// </summary>
        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = (_interactorIsValid ? Color.yellow : Color.black);
            Gizmos.DrawWireSphere(transform.position, World.Instance.InteractRange);
        }
    }
}
