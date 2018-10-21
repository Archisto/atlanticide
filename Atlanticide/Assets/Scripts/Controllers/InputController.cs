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
        // "Controller (XBOX 360 For Windows)"
        private const int XboxControllerNameLength = 33;

        // "Wireless Controller"
        private const int PSControllerNameLength = 19;

        public List<PlayerInput> _inputs;

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
            _inputs = new List<PlayerInput>
            {
                new PlayerInput(InputDevice.Keyboard),
                new PlayerInput(InputDevice.Gamepad1),
                new PlayerInput(InputDevice.Gamepad2),
                new PlayerInput(InputDevice.Gamepad3)
            };

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

                if (GameManager.Instance.GameState == GameManager.State.PressStart)
                {
                    CheckPressStartInput();
                }
                else if (GameManager.Instance.GameState != GameManager.State.Play
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

        private void CheckPressStartInput()
        {
            foreach (PlayerInput input in _inputs)
            {
                input.GetPressStartInput();
            }
        }

        /// <summary>
        /// Checks debugging input.
        /// </summary>
        private void CheckDebugInput()
        {
            // Change scene
            LoadLevelOrPuzzle();

            // Pause play mode
            if (Input.GetKeyDown(KeyCode.P))
            {
                Debug.Break();
            }

            if (_players != null)
            {
                // Respawn players
                if (Input.GetKeyDown(KeyCode.U))
                {
                    foreach (PlayerCharacter player in _players)
                    {
                        player.Respawn();
                    }
                }

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

                // Toggle noclip
                if (Input.GetKeyDown(KeyCode.I))
                {
                    bool noclip = false;
                    foreach (PlayerCharacter player in _players)
                    {
                        WallCollider wc = player.GetComponent<WallCollider>();
                        GroundCollider gc = player.GetComponent<GroundCollider>();
                        noclip = wc.enabled;
                        wc.enabled = !noclip;
                        gc.enabled = !noclip;
                    }
                    Debug.Log("Noclip: " + noclip);
                }

                // Toggle god mode
                if (Input.GetKeyDown(KeyCode.L))
                {
                    bool godMode = false;
                    foreach (PlayerCharacter player in _players)
                    {
                        godMode = !player.IsInvulnerable;
                        player.IsInvulnerable = godMode;

                    }
                    Debug.Log("God mode: " + godMode);
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

                // First-person mode
                if (Input.GetKeyDown(KeyCode.O))
                {
                    _camera.SetFirstPersonPlayer(_camera.firstPersonMode ? null : _players[0]);
                }

                // Save game
                if (Input.GetKeyDown(KeyCode.F5))
                {
                    GameManager.Instance.SaveGame();
                }

                // Load game
                if (Input.GetKeyDown(KeyCode.F9))
                {
                    GameManager.Instance.LoadGame();
                }
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
                        otherPlayer.Input.InputDevice =
                            _players[playerNum].Input.InputDevice;
                        LogController(otherPlayer);
                        break;
                    }
                }

                _players[playerNum].Input.InputDevice = inputDevice;
                LogController(_players[playerNum]);
                GameManager.Instance.SaveInputDevices();
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

        // Testing
        private void LoadLevelOrPuzzle()
        {
            bool input = false;
            int levelOrPuzzle = 1;
            bool level = true;

            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                if (World.Instance.GamePaused)
                {
                    World.Instance.PauseGame(false);
                }
                GameManager.Instance.LoadTestLevel();
                return;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                levelOrPuzzle = 1;
                level = true;
                input = true;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                levelOrPuzzle = 2;
                level = false;
                input = true;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                levelOrPuzzle = 2;
                level = true;
                input = true;
            }

            if (input)
            {
                if (level)
                {
                    GameManager.Instance.LoadLevel(levelOrPuzzle);
                }
                else
                {
                    GameManager.Instance.LoadPuzzle(levelOrPuzzle);
                }
            }
        }
    }
}
