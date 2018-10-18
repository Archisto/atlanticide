using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// Handles player input.
    /// </summary>
    public class PlayerInput
    {
        private const float HoldTime = 0.35f;

        private string _horizontalMoveKey;
        private string _verticalMoveKey;
        private string _horizontalLookKey;
        private string _verticalLookKey;
        private string _actionKey;
        private string _altActionKey;
        private string _stanceKey;
        private string _interactKey;
        private string _jumpKey;
        private string _toolSwapKey;
        private string _pauseKey;

        private HoldInput _holdActionInput;

        private InputDevice _inputDevice;
        public InputDevice InputDevice
        {
            get
            {
                return _inputDevice;
            }
            set
            {
                _inputDevice = value;
                UpdateInputKeys();
            }
        }

        /// <summary>
        /// Creates the player input.
        /// </summary>
        /// <param name="playerNum">The player number</param>
        public PlayerInput(int playerNum)
        {
            if (playerNum >= 0 && playerNum <= 3)
            {
                if(playerNum != 0)
                {
                    playerNum = 2;
                }
                InputDevice = (InputDevice) playerNum;
            }

            _holdActionInput = new HoldInput(HoldTime);
        }

        /// <summary>
        /// Updates the input key strings.
        /// </summary>
        private void UpdateInputKeys()
        {
            _horizontalMoveKey = "HorizontalMove" + InputDevice.ToString();
            _verticalMoveKey = "VerticalMove" + InputDevice.ToString();
            _horizontalLookKey = "HorizontalLook" + InputDevice.ToString();
            _verticalLookKey = "VerticalLook" + InputDevice.ToString();
            _actionKey = "Action" + InputDevice.ToString();
            _altActionKey = "AltAction" + InputDevice.ToString();
            _stanceKey = "Stance" + InputDevice.ToString();
            _interactKey = "Interact" + InputDevice.ToString();
            _jumpKey = "Jump" + InputDevice.ToString();
            _toolSwapKey = "ToolSwap" + InputDevice.ToString();
            _pauseKey = "Pause" + InputDevice.ToString();
        }

        /// <summary>
        /// Gets the player's move input.
        /// </summary>
        /// <returns>Moving direction</returns>
        public Vector3 GetMoveInput()
        {
            return new Vector3(
                Input.GetAxisRaw(_horizontalMoveKey),
                Input.GetAxisRaw(_verticalMoveKey));
        }

        /// <summary>
        /// Gets the player's look input.
        /// </summary>
        /// <returns>Looking direction</returns>
        public Vector3 GetLookInput()
        {
            return new Vector3(
                Input.GetAxisRaw(_horizontalLookKey),
                Input.GetAxisRaw(_verticalLookKey));
        }

        /// <summary>
        /// Gets the player's action input.
        /// </summary>
        /// <returns>Is the action input pressed</returns>
        public bool GetActionInput(out bool inputHeld, out bool inputjustReleased)
        {
            bool input = Input.GetButton(_actionKey) || Input.GetAxis(_actionKey) == 1;
            inputHeld = _holdActionInput.InputIsHeld(input);
            inputjustReleased = _holdActionInput.InputJustReleased;
            return input;
        }

        /// <summary>
        /// Gets the player's alternate action input.
        /// </summary>
        /// <returns>Is the alt action input pressed</returns>
        public bool GetAltActionInput()
        {
            return Input.GetButton(_altActionKey) || Input.GetAxis(_altActionKey) == 1;
        }

        /// <summary>
        /// Gets the player's stance input.
        /// </summary>
        /// <returns>Is the stance input pressed</returns>
        public bool GetStanceInput()
        {
            return Input.GetButtonDown(_stanceKey);
        }

        /// <summary>
        /// Gets the player's interact input.
        /// </summary>
        /// <returns>Is the interact input pressed</returns>
        public bool GetInteractInput()
        {
            return Input.GetButtonDown(_interactKey);
        }

        /// <summary>
        /// Gets the player's jump input.
        /// </summary>
        /// <returns>Is the jump input pressed</returns>
        public bool GetJumpInput()
        {
            return Input.GetButtonDown(_jumpKey);
        }

        /// <summary>
        /// Gets the player's tool swap input.
        /// </summary>
        /// <returns>Is the jump input pressed</returns>
        public bool GetToolSwapInput()
        {
            return Input.GetButtonDown(_toolSwapKey);
        }

        /// <summary>
        /// Gets the player's pause input.
        /// </summary>
        /// <returns>Is the pause input pressed</returns>
        public bool GetPauseInput()
        {
            return Input.GetButtonDown(_pauseKey);
        }

        public void ResetInput()
        {
            _holdActionInput.ResetHold();
        }
    }
}
