using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// The input device: the keyboard or a gamepad.
    /// </summary>
    public enum InputDevice
    {
        Keyboard = 0,
        Gamepad1 = 1,
        Gamepad2 = 2,
        Gamepad3 = 3
    }

    /// <summary>
    /// Handles player input.
    /// </summary>
    public class PlayerInput
    {
        private string _horizontalMoveKey;
        private string _verticalMoveKey;
        private string _horizontalLookKey;
        private string _verticalLookKey;
        private string _actionKey;
        private string _altActionKey;
        private string _pauseKey;

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
                InputDevice = (InputDevice) playerNum;
            }
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
            _pauseKey = "Pause" + InputDevice.ToString();
        }

        /// <summary>
        /// Gets the player's move input.
        /// </summary>
        /// <returns>Moving direction</returns>
        public Vector3 GetMoveInput()
        {
            return new Vector3(
                Input.GetAxis(_horizontalMoveKey),
                Input.GetAxis(_verticalMoveKey))
                .normalized;
        }

        /// <summary>
        /// Gets the player's look input.
        /// </summary>
        /// <returns>Looking direction</returns>
        public Vector3 GetLookInput()
        {
            return new Vector3(
                Input.GetAxis(_horizontalLookKey),
                Input.GetAxis(_verticalLookKey))
                .normalized;
        }

        /// <summary>
        /// Gets the player's action input.
        /// </summary>
        /// <returns>Is the action input pressed</returns>
        public bool GetActionInput()
        {
            return Input.GetButton(_actionKey);
        }
    }
}
