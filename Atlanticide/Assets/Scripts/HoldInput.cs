using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class HoldInput
    {
        private float _holdTime;
        private float _elapsedHoldTime;
        private bool _held;
        private bool _oldInput;

        public bool InputJustReleased { get; private set; }

        public HoldInput(float holdTime)
        {
            _holdTime = holdTime;
        }

        public bool InputIsHeld(bool input)
        {
            InputJustReleased = false;

            if (input && !_held)
            {
                _elapsedHoldTime += World.Instance.DeltaTime;
                if (_elapsedHoldTime >= _holdTime)
                {
                    _held = true;
                }
            }
            else if (!input && _oldInput)
            {
                if (_held)
                {
                    ResetHold();
                }
                else
                {
                    _elapsedHoldTime = 0f;
                }

                InputJustReleased = true;
            }

            _oldInput = input;
            return _held;
        }

        public void ResetHold()
        {
            _oldInput = false;
            _held = false;
            _elapsedHoldTime = 0f;
            InputJustReleased = false;
        }
    }
}
