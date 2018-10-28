using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class DamageDealer : MonoBehaviour
    {
        private const string InvisibleKey = "Invisible";

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

            // Passes through invisible walls
            if (collision.gameObject.tag == InvisibleKey)
            {
                return;
            }

            // is something hit
            bool hit = false;
            bool dontDestroy = false;
            Collider immediateCollider = collision.contacts[0].otherCollider;

            if (_damageCharacters)
            {
                GameCharacter character = immediateCollider.
                    transform.parent.GetComponent<GameCharacter>();
                Shield shield = immediateCollider.transform.GetComponent<Shield>();

                if (character != null && shield == null)
                {
                    dontDestroy = character.IsDead; // TODO: Replace with something better
                    hit = Hit(character);
                }

                //Debug.Log("immediateCollider.gameObject: " + immediateCollider.gameObject.name);
            }

            if (!hit && _damageDestructibleObjects)
            {
                Destructible destructible = immediateCollider.gameObject.GetComponent<Destructible>();
                hit = Hit(destructible);
            }

            if (_deactivateOnCollision && !dontDestroy)
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

        /// <summary>
        /// Draws the collider of the object
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Collider col = GetComponent<Collider>();

            if(col == null)
            {
                return;
            }
            
            if(col.GetType() == typeof(SphereCollider))
            {
                Gizmos.DrawWireSphere(col.bounds.center, col.bounds.size.x/2);
            } else
            {
                Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
            }
        }

    }
}
