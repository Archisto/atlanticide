using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class PlayerDetector : MonoBehaviour
    {
        private const string _playerString = "Player";

        private int _playerInt;

        public LevelElevator levelElevator;

        public float elevatorRaiseDelay;

        private float bothPlayersOnElevatorCounter = 0f;

        // Use this for initialization
        void Start()
        {
            _playerInt = LayerMask.NameToLayer(_playerString);
        }

        private void OnCollisionStay(Collision collision)
        {
            int playersInDetector = 0;
            foreach (ContactPoint cp in collision.contacts)
            {
                if (cp.otherCollider.gameObject.layer == _playerInt)
                {
                    playersInDetector++;
                }
            }
            if (playersInDetector == 2)
            {
                bothPlayersOnElevatorCounter += Time.deltaTime;
                if (bothPlayersOnElevatorCounter >= elevatorRaiseDelay)
                {
                    levelElevator.RaiseElevator();
                    gameObject.SetActive(false);
                }
            } else
            {
                bothPlayersOnElevatorCounter = 0f;
            }
        }
    }
}