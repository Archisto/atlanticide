using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Atlanticide
{
    public class PlayerCharacter : GameCharacter
    {
        [SerializeField]
        private GameObject _pushBlock;

        [SerializeField]
        private GameObject _telegrab;

        [SerializeField, Range(0.01f, 2f)]
        private float _energyDrainSpeed;

        [SerializeField, Range(0.01f, 2f)]
        private float _energyRechargeSpeed;

        [SerializeField, Range(0.01f, 1f)]
        private float _minRechargedEnergy;

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
            _myWall = _pushBlock;
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
            Vector3 newPosition = transform.position;
            newPosition.x += direction.x * _speed * Time.deltaTime;
            newPosition.z += direction.y * _speed * Time.deltaTime;

            //transform.position = GetPositionOffWall(transform.position, newPosition);
            transform.position = newPosition;
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

            // Test

            // Push block
            //_pushBlock.SetActive(_useEnergy);

            // Telegrab
            _telegrab.SetActive(_useEnergy);
            GameManager.Instance.UpdateTelegrab(ID, _telegrab.transform, _useEnergy);
        }

        protected override void Die()
        {
            base.Die();
        }

        public override void Respawn()
        {
            base.Respawn();
            _energy = 1;
            _outOfEnergy = false;
            transform.position = new Vector3(0, 5, 0);
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
