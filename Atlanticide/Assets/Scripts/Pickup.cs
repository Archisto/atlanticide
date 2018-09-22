using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class Pickup : MonoBehaviour
    {
        [SerializeField]
        private int _score = 100;

        /// <summary>
        /// Handles colliding with the player characters.
        /// </summary>
        /// <param name="collision">The collision</param>
        private void OnCollisionEnter(Collision collision)
        {
            PlayerCharacter pc = collision.gameObject.GetComponent<PlayerCharacter>();
            if (pc != null)
            {
                Collect(pc);
            }
        }

        /// <summary>
        /// Gives score and destroys the pickup.
        /// </summary>
        /// <param name="character">A player character</param>
        private void Collect(PlayerCharacter character)
        {
            GameManager.Instance.UpdateScore(_score);
            gameObject.SetActive(false);
        }
    }
}
