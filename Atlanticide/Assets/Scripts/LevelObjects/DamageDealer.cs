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
        /// What type of target is hit if any
        /// </summary>
        public enum HitTarget
        {
            NULL,
            NOTHING,
            SHIELD,
            CHARACTER,
            DESTRUCTIBLE
        }

        private HitTarget Target;

        /// <summary>
        /// Returns the value of Target and sets Target = HitTarget.NULL
        /// </summary>
        /// <returns>value of Target</returns>
        public HitTarget GetTarget()
        {
            HitTarget temp = Target;
            Target = HitTarget.NULL;
            return temp;
        }

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

            if (_damageCharacters)
            {
                Debug.Log("damage characters");
                Shield shield = null;
                GameCharacter character = null;

                // find shield
                foreach(ContactPoint cp in collision.contacts) {
                    shield = cp.otherCollider.gameObject.GetComponent<Shield>();
                    character = cp.otherCollider.gameObject.GetComponent<GameCharacter>();
                    if(shield != null && character != null)
                    {
                        break;
                    }
                }

                if(shield == null)
                {
                    Debug.Log("shield null");
                }

                if(character == null)
                {
                    Debug.Log("character null");
                }

                // check shield hit
                hit = Hit(shield);

                // if not hit shield, check character hit
                if(!hit)
                {
                    hit = Hit(character);
                }
            }

            if (!hit && _damageDestructibleObjects)
            {
                Destructible destructible = collision.gameObject.GetComponent<Destructible>();
                hit = Hit(destructible);
            }

            if (!hit)
            {
                Target = HitTarget.NOTHING;
            }

            if (_deactivateOnCollision)
            {
                gameObject.SetActive(false);
            }

            Debug.Log("HitTarget: " + Target);
        }

        /// <summary>
        /// Calls hit reaction on shield
        /// </summary>
        /// <param name="shield">A shield of the game character</param>
        /// <returns>Was the shield hit</returns>
        private bool Hit(Shield shield)
        {
            if (shield == null)
            {
                Debug.Log("no shield");
                return false;
            }

            if(shield.enabled)
            {
                shield.Hit();
                Target = HitTarget.SHIELD;
                return true;
            }
            return false;
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
                Target = HitTarget.CHARACTER;
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
                Target = HitTarget.DESTRUCTIBLE;
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
