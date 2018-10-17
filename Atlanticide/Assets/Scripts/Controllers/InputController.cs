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
    /// The gamepad type.
    /// </summary>
    public enum GamepadType
    {
        None = 0,
        Xbox = 1,
        PlayStation = 2
    }

    public class InputController : MonoBehaviour
    {
        private const int XboxControllerNameLength = 33;
        private const int PSControllerNameLength = 19;

        private PlayerCharacter[] _players;
        private ToolSwapping _toolSwap;
        private CameraController _camera;
        private int _pausingPlayerNum;
        private float _inputDeadZone = 0.2f;

        private float SqrInputDeadZone
        {
            get { return _inputDeadZone * _inputDeadZone; }
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _players = GameManager.Instance.GetPlayers();
            _toolSwap = FindObjectOfType<ToolSwapping>();
            _camera = FindObjectOfType<CameraController>();

            CheckConnectedControllers();
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (!GameManager.Instance.FadeActive)
            {
                if (GameManager.Instance.PlayReady)
                {
                    CheckPlayerInput();
                }

                if (GameManager.Instance.GameState != GameManager.State.Play
                    || World.Instance.GamePaused)
                {
                    CheckMenuInput();
                }

                // Testing
                CheckDebugInput();
            }
        }

        private Vector3 GetFinalMovingInput(Vector3 movingInput)
        {
            float moveMagnitude = movingInput.magnitude;
            if (moveMagnitude > _inputDeadZone)
            {
                //Debug.Log("move x: " + movingInput.x + ", y: " + movingInput.y + "; mag: " + moveMagnitude);

                float movementFactor = ((moveMagnitude - _inputDeadZone) / (1 - _inputDeadZone));
                movementFactor = (movementFactor > 1f ? 1f : movementFactor);
                movingInput = movingInput.normalized * movementFactor;

                return movingInput;
            }

            return Vector3.zero;
        }

        private Vector3 GetFinalLookingInput(Vector3 lookingInput)
        {
            if (lookingInput.sqrMagnitude > SqrInputDeadZone)
            {
                return lookingInput.normalized;
            }

            return Vector3.zero;
        }

        /// <summary>
        /// Checks player specific input.
        /// </summary>
        private void CheckPlayerInput()
        {
            for (int i = 0; i < GameManager.Instance.PlayerCount; i++)
            {
                // Pausing and unpausing the game
                if (_players[i].Input.GetPauseInput() &&
                    (!World.Instance.GamePaused || IsAllowedToUnpause(i)))
                {
                    TogglePause(i);
                }

                if (!_players[i].IsDead)
                {
                    if (!World.Instance.GamePaused)
                    {
                        // Moving the player character
                        Vector3 movingInput = GetFinalMovingInput(_players[i].Input.GetMoveInput());
                        Vector3 lookingInput = GetFinalLookingInput(_players[i].Input.GetLookInput());

                        if (movingInput != Vector3.zero)
                        {
                            _players[i].MoveInput(movingInput);
                        }

                        if (lookingInput != Vector3.zero)
                        {
                            _players[i].LookInput(lookingInput);
                        }

                        // Changing stance
                        _players[i].HandleStanceInput();

                        // Interacting with certain level objects
                        _players[i].HandleInteractionInput();

                        // Jumping
                        _players[i].HandleJumpInput();

                        // Using the player's primary action
                        if (_players[i].HandleActionInput())
                        {
                            CancelToolSwap(_players[i]);
                        }

                        // Using the player's alternate action
                        if (_players[i].HandleAltActionInput())
                        {
                            CancelToolSwap(_players[i]);
                        }

                        // Tool swapping
                        if (_players[i].CheckToolSwapInput())
                        {
                            _toolSwap.InitiateSwapRequest(_players[i]);
                        }
                    }
                }
                // If the player has initiated a tool swap request
                // but dies, the request is canceled
                else
                {
                    CancelToolSwap(_players[i]);
                }
            }
        }
        
        /// <summary>
        /// Checks menu input.
        /// </summary>
        private void CheckMenuInput()
        {
            // TODO
        }

        /// <summary>
        /// Checks debugging input.
        /// </summary>
        private void CheckDebugInput()
        {
            // Max energy
            if (Input.GetKeyDown(KeyCode.Y))
            {
                World.Instance.SetEnergyChargesAndUpdateUI
                    (World.Instance.MaxEnergyCharges);
            }

            // Swap tools
            if (Input.GetKeyDown(KeyCode.T))
            {
                _toolSwap.SwapTools();
            }

            // Change player count
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                GameManager.Instance.ActivatePlayers(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                GameManager.Instance.ActivatePlayers(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                GameManager.Instance.ActivatePlayers(3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                GameManager.Instance.ActivatePlayers(4);
            }

            // Change scene
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                GameManager.Instance.LoadLevel(1);
            }
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                GameManager.Instance.LoadLevel(2);
            }
            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                if (World.Instance.GamePaused)
                {
                    World.Instance.PauseGame(false);
                }
                GameManager.Instance.LoadTestLevel();
            }

            // Pause play mode
            if (Input.GetKeyDown(KeyCode.P))
            {
                Debug.Break();
            }

            // First-person mode
            if (Input.GetKeyDown(KeyCode.O))
            {
                _camera.SetFirstPersonPlayer(_camera.firstPersonMode ? null : _players[0]);
            }
        }

        public void TogglePause(int pausingPlayer)
        {
            if (_camera.firstPersonMode)
            {
                _camera.SetFirstPersonPlayer(null);
            }

            if (!World.Instance.GamePaused)
            {
                _pausingPlayerNum = pausingPlayer;
                World.Instance.PauseGame(true, _players[_pausingPlayerNum].name);
            }
            else
            {
                _pausingPlayerNum = -1;
                World.Instance.PauseGame(false);
            }
        }

        /// <summary>
        /// Returns whether the given player can unpause the game.
        /// If the player is unavailable, anyone can unpause.
        /// </summary>
        /// <param name="playerNum">A player number</param>
        /// <returns>Can the player unpause.</returns>
        private bool IsAllowedToUnpause(int playerNum)
        {
            if (_pausingPlayerNum >= GameManager.Instance.PlayerCount)
            {
                return true;
            }
            else
            {
                return _pausingPlayerNum == playerNum;
            }
        }

        private void CancelToolSwap(PlayerCharacter player)
        {
            if (_toolSwap.RequestInitiatedBy(player))
            {
                _toolSwap.EndSwapRequest();
            }
        }

        /// <summary>
        /// Swaps the input devices of players 1 and 2.
        /// </summary>
        public void SwapInputDevices()
        {
            SetPlayerInputDevice(0, _players[1].Input.InputDevice);
        }

        /// <summary>
        /// Sets the input device of the given player.
        /// If the input device is already used by a player,
        /// the input devices are swapped.
        /// </summary>
        /// <param name="playerNum">A player's index in the array</param>
        /// <param name="inputDevice">An input device</param>
        private void SetPlayerInputDevice(int playerNum, InputDevice inputDevice)
        {
            if (playerNum >= 0 && playerNum < _players.Length &&
                _players[playerNum] != null)
            {
                // The given player already has the input device
                if (_players[playerNum].Input.InputDevice == inputDevice)
                {
                    Debug.LogWarning("The player already has that input device.");
                    return;
                }

                foreach (PlayerCharacter otherPlayer in _players)
                {
                    // The other player which has the input device
                    // swaps it with the given player
                    if (otherPlayer.Input.InputDevice == inputDevice)
                    {
                        InputDevice temp = _players[playerNum].Input.InputDevice;
                        _players[playerNum].Input.InputDevice = inputDevice;
                        otherPlayer.Input.InputDevice = temp;
                        LogController(_players[playerNum]);
                        LogController(otherPlayer);
                        return;
                    }
                }

                // The input device is currently not used by anyone,
                // so it is simply set to the player
                _players[playerNum].Input.InputDevice = inputDevice;
                LogController(_players[playerNum]);
            }
        }

        public void CheckConnectedControllers()
        {
            for (int i = 0; i < Input.GetJoystickNames().Length; i++)
            {
                if (string.IsNullOrEmpty(Input.GetJoystickNames()[i]))
                {
                    Debug.LogWarning("There is no controller attached to this slot!");
                }
                else
                {
                    Debug.Log("Controller: " + 
                        GetConnectedControllerType(Input.GetJoystickNames()[i]));
                }
            }
        }

        public GamepadType GetConnectedControllerType(string joystickName)
        {
            if (string.IsNullOrEmpty(joystickName))
            {
                return GamepadType.None;
            }
            else if (joystickName.Length == XboxControllerNameLength)
            {
                return GamepadType.Xbox;
            }
            else if (joystickName.Length == PSControllerNameLength)
            {
                return GamepadType.PlayStation;
            }
            else
            {
                return GamepadType.None;
            }
        }

        /// <summary>
        /// Resets the input controller.
        /// </summary>
        public void ResetInput()
        {
            _toolSwap.EndSwapRequest();
        }

        private void LogController(PlayerCharacter player)
        {
            Debug.Log(player.name + "'s controller: " +
                player.Input.InputDevice);
        }
    }
}
