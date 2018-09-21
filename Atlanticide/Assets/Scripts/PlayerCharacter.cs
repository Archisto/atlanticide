using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Atlanticide
{
    public class PlayerCharacter : GameCharacter
    {
        [SerializeField]
        private GameObject _pushBeam;

        [SerializeField]
        private GameObject _telegrab;

        [SerializeField, Range(0.01f, 2f)]
        private float _energyDrainSpeed;

        [SerializeField, Range(0.01f, 2f)]
        private float _energyRechargeSpeed;

        [SerializeField, Range(0.01f, 1f)]
        private float _minRechargedEnergy;

        private Weapon _weapon;
        private bool _useEnergy; // TODO: Use for what?
        private bool _outOfEnergy;
        private float _energy = 1;

        public int ID { get; set; }

        public PlayerInput Input { get; set; }

        public Slider EnergyBar { get; set; }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            _myWall = _pushBeam;
            _weapon = GetComponentInChildren<Weapon>();
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        protected override void Update()
        {
            base.Update();

            if (IsDead)
            {
                return;
            }

            if (EnergyBar != null)
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
            if (!IsImmobile)
            {
                Vector3 newPosition = transform.position;
                newPosition.x += direction.x * _speed * Time.deltaTime;
                newPosition.z += direction.y * _speed * Time.deltaTime;

                if (_isRising)
                {
                    transform.position = newPosition;
                }
                else
                {
                    float groundHeightDiff = GroundHeightDifference(newPosition);

                    if (groundHeightDiff < 0.5 * _characterSize.y)
                    {
                        if (groundHeightDiff > -0.1 * _characterSize.y &&
                            groundHeightDiff < 0.2 * _characterSize.y)
                        {
                            newPosition.y += groundHeightDiff;
                        }

                        //transform.position = GetPositionOffWall(transform.position, newPosition);
                        transform.position = newPosition;
                    }
                }
            }
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
            _energy = Utils.DrainOrRecharge(_energy, _useEnergy, _energyDrainSpeed,
                _energyRechargeSpeed, _minRechargedEnergy, _outOfEnergy, out _outOfEnergy);

            if (EnergyBar != null)
            {
                EnergyBar.value = _energy;
            }
        }

        public void SpendEnergy(bool active)
        {
            // TODO: For what?
            _useEnergy = active && !_outOfEnergy;

            // Push beam
            //_pushBeam.SetActive(_useEnergy);

            // Telegrab
            _telegrab.SetActive(_useEnergy);
            GameManager.Instance.UpdateTelegrab(ID, _telegrab.transform, _useEnergy);
        }

        public void FireWeapon()
        {
            _weapon.Fire();
        }

        protected override void Die()
        {
            base.Die();

            // Testing
            Respawn();
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

            if (_useEnergy)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(_telegrab.transform.position, World.Instance.telegrabRadius);
            }
        }
    }
}
