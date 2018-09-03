using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrideUnbroken
{
    public class InputManager : MonoBehaviour
    {
        private const string HorizontalKey = "Horizontal";
        private const string VerticalKey = "Vertical";
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
        }

        private void CheckInput()
        {
            // Moving the player character
            Vector3 direction = new Vector3(Input.GetAxisRaw(HorizontalKey), Input.GetAxisRaw(VerticalKey));

            if (direction != Vector3.zero)
            {
                _player.MoveInput(direction);
            }

            // Double tempo
            if (Input.GetButton(ActionKey))
            {
                _player.DoubleTempoInput(true);
            }
            else
            {
                _player.DoubleTempoInput(false);
            }
        }
    }
}
