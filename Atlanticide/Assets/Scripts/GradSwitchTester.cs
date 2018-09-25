using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class GradSwitchTester : MonoBehaviour
    {
        [SerializeField]
        float maxDist = 5;

        [SerializeField]
        bool increment;

        GradualSwitch mySwitch;
        PlayerCharacter[] players;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            mySwitch = GetComponent<GradualSwitch>();
            players = GameManager.Instance.GetPlayers();
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            float dist = maxDist - players[0].transform.position.y;

            if (!increment)
            {
                mySwitch.SetProgress((maxDist - dist) / maxDist);
            }
            else
            {
                mySwitch.AdjustProgress(dist / 10000f);
            }
        }
    }
}
