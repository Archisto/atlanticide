using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Atlanticide
{
    public class PlayerCharacter : GameCharacter
    {
        [SerializeField]
        private Slider _energyBar;

        [SerializeField, Range(0.01f, 2f)]
        private float _energyDrainSpeed;

        [SerializeField, Range(0.01f, 2f)]
        private float _energyRechargeSpeed;

        [SerializeField, Range(0.01f, 1f)]
        private float _minRechargedEnergy;

        private bool _useEnergy; // TODO: Use for what?
        private bool _outOfEnergy;
        private float _energy = 1;
        private InputController _input;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            _input = FindObjectOfType<InputController>();
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        protected override void Update()
        {
            base.Update();

            if (_isDead)
            {
                return;
            }

            if (_energyBar != null)
            {
                UpdateEnergy();
            }
        }

        /// <summary>
        /// Moves the player character.
        /// </summary>
        /// <param name="direction">The moving direction</param>
        public void MoveInput(Vector3 direction)
        {
            Vector3 newPosition = transform.position;
            newPosition.x += direction.x * _speed * Time.deltaTime;
            newPosition.z += direction.y * _speed * Time.deltaTime;
            transform.position = newPosition;
            //Debug.LogFormat("X: {0}, Z: {1}", direction.x, direction.y);
        }

        /// <summary>
        /// Rotates the player character.
        /// </summary>
        /// <param name="direction">The looking direction</param>
        public void LookInput(Vector3 direction)
        {
            RotateTowards(direction);
        }

        /// <summary>
        /// Updates the player's energy.
        /// </summary>
        private void UpdateEnergy()
        {
            // Drain
            if (_useEnergy)
            {
                if (_energy > 0)
                {
                    _energy -= _energyDrainSpeed * Time.deltaTime;
                    if (_energy <= 0)
                    {
                        _energy = 0;
                        _outOfEnergy = true;
                    }

                    _energyBar.value = _energy;
                }
            }
            // Recharge
            else if (_energy < 1)
            {
                _energy += _energyRechargeSpeed * Time.deltaTime;

                if (_outOfEnergy && _energy >= _minRechargedEnergy)
                {
                    _outOfEnergy = false;
                }
                if (_energy > 1)
                {
                    _energy = 1;
                }

                _energyBar.value = _energy;
            }
        }

        public void SpendEnergy(bool active)
        {
            // TODO: For what?
            _useEnergy = active && !_outOfEnergy;
        }

        protected override void Die()
        {
        }

        public override void Respawn()
        {
            base.Respawn();
            _energy = 1;
            _outOfEnergy = false;
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
        }
    }
}
