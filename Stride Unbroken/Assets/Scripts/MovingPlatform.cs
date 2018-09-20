using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class MovingPlatform : Platform
    {
        [SerializeField]
        private Vector3[] _positions;

        [SerializeField]
        private bool _active;

        private Vector3 _startingPosition;
        private bool _activeByDefault;
        private int _currentPositionIndex;
        private bool _tickChanged;

        /// <summary>
        /// Changes the position index to be either previous or the next.
        /// </summary>
        /// <param name="increaseIndex">Should the position index be increased</param>
        /// <returns>A position index</returns>
        private int GetNewPositionIndex(bool increaseIndex)
        {
            int result = _currentPositionIndex;

            if (_positions.Length > 0)
            {
                result += (increaseIndex ? 1 : -1);
                if (increaseIndex && result == _positions.Length)
                {
                    result = 0;
                }
                else if (!increaseIndex && result == -1)
                {
                    result = _positions.Length - 1;
                }
            }

            return result;
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected override void Start()
        {
            if (_positions.Length > 0)
            {
                _startingPosition = _positions[0];
            }
            else
            {
                _startingPosition = transform.position;
            }

            _activeByDefault = _active;
            base.Start();
        }

        /// <summary>
        /// Resets the platform's values to their defaults.
        /// </summary>
        protected override void ResetValues()
        {
            base.ResetValues();
            transform.position = _startingPosition;
            _currentPositionIndex = 0;
            _active = _activeByDefault;
            _tickChanged = false;
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        protected override void Update()
        {
            if (!IsBroken && _active)
            {
                UpdateMovement();
            }
        }

        /// <summary>
        /// Handles the platform's movement on each tick.
        /// </summary>
        private void HandleTickEvent()
        {
            if (!IsBroken && _active)
            {
                UpdateNextPosition();
            }

            _tickChanged = true;
        }

        private void UpdateNextPosition()
        {
            _currentPositionIndex = GetNewPositionIndex(true);
        }

        /// <summary>
        /// Updates the platform's movement.
        /// </summary>
        private void UpdateMovement()
        {
            if (!_tickChanged)
            {
                // TODO: Not controlled by the metronome
                //transform.position = Vector3.Lerp(
                //    _positions[GetNewPositionIndex(false)],
                //    _positions[_currentPositionIndex],
                //    Metronome.TickRatio);
            }
            else
            {
                _tickChanged = false;
            }
        }

        /// <summary>
        /// Sets default values when the object is reset in the editor.
        /// </summary>
        private void Reset()
        {
            _active = true;
        }
    }
}

