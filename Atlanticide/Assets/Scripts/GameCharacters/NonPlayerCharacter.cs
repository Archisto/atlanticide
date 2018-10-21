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
                Vector3 avoidVector = _avoidOtherCharDir * _speed * World.Instance.DeltaTime;
                Vector3 newPosition = transform.position + avoidVector;
                _avoidDist += avoidVector.magnitude;

                if (_groundCollider.GroundHeightDifference(newPosition) <= -10 || _avoidDist > _maxAvoidDist)
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

        public void PromoteToPlayer()
        {
            Kill();
        }

        /// <summary>
        /// Kills the character.
        /// </summary>
        public override void Kill()
        {
            base.Kill();
            _touchedByOtherChar = false;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Resets the character's base values.
        /// </summary>
        protected override void ResetBaseValues()
        {
            base.ResetBaseValues();
            _touchedByOtherChar = false;
        }

        private void OnCollisionEnter(Collision collision)
        {
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
