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
            bool hit = false;

            if (_damageCharacters)
            {
                GameCharacter character = immediateCollider.gameObject.GetComponent<GameCharacter>();
                hit = Hit(character);
            }

            if (!hit && _damageDestructibleObjects)
            {
                Destructible destructible = collision.gameObject.GetComponent<Destructible>();
                hit = Hit(destructible);
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
        /// <returns>Was the character hit.</returns>
        private bool Hit(GameCharacter character)
        {
            if (character != null && !character.IsDead)
            {
                character.TakeDamage(damage);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Deals damage to a destructible object.
        /// </summary>
        /// <param name="destructible">A destructible object<param>
        /// <returns>Was the object hit.</returns>
        private bool Hit(Destructible destructible)
        {
            if (destructible != null)
            {
                destructible.TakeDamage(damage);
                return true;
            }

            return false;
        }
    }
}
