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
        private bool _useEnergy;
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
                Vector3 movement = new Vector3(direction.x, 0, direction.y) * _speed * Time.deltaTime;
                Vector3 newPosition = transform.position + movement;

                if (_isRising)
                {
                    transform.position = newPosition;
                }
                else
                {
                    float groundHeightDiff = GroundHeightDifference(newPosition);

                    // If the slope is too steep upwards, the character doesn't move
                    if (groundHeightDiff < 0.5f * _characterSize.y)
                    {
                        // If the slope is too steep upwards or downwards, the height difference is ignored.
                        // Slopes that are too steep upwards are handled with the Rise method.
                        // Super minimal height differences are also ignored.
                        if (groundHeightDiff > -0.1f * _characterSize.y &&
                            groundHeightDiff < 0.2f * _characterSize.y &&
                            (groundHeightDiff < -0.0001f * _characterSize.y ||
                            groundHeightDiff > 0.0001f * _characterSize.y))
                        {
                            movement.y = groundHeightDiff;
                            float maxGroundHeiDiff = (groundHeightDiff > 0 ? 0.2f : -0.1f) * _characterSize.y;
                            float ratio = Utils.ReverseRatio(groundHeightDiff, 0, maxGroundHeiDiff);
                            ratio = (ratio < 0.4f ? 0.4f : ratio);

                            // Very steep slope: groundHeightDiff = +-0.03
                            //float slopeSpeedDampening = Utils.Ratio(Mathf.Abs(groundHeightDiff), 0, 1f);
                            //float slopeSpeedDampening = (Mathf.Abs(groundHeightDiff) > 0.07f ? 0.25f : 0.1f);
                            //float slopeSpeedDampening = 0f;

                            movement.x = movement.x * ratio;
                            //movement.x = Utils.WeighValue(movement.x, 0, slopeSpeedDampening);
                            movement.z = movement.z * ratio;
                            //movement.z = Utils.WeighValue(movement.z, 0, slopeSpeedDampening);
                            GameManager.Instance.SetScore((int) (groundHeightDiff * 100));

                            newPosition = transform.position + movement;

                            //newPosition =
                            //    transform.position +
                            //    new Vector3(direction.x, groundHeightDiff, direction.y).normalized * _speed * Time.deltaTime;
                            //newPosition.y = transform.position.y + groundHeightDiff;
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

            NonPlayerCharacter[] npcs = GameManager.Instance.GetNPCs();
            NonPlayerCharacter npc = Utils.GetFirstActiveOrInactiveObject(npcs, true) as NonPlayerCharacter;
            if (npc != null)
            {
                Respawn(npc);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void Respawn(NonPlayerCharacter npc)
        {
            // Promotes an NPC to a player character
            _respawnPosition = npc.transform.position;
            npc.PromoteToPlayer();

            base.Respawn();
            _energy = 1;
            _outOfEnergy = false;
        }

        public override void CancelActions()
        {
            base.CancelActions();
            SpendEnergy(false);
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
