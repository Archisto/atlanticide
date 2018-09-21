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
        private bool _destroyOnCollision;

        public bool damageDealt;

        /// <summary>
        /// Handles colliding with objects that can take damage.
        /// </summary>
        /// <param name="collision">The collision</param>
        private void OnCollisionEnter(Collision collision)
        {
            if (!damageDealt || _continuousDamage)
            {
                GameCharacter character = collision.gameObject.GetComponent<GameCharacter>();
                DealDamage(character);
            }

            if (_destroyOnCollision)
            {
                Destroy(gameObject);
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
                damageDealt = true;
                return character.TakeDamage(damage);
            }

            return false;
        }

        /// <summary>
        /// Reactivates the damage dealer if the damage has already been dealt.
        /// </summary>
        public void Reactivate()
        {
            damageDealt = false;
        }
    }
}
