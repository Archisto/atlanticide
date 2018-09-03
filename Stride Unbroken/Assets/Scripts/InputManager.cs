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
            // Rotating the big cube
            Vector3 direction = new Vector3(Input.GetAxisRaw(HorizontalKey), Input.GetAxisRaw(VerticalKey));

            if (direction != Vector3.zero)
            {
                
            }
        }
    }
}
