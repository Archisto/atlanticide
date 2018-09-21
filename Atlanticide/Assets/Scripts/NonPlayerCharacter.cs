using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Atlanticide
{
    public class NonPlayerCharacter : GameCharacter
    {
        private bool _touchedByOtherChar;
        private Vector3 _avoidOtherCharDir;
        private float _maxAvoidDist = 1.3f;
        private float _avoidDist;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected override void Start()
        {
            base.Start();
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        protected override void Update()
        {
            base.Update();
            AvoidOtherCharacter();
        }

        private void AvoidOtherCharacter()
        {
            if (_touchedByOtherChar)
            {
                Vector3 avoidVector = _avoidOtherCharDir * _speed * Time.deltaTime;
                Vector3 newPosition = transform.position + avoidVector;
                _avoidDist += avoidVector.magnitude;

                if (!CheckGroundCollision(newPosition, false) || _avoidDist > _maxAvoidDist)
                {
                    _touchedByOtherChar = false;
                    _avoidDist = 0;
                }
                else
                {
                    transform.position = newPosition;
                }
            }
        }

        protected override void Die()
        {
            base.Die();
            _touchedByOtherChar = false;
            gameObject.SetActive(false);
        }

        public override void Respawn()
        {
            base.Respawn();
            gameObject.SetActive(true);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (Telegrabbed)
            {
                return;
            }

            GameCharacter character = collision.gameObject.GetComponent<GameCharacter>();
            if (character != null)
            {
                if (!_touchedByOtherChar)
                {
                    _avoidOtherCharDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
                    _avoidOtherCharDir = _avoidOtherCharDir * Random.Range(0.7f, 1.3f);
                }

                _touchedByOtherChar = true;
            }
        }
    }
}
