using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class Door : LevelObject
    {
        [SerializeField]
        private int _keyCode;

        [SerializeField]
        private float _openTime = 1f;

        [SerializeField]
        private Vector3 _openRotation;

        public bool unlocked;
        public bool open;
        private bool _defaultUnlocked;
        private bool _defaultOpen;
        private bool _changingOpenState;
        private float _openProgress;
        private float _elapsedTime;
        private Quaternion _closedRotation;
        private Quaternion _openRotationQ;

        private PlayerProximitySwitch _proxSwitch;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _proxSwitch = GetComponent<PlayerProximitySwitch>();
            if (_proxSwitch == null)
            {
                Debug.LogError(Utils.GetComponentMissingString("ProximitySwitch"));
            }

            if (open)
            {
                _openProgress = 1f;
            }

            _closedRotation = transform.rotation;
            _openRotationQ = Quaternion.Euler(_openRotation);
            _defaultUnlocked = unlocked;
            _defaultOpen = open;
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (_changingOpenState && _openTime > 0f)
            {
                UpdateOpenProgress();
            }
            else if (!open)
            {
                CheckForKey();
            }
        }

        private void UpdateOpenProgress()
        {
            _openProgress = (open ? _elapsedTime / _openTime : (_openTime - _elapsedTime) / _openTime);
            _elapsedTime += World.Instance.DeltaTime;
            if (_elapsedTime >= _openTime)
            {
                _elapsedTime = 0f;
                _changingOpenState = false;
                _openProgress = Mathf.Clamp01(_openProgress);
            }

            Quaternion newRotation = Quaternion.Lerp(_closedRotation, _openRotationQ, _openProgress);
            transform.rotation = newRotation;
        }

        private void CheckForKey()
        {
            if (_proxSwitch != null && _proxSwitch.Activated)
            {
                foreach (int ownedKeyCode in World.Instance.keyCodes)
                {
                    if (_keyCode == ownedKeyCode)
                    {
                        Unlock();
                        Open();
                    }
                }
            }
        }

        public void Open()
        {
            if (unlocked && !open)
            {
                open = true;
                _changingOpenState = true;
            }
        }

        public void Close()
        {
            if (open)
            {
                open = false;
                _changingOpenState = true;
            }
        }

        public void Unlock()
        {
            if (!unlocked)
            {
                unlocked = true;
            }
        }

        public void Lock()
        {
            if (unlocked)
            {
                Close();
                unlocked = false;
            }
        }

        /// <summary>
        /// Resets the object to its default state.
        /// </summary>
        public override void ResetObject()
        {
            unlocked = _defaultUnlocked;
            open = _defaultOpen;
            _changingOpenState = false;
            _openProgress = (open ? 1f : 0f);
            transform.rotation = (open ? _openRotationQ : _closedRotation);
        }

        /// <summary>
        /// Draws gizmos.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (unlocked)
            {
                Gizmos.color = (open ? Color.green : Color.yellow);
                Gizmos.DrawSphere(transform.position, 0.5f);
            }
        }
    }
}
