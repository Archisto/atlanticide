using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class DamageDealer : MonoBehaviour
    {
        public int damage = 1;

        [SerializeField]
        private bool _continuousDamage;

        [SerializeField]
        private bool _deactivateOnCollision;

        /// <summary>
        /// Handles colliding with objects that can take damage.
        /// </summary>
        /// <param name="collision">The collision</param>
        private void OnCollisionEnter(Collision collision)
        {
            GameCharacter character = collision.gameObject.GetComponent<GameCharacter>();
            DealDamage(character);

            if (_deactivateOnCollision)
            {
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Deals damage to a game character.
        /// </summary>
        /// <param name="character">The game character</param>
        /// <returns>Did the game character take damage.</returns>
        private bool DealDamage(GameCharacter character)
        {
            if (character != null)
            {
                return character.TakeDamage(damage);
            }

            return false;
        }
    }
}
