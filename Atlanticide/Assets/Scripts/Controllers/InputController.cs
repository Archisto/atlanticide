using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class InputController : MonoBehaviour
    {
        private PlayerCharacter[] _players;
        private int _pausingPlayerNum;
        
        /// <summary>
        /// Is the game paused.
        /// </summary>
        public bool Paused { get; private set; }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _players = GameManager.Instance.GetPlayers();
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (!GameManager.Instance.FadeActive)
            {
                CheckPlayerInput();

                if (Paused)
                {
                    CheckMenuInput();
                }

                // Testing
                CheckDebugInput();
            }
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
                    (!Paused || IsAllowedToUnpause(i)))
                {
                    Paused = !Paused;
                    _pausingPlayerNum = (Paused ? i : -1);

                    if (Paused)
                    {
                        Debug.Log("Game paused by " + _players[_pausingPlayerNum].name);
                    }
                    else
                    {
                        Debug.Log("Game unpaused");
                    }
                }

                if (!Paused && !_players[i].IsDead)
                {
                    // Moving the player character
                    Vector3 movingDirection = _players[i].Input.GetMoveInput();
                    Vector3 lookingDirection = _players[i].Input.GetLookInput();

                    if (movingDirection != Vector3.zero)
                    {
                        _players[i].MoveInput(movingDirection);
                    }

                    if (lookingDirection != Vector3.zero)
                    {
                        _players[i].LookInput(lookingDirection);
                    }

                    // Jumping
                    if (_players[i].Input.GetJumpInput())
                    {
                        _players[i].Jump();
                    }

                    // Using an ability
                    _players[i].UseAbility(_players[i].Input.GetActionInput());

                    // Firing a weapon
                    if (_players[i].Input.GetAltActionInput())
                    {
                        _players[i].FireWeapon();
                    }
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
            // Reset
            if (Input.GetKeyDown(KeyCode.R))
            {
                GameManager.Instance.ResetLevel();
            }

            // Fade out/in
            if (Input.GetKeyDown(KeyCode.G))
            {
                FadeToColor fade = FindObjectOfType<FadeToColor>();
                if (fade != null)
                {
                    fade.StartNextFade();
                }
            }

            // Move NPCs
            if (Input.GetKey(KeyCode.J))
            {
                foreach (NonPlayerCharacter npc in GameManager.Instance.GetNPCs())
                {
                    npc.transform.position += Vector3.left * 5 * Time.deltaTime;
                }
            }
            else if (Input.GetKey(KeyCode.L))
            {
                foreach (NonPlayerCharacter npc in GameManager.Instance.GetNPCs())
                {
                    npc.transform.position += Vector3.right * 5 * Time.deltaTime;
                }
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
                GameManager.Instance.StartLoadingScene("Lauri's Colosseum");
            }
        }

        private void SetPlayerInputDevice(int playerNum, InputDevice inputDevice)
        {
            if (playerNum >= 0 && playerNum < _players.Length &&
                _players[playerNum] != null)
            {
                // The given player already has the input device
                if (_players[playerNum].Input.InputDevice == inputDevice)
                {
                    return;
                }

                foreach (PlayerCharacter player in _players)
                {
                    // The player which has the input device
                    // swaps it with the given player
                    if (player.Input.InputDevice == inputDevice)
                    {
                        InputDevice temp = _players[playerNum].Input.InputDevice;
                        _players[playerNum].Input.InputDevice = inputDevice;
                        player.Input.InputDevice = temp;
                        return;
                    }
                }

                _players[playerNum].Input.InputDevice = inputDevice;
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
    }
}
