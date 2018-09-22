using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class DamageDealer : MonoBehaviour
    {
        /// <summary>
        /// The amount of damage dealt on impact
        /// </summary>
        public int damage = 1;

        [SerializeField]
        private bool _damageCharacters;

        [SerializeField]
        private bool _damageDestructibleObjects;

        [SerializeField]
        private bool _deactivateOnCollision;

        /// <summary>
        /// Handles colliding with objects that can take damage.
        /// </summary>
        /// <param name="collision">The collision</param>
        private void OnCollisionEnter(Collision collision)
        {
            Collider immediateCollider = collision.contacts[0].otherCollider;

            if (_damageCharacters)
            {
                GameCharacter character = immediateCollider.gameObject.GetComponent<GameCharacter>();
                DealDamage(character);
            }
            if (_damageDestructibleObjects)
            {
                Destructible destructible = collision.gameObject.GetComponent<Destructible>();
                DealDamage(destructible);
            }

            if (_deactivateOnCollision)
            {
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Deals damage to a game character.
        /// </summary>
        /// <param name="character">A game character</param>
        /// <returns>Did the game character take damage.</returns>
        private bool DealDamage(GameCharacter character)
        {
            if (character != null)
            {
                return character.TakeDamage(damage);
            }

            return false;
        }

        /// <summary>
        /// Deals damage to a destructible object.
        /// </summary>
        /// <param name="destructible">A destructible object<param>
        /// <returns>Did the object take damage.</returns>
        private bool DealDamage(Destructible destructible)
        {
            if (destructible != null)
            {
                return destructible.TakeDamage(damage);
            }

            return false;
        }
    }
}
