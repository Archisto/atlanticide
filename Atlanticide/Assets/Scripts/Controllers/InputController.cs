using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class InputController : MonoBehaviour
    {
        private const string HorizontalMoveKey = "HorizontalMove";
        private const string VerticalMoveKey = "VerticalMove";
        private const string HorizontalLookKey = "HorizontalLook";
        private const string VerticalLookKey = "VerticalLook";
        private const string ActionKey = "Action";

        [SerializeField]
        private CameraController _camera;

        private PlayerCharacter[] _players;

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
            CheckInput();
            CheckDebugInput();
        }

        private void CheckInput()
        {
            for (int i = 0; i < _players.Length; i++)
            {
                if (_players[i] != null && _players[i].gameObject.activeSelf && i <= 1) // TODO: Remove 2-player limit
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

                    // Spend energy (for what?)
                    _players[i].SpendEnergy(_players[i].Input.GetActionInput());
                }
            }
        }

        private void CheckDebugInput()
        {
            // Respawn
            if (Input.GetKeyDown(KeyCode.R))
            {
                _players[0].Respawn();
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
    }
}
