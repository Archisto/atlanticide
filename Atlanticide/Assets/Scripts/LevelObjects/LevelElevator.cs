using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class LevelElevator : MonoBehaviour
    {
        private PlayerCharacter[] playerCharacters;

        public GameObject elevatorGear;

        public float elevatorSpeed,
            gearRotationSpeed;

        private float gearRotationAngle;

        public bool raiseElevator = false;

        private bool _hasPlayedElevatorSound;

        // Use this for initialization
        void Start()
        {
            playerCharacters = FindObjectsOfType<PlayerCharacter>();
            gearRotationAngle = Time.deltaTime * gearRotationSpeed;
        }

        // Update is called once per frame
        void Update()
        {
            if (raiseElevator)
            {
                Vector3 translateVector3 = new Vector3(0f, 0f, Time.deltaTime * elevatorSpeed);
                transform.Translate(translateVector3);
                elevatorGear.transform.Rotate(Vector3.left, gearRotationAngle);

                if (!_hasPlayedElevatorSound)
                {
                    SFXPlayer.Instance.PlayLooped(Sound.Elevator);

                    _hasPlayedElevatorSound = true;
                }
            }

            if (!raiseElevator && _hasPlayedElevatorSound)
            {
                SFXPlayer.Instance.StopIndividualSFX("ascend");

                _hasPlayedElevatorSound = false;
            }
        }

        public void RaiseElevator()
        {
            raiseElevator = true;
            // Freeze players here!
        }
    }
}