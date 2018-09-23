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

        [SerializeField]
        private GameObject _shield;

        [SerializeField, Range(0.2f, 20f)]
        private float _jumpHeight = 1f;

        [SerializeField, Range(0.1f, 20f)]
        private float _climbSpeed = 1f;

        [SerializeField, Range(0.01f, 2f)]
        private float _energyDrainSpeed;

        [SerializeField, Range(0.01f, 2f)]
        private float _energyRechargeSpeed;

        [SerializeField, Range(0.01f, 1f)]
        private float _minRechargedEnergy;

        [SerializeField]
        private float _respawnTime = 1f;

        private Weapon _weapon;
        private Climbable _climbable;
        private bool _jumping;
        private bool _onGround;
        private bool _abilityActive;
        private bool _outOfEnergy;
        private float _energy = 1;
        private float _jumpForce;
        private float _elapsedRespawnTime;

        public int ID { get; set; }

        public PlayerInput Input { get; set; }

        public Slider EnergyBar { get; set; }

        public bool Climbing { get; private set; }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            ResetBaseValues();
            _myWall = _pushBeam;
            _weapon = GetComponentInChildren<Weapon>();
        }

        protected override void ResetBaseValues()
        {
            base.ResetBaseValues();
            _jumping = false;
            SetAbilityActive(false);
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        protected override void Update()
        {
            base.Update();

            if (IsDead)
            {
                UpdateRespawnTimer();
            }
            else
            {
                UpdateJump();

                if (EnergyBar != null)
                {
                    UpdateEnergy();
                }
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
                if (!Climbing)
                {
                    Move(direction);
                }
                else
                {
                    Climb(direction);
                }
            }
        }

        private void Move(Vector3 direction)
        {
            Vector3 movement = new Vector3(direction.x, 0, direction.y) * _speed * Time.deltaTime;
            Vector3 newPosition = transform.position + movement;

            if (_isRising || _jumping)
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

                        newPosition = transform.position + movement;

                        //newPosition =
                        //    transform.position +
                        //    new Vector3(direction.x, groundHeightDiff, direction.y).normalized * _speed * Time.deltaTime;
                        //newPosition.y = transform.position.y + groundHeightDiff;

                        _onGround = true;
                    }

                    //transform.position = GetPositionOffWall(transform.position, newPosition);
                    transform.position = newPosition;
                }
            }
        }

        private void Climb(Vector3 direction)
        {
            Vector3 movement = Vector3.zero;
            movement.y = direction.y * _climbSpeed * Time.deltaTime;
            float climbProgress = _climbable.GetClimbProgress(transform.position + movement);

            Debug.Log(climbProgress);

            transform.position = _climbable.GetPositionOnClimbable(climbProgress);

            if ((climbProgress >= 1 && direction.y > 0) || (climbProgress <= 0 && direction.y < 0))
            {
                EndClimb();
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
        /// Makes the character fall.
        /// </summary>
        protected override void Fall()
        {
            if (!_jumping && !Climbing)
            {
                base.Fall();
                _onGround = false;
            }
        }

        /// <summary>
        /// Makes the player character jump.
        /// </summary>
        public void Jump()
        {
            if ((!_jumping && _onGround) || Climbing)
            {
                if (Climbing)
                {
                    EndClimb();
                }

                _jumping = true;
                _jumpForce = _jumpHeight * 4;
                _onGround = false;
            }
        }

        /// <summary>
        /// Updates jumping.
        /// </summary>
        private void UpdateJump()
        {
            if (_jumping)
            {
                if (_jumpForce > 0)
                {
                    Rise(_jumpForce);
                    _jumpForce -= 5 * World.Instance.gravity * Time.deltaTime;
                }

                if (_jumpForce <= 0)
                {
                    _jumpForce = 0;
                    _jumping = false;
                }
            }
        }

        /// <summary>
        /// Updates the player's energy.
        /// </summary>
        private void UpdateEnergy()
        {
            _energy = Utils.DrainOrRecharge(_energy, _abilityActive, _energyDrainSpeed,
                _energyRechargeSpeed, _minRechargedEnergy, _outOfEnergy, out _outOfEnergy);

            if (_outOfEnergy)
            {
                SetAbilityActive(false);
            }

            if (EnergyBar != null)
            {
                EnergyBar.value = _energy;
            }
        }

        /// <summary>
        /// Updates the respawn timer.
        /// </summary>
        private void UpdateRespawnTimer()
        {
            _elapsedRespawnTime += Time.deltaTime;
            if (_elapsedRespawnTime >= _respawnTime)
            {
                _elapsedRespawnTime = 0;
                TryRespawn();
            }
        }

        public void UseAbility(bool active)
        {
            _abilityActive = active && !_outOfEnergy;
            SetAbilityActive(_abilityActive);
        }

        private void SetAbilityActive(bool active)
        {
            // Push beam
            //_pushBeam.SetActive(active);

            // Telegrab
            //_telegrab.SetActive(active);
            //GameManager.Instance.UpdateTelegrab(ID, _telegrab.transform, active);

            // Shield
            _shield.SetActive(active);
        }

        /// <summary>
        /// Fires the player character's weapon.
        /// </summary>
        public void FireWeapon()
        {
            if (!_abilityActive)
            {
                _weapon.Fire();
            }
        }

        public void StartClimb(Climbable climbable)
        {
            Debug.Log(name + " started climbing " + climbable + ".");
            Climbing = true;
            _climbable = climbable;
        }

        public void EndClimb()
        {
            Debug.Log(name + " stopped climbing.");
            Climbing = false;
            _climbable = null;
        }

        /// <summary>
        /// Kills the character.
        /// </summary>
        protected override void Die()
        {
            base.Die();
            SetAbilityActive(false);
        }

        public void TryRespawn()
        {
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

        /// <summary>
        /// Respawns the character.
        /// </summary>
        /// <param name="npc">The NPC that takes the player character's place.</param>
        public void Respawn(NonPlayerCharacter npc)
        {
            // Promotes an NPC to a player character
            _respawnPosition = npc.transform.position;
            npc.PromoteToPlayer();

            base.Respawn();
            _energy = 1;
            _outOfEnergy = false;
        }

        protected override bool CheckGroundCollision(Vector3 position, bool currPos)
        {
            bool result = base.CheckGroundCollision(position, currPos);

            if (result && !_jumping)
            {
                _onGround = true;
            }

            return result;
        }

        public override void CancelActions()
        {
            base.CancelActions();
            _jumping = false;
            UseAbility(false);
            EndClimb();
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (IsDead)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, 1);
            }
            else if (_abilityActive)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(_telegrab.transform.position, World.Instance.telegrabRadius);
            }
        }
    }
}
