using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class Destructible : LevelObject
    {
        [SerializeField]
        protected int _maxHitpoints = 3;

        protected int _hitpoints;

        public bool Destroyed { get; protected set; }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _defaultPosition = transform.position;
            ResetObject();
        }

        /// <summary>
        /// Makes the object take damage.
        /// </summary>
        /// <param name="damage">The damage amount</param>
        /// <returns>Is the object destroyed.</returns>
        public virtual bool TakeDamage(int damage)
        {
            _hitpoints -= damage;

            if (_hitpoints <= 0)
            {
                _hitpoints = 0;
                Destroy();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Destroys the object.
        /// </summary>
        public override void Destroy()
        {
            Debug.Log(name + " was destroyed.");
            Destroyed = true;
            base.Destroy();
        }


        /// <summary>
        /// Resets the object to its default state.
        /// </summary>
        public override void ResetObject()
        {
            Destroyed = false;
            gameObject.SetActive(true);
            _hitpoints = _maxHitpoints;
            SetToDefaultPosition();
        }

        /// <summary>
        /// Draws gizmos.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!Destroyed)
            {
                Gizmos.color = Color.red;

                for (int i = 0; i < _hitpoints; i++)
                {
                    Vector3 position = transform.position + Vector3.up * 3f;
                    position.x += (i - _maxHitpoints / 2) * 0.8f;
                    Gizmos.DrawSphere(position, 0.3f);
                }
            }
        }
    }
}
