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
                DestroyObject();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Destroys the object.
        /// </summary>
        public override void DestroyObject()
        {
            Debug.Log(name + " was destroyed.");
            Destroyed = true;
            gameObject.SetActive(false);
            base.DestroyObject();
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
                Utils.DrawHPGizmo(transform.position + Vector3.up * 3f,
                    _hitpoints, _maxHitpoints, Color.red);
            }
        }
    }
}
