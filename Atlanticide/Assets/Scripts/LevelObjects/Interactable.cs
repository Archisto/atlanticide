using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public abstract class Interactable : LevelObject
    {
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
                        _interactorIsValid = false;
                    }
                }
            }
        }


    }
}
