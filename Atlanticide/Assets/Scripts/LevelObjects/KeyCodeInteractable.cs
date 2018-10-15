using System;
using UnityEngine;

namespace Atlanticide
{
    public class KeyCodeInteractable : Interactable
    {
        [SerializeField]
        private bool _available = true;

        public int keyCode;

        [SerializeField]
        private int _defaultEnergyCost;

        [SerializeField]
        private bool _toggle;

        private bool _availableByDefault;

        public bool Available
        {
            get
            {
                return _available;
            }
            set
            {
                _available = value;
                if (!_available)
                {
                    SetInteractorTarget(false, true);
                }
            }
        }

        public bool Activated { get; private set; }

        public override bool ShowTargetIcon
        {
            get { return _showTargetIcon && Available; }
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _availableByDefault = Available;
            EnergyCost = _defaultEnergyCost;
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        protected override void UpdateObject()
        {
            if (Available)
            {
                if (Activated && !_toggle)
                {
                    Available = false;
                }
                else
                {
                    CheckForPlayerWithinRange();
                }

                base.UpdateObject();
            }
        }

        private void CheckForPlayerWithinRange()
        {
            if (Interactor == null)
            {
                // Gets a living player within range
                PlayerCharacter pc = GameManager.Instance.GetPlayerWithinRange
                    (transform.position, World.Instance.InteractRange);
                if (pc != null)
                {
                    Interactor = pc;
                }
            }

            if (Interactor != null)
            {
                if (Interactor.IsDead)
                {
                    Interactor = null;
                    return;
                }

                float distance = Vector3.Distance
                    (transform.position, Interactor.transform.position);
                _interactorIsValid = (distance <= World.Instance.InteractRange);
                SetInteractorTarget(_interactorIsValid, true);
            }
        }

        /// <summary>
        /// Makes the interactor player interact with this object.
        /// </summary>
        /// <returns>Was the interaction successful</returns>
        public override bool Interact()
        {
            if (!Activated)
            {
                World.Instance.TryActivateNewKeyCode(keyCode, true);
                Activated = true;

                return true;
            }
            else if (_toggle)
            {
                World.Instance.DeactivateKeyCode(keyCode);
                Activated = false;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Resets the object to its default state.
        /// </summary>
        public override void ResetObject()
        {
            Available = _availableByDefault;
            Activated = false;
            EnergyCost = _defaultEnergyCost;
            base.ResetObject();
        }

        /// <summary>
        /// Draws gizmos.
        /// </summary>
        protected override void OnDrawGizmos()
        {
            if (Available)
            {
                base.OnDrawGizmos();
            }

            Gizmos.color = (Activated ? Color.green : Color.black);
            Gizmos.DrawSphere(transform.position + Vector3.up * 1.5f, 0.5f);
        }
    }
}
