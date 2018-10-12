using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class Shield : MonoBehaviour
    {
        [SerializeField, Range(0.1f, 5f)]
        private float _openTime = 0.5f;

        [SerializeField, Range(0.1f, 5f)]
        private float _bashTime = 0.25f;

        [SerializeField, Range(0.1f, 5f)]
        private float _bashRecoveryTime = 0.5f;

        private Transparency _tp; // testing
        private bool _active;
        private bool _bashActive;
        private bool _updateOpen;
        private bool _updateBash;
        private bool _instant;
        public float _openProgress;
        public float _bashProgress;
        private float _elapsedTime;

        public bool BlocksDamage
        {
            get { return _active && !_updateOpen && !_updateBash; }
        }

        public bool BashActive
        {
            get { return _bashActive; }
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            // Testing shield opening with transparency
            _tp = GetComponent<Transparency>();
            _tp.SetAlpha(_openProgress);
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (_updateBash)
            {
                UpdateShieldBash();
            }
            else if (_updateOpen)
            {
                UpdateOpenProgress();
                _tp.SetAlpha(_openProgress);
            }
        }

        private void UpdateOpenProgress()
        {
            _elapsedTime += World.Instance.DeltaTime;
            _openProgress = (_active ?
                _elapsedTime / _openTime : (_openTime - _elapsedTime) / _openTime);
            if (_elapsedTime >= _openTime)
            {
                _elapsedTime = 0f;
                _updateOpen = false;
                _openProgress = Mathf.Clamp01(_openProgress);
            }
        }

        private void UpdateShieldBash()
        {
            _elapsedTime += World.Instance.DeltaTime;
            float targetTime = (_bashActive ? _bashTime : _bashRecoveryTime);
            _bashProgress = (_bashActive ?
                _elapsedTime / targetTime : (targetTime - _elapsedTime) / targetTime);
            if (_elapsedTime >= targetTime)
            {
                _elapsedTime = 0f;
                if (_bashActive)
                {
                    _bashProgress = 1f;
                    BashClimax();
                }
                else
                {
                    _bashProgress = 0f;
                    _updateBash = false;
                }
            }
        }

        private void BashClimax()
        {
            // TODO
            _bashActive = false;
        }

        public void Activate(bool activate)
        {
            if (_active != activate)
            {
                _active = activate;

                if (_instant)
                {
                    _updateOpen = false;
                    _openProgress = (_active ? 1f : 0f);
                }
                else
                {
                    StartOpeningOrClosing();
                }
            }
        }

        private void StartOpeningOrClosing()
        {
            _updateOpen = true;
            if (!_updateBash && _elapsedTime > 0f)
            {
                _elapsedTime = _openTime - _elapsedTime;
            }
        }

        public void Bash()
        {
            if (!_bashActive &&
                BlocksDamage)
            {
                _bashActive = true;

                if (_instant)
                {
                    BashClimax();
                }
                else
                {
                    _updateBash = true;
                    _elapsedTime = 0f;
                }
            }
        }

        public void CancelBash()
        {
            _bashActive = false;
            _updateBash = false;
            _bashProgress = 0f;
            _elapsedTime = 0f;
        }

        // called when shield is hit by object (DamageDealer)
        public void Hit()
        {

        }

        /// <summary>
        /// Resets the shield to its default state.
        /// </summary>
        public void ResetShield()
        {
            // Makes the shield instantly return to its default state
            _instant = true;

            Activate(false);
            CancelBash();
            _elapsedTime = 0f;

            _instant = false;
        }

        /// <summary>
        /// Draws gizmos.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (_updateBash)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position,
                    transform.position + transform.up * 3f * _bashProgress);
            }
        }
    }
}