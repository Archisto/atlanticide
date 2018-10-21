using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class PressurePlate : Switch
    {
        [SerializeField]
        private GameObject _plate;

        [SerializeField]
        private Hitbox _hitbox;

        [SerializeField, Range(0.01f, 5f)]
        private float _pressTime = 0.15f;

        [SerializeField]
        private float _pressYMovement = -0.1f;

        [SerializeField]
        private LayerMask _mask;

        private bool _plateMoving;
        private Vector3 _plateDefaultPosition;
        private Vector3 _platePushedPosition;
        private Coroutine _runningRoutine;
        private Timer _pushDownTimer;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            _hitbox.mask = _mask;
            _pushDownTimer = new Timer(_pressTime, true, true);
            _plateDefaultPosition = _plate.transform.localPosition;
            _platePushedPosition = _plateDefaultPosition
                + Vector3.up * _pressYMovement;
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        protected override void UpdateObject()
        {
            CheckCollision();
            base.UpdateObject();
        }

        /// <summary>
        /// Checks whether the hitbox has been collided with
        /// and updates activation state accordingly.
        /// </summary>
        private void CheckCollision()
        {
            if (!Activated || !_permanent)
            {
                if (_hitbox.Collision != null)
                {
                    if (!Activated && !_plateMoving)
                    {
                        Activate();
                    }
                }
                else if (Activated && !_plateMoving)
                {
                    Deactivate();
                }
            }
        }

        private void Activate()
        {
            Activated = true;
            _runningRoutine = StartCoroutine(MovePlateRoutine());
        }

        private void Deactivate()
        {
            Activated = false;
            _runningRoutine = StartCoroutine(MovePlateRoutine());
        }

        /// <summary>
        /// Moves the plate up or down.
        /// </summary>
        /// <returns>IEnumerator</returns>
        private IEnumerator MovePlateRoutine()
        {
            Vector3 startPos = _plate.transform.localPosition;
            Vector3 targetPos =
                (Activated ? _platePushedPosition : _plateDefaultPosition);

            _plateMoving = true;
            _pushDownTimer.Activate();
            float ratio = 0f;
            while (ratio < 1f)
            {
                _plate.transform.localPosition =
                    Vector3.Lerp(startPos, targetPos, ratio);
                _pushDownTimer.Update();
                ratio = _pushDownTimer.GetRatio();
                yield return null;
            }

            _plate.transform.localPosition = targetPos;
            _pushDownTimer.Reset();
            _plateMoving = false;
        }

        /// <summary>
        /// Resets the object to its default state.
        /// </summary>
        public override void ResetObject()
        {
            _plate.transform.localPosition =_plateDefaultPosition;
            _pushDownTimer.Reset();
            _plateMoving = false;

            if (_runningRoutine != null)
            {
                StopCoroutine(_runningRoutine);
            }

            base.ResetObject();
        }

        /// <summary>
        /// Draws gizmos.
        /// </summary>
        protected override void OnDrawGizmos()
        {
            if (_drawGizmos)
            {
                base.OnDrawGizmos();
                Gizmos.DrawSphere(transform.position + Vector3.up * 1.5f, 0.5f);
            }
        }
    }
}
