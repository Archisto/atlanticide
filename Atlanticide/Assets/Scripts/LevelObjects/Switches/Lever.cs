using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    [RequireComponent(typeof(KeyCodeInteractable))]
    public class Lever : Switch
    {
        [SerializeField]
        private Transform _handlePivot;

        [SerializeField]
        private float _offAngle = -25;

        [SerializeField]
        private float _onAngle = 25;

        [SerializeField]
        private float _moveTime = 1f;

        [SerializeField]
        private bool _onByDefault;

        protected KeyCodeInteractable _interactable;
        protected bool _handleMoving;
        private Timer _moveTimer;
        private Quaternion _offRotation;
        private Quaternion _onRotation;
        private Coroutine _runningRoutine;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            _interactable = GetComponent<KeyCodeInteractable>();
            _moveTimer = new Timer(_moveTime, true);
            _offRotation = Quaternion.Euler(new Vector3(0, 0, _offAngle));
            _onRotation = Quaternion.Euler(new Vector3(0, 0, _onAngle));
            Init();
        }

        /// <summary>
        /// Initializes the object at the start and when reset.
        /// </summary>
        private void Init()
        {
            Activated = _onByDefault;
            _handlePivot.localRotation = (Activated ? _onRotation : _offRotation);
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        protected override void UpdateObject()
        {
            bool canBeOperated = CanBeOperated();

            if (_interactable.Available != canBeOperated)
            {
                _interactable.Available = canBeOperated;
            }

            if (canBeOperated)
            {
                bool targetState = !(_onByDefault != Activated);
                if (_interactable.Activated == targetState)
                {
                    Activated = !Activated;
                    _interactable.Available = false;
                    _runningRoutine = StartCoroutine(MoveLeverRoutine());
                }

                base.UpdateObject();
            }
        }

        protected virtual bool CanBeOperated()
        {
            return !_handleMoving && !(Activated && _permanent);
        }

        /// <summary>
        /// Moves the lever back or forth.
        /// </summary>
        /// <returns>IEnumerator</returns>
        private IEnumerator MoveLeverRoutine()
        {
            Quaternion startRot = _handlePivot.localRotation;
            Quaternion targetRot =
                (Activated ? _onRotation : _offRotation);

            _handleMoving = true;
            _moveTimer.Activate();
            float ratio = 0f;
            while (ratio < 1f)
            {
                _handlePivot.localRotation =
                    Quaternion.Lerp(startRot, targetRot, ratio);
                _moveTimer.Check();
                ratio = _moveTimer.GetRatio();
                yield return null;
            }

            _handlePivot.localRotation = targetRot;
            _moveTimer.Reset();
            _handleMoving = false;
        }

        public override void ResetObject()
        {
            _handleMoving = false;
            _moveTimer.Reset();
            if (_runningRoutine != null)
            {
                StopCoroutine(_runningRoutine);
            }
            Init();
            base.ResetObject();
        }
    }
}

