﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class InputController : MonoBehaviour
    {
        private PlayerCharacter[] _players;
        private ToolSwapping _toolSwap;
        private int _pausingPlayerNum;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _players = GameManager.Instance.GetPlayers();
            _toolSwap = FindObjectOfType<ToolSwapping>();
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

                if (!World.Instance.GamePaused && !_players[i].IsDead)
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

                    // Tool swapping
                    if (_players[i].Input.GetToolSwapInput())
                    {
                        _toolSwap.InitiateSwapRequest(_players[i]);
                    }

                    // Using the player's primary action
                    _players[i].CheckActionInput();

                    // Using the player's alternate action
                    _players[i].CheckAltActionInput();
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
            // Move NPCs
            if (Input.GetKey(KeyCode.J))
            {
                foreach (NonPlayerCharacter npc in GameManager.Instance.GetNPCs())
                {
                    npc.transform.position += Vector3.left * 5 * World.Instance.DeltaTime;
                }
            }
            else if (Input.GetKey(KeyCode.L))
            {
                foreach (NonPlayerCharacter npc in GameManager.Instance.GetNPCs())
                {
                    npc.transform.position += Vector3.right * 5 * World.Instance.DeltaTime;
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
                GameManager.Instance.LoadTestLevel();
            }

            // Pause play mode
            if (Input.GetKeyDown(KeyCode.P))
            {
                Debug.Break();
            }
        }

        public void TogglePause(int pausingPlayer)
        {
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

        /// <summary>
        /// Resets the input controller.
        /// </summary>
        public void ResetInput()
        {
            _toolSwap.EndSwapRequest();
        }
    }
}
