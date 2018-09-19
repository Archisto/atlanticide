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

        [SerializeField]
        private PlayerCharacter _player;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {

        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            CheckInput();
            CheckDebugInput();
        }

        public Vector3 GetMoveInput()
        {
            return new Vector3(Input.GetAxis(HorizontalMoveKey), Input.GetAxis(VerticalMoveKey)).normalized;
        }

        public Vector3 GetLookInput()
        {
            return new Vector3(Input.GetAxis(HorizontalLookKey), Input.GetAxis(VerticalLookKey)).normalized;
        }

        private void CheckInput()
        {
            // Moving the player character
            Vector3 movingDirection = GetMoveInput();
            Vector3 lookingDirection = GetLookInput();

            if (movingDirection != Vector3.zero)
            {
                _player.MoveInput(movingDirection);
            }

            if (lookingDirection != Vector3.zero)
            {
                _player.LookInput(lookingDirection);
            }

            // Spend energy (for what?)
            _player.SpendEnergy(Input.GetButton(ActionKey));
        }

        private void CheckDebugInput()
        {
            // Respawn
            if (Input.GetKeyDown(KeyCode.R))
            {
                _player.Respawn();
            }
        }
    }
}
