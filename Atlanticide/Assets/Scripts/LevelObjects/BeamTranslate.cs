using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class BeamTranslate : MonoBehaviour
    {
        public float movementMultiplier;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            float horizontal = Input.GetAxis("HorizontalMoveKeyboard");
            float vertical = Input.GetAxis("VerticalMoveKeyboard");

            transform.Translate(new Vector3(horizontal * movementMultiplier, 0f, vertical * movementMultiplier));
        }
    }
}