using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Atlanticide
{
    public class PlayerCharacter : GameCharacter
    {
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
        private EnergyCollector _energyCollector;
        private Climbable _climbable;
        //private Pushable _pushable;
        private bool _jumping;
        private bool _abilityActive;
        private bool _outOfEnergy;
        private float _energy = 1;
        private float _jumpForce;
        private float _elapsedRespawnTime;

        public int ID { get; set; }

        public PlayerInput Input { get; set; }

        public Slider EnergyBar { get; set; }

        public EnergyCollector EnergyCollector { get { return _energyCollector; } }

        public bool Climbing { get; private set; }

        public bool Pushing { get; private set; }


        public bool IsAvailableForActions()
        {
            return (!IsDead && !IsImmobile && 
                    !Climbing && !Pushing);
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            _weapon = GetComponentInChildren<Weapon>();
            _energyCollector = GetComponentInChildren<EnergyCollector>();

            if (_energyCollector != null)
            {
                UpdateEnergyBar();
            }
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
                if (Climbing)
                {
                    Climb(direction);
                }
                //else if (Pushing)
                //{
                //    Push(direction);
                //}
                else
                {
                    Move(direction);
                }
            }
        }

        private void Move(Vector3 direction)
        {
            Vector3 movement = new Vector3(direction.x, 0, direction.y) * _speed * World.Instance.DeltaTime;
            Vector3 newPosition = transform.position + movement * (Pushing ? 0.3f : 1f);

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
                        //    new Vector3(direction.x, groundHeightDiff, direction.y).normalized * _speed * World.Instance.DeltaTime;
                        //newPosition.y = transform.position.y + groundHeightDiff;

                        _onGround = true;
                    }

                    //transform.position = GetPositionOffWall(transform.position, newPosition);
                    transform.position = newPosition;
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

        protected override bool CheckGroundCollision(Vector3 position, bool currPos)
        {
            bool result = base.CheckGroundCollision(position, currPos);

            if (result && !_jumping)
            {
                _onGround = true;
            }

            return result;
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
                if (Pushing)
                {
                    EndPush();
                }

                _jumping = true;
                _jumpForce = _jumpHeight * 4;
                _onGround = false;
                SFXPlayer.Instance.Play(Sound.Jump);
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
                    _jumpForce -= 5 * World.Instance.gravity * World.Instance.DeltaTime;
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

            //UpdateEnergyBar();
        }

        private void UpdateEnergyBar()
        {
            // TODO: Decide how the energy bar is used or even if at all.

            if (EnergyBar != null && _energyCollector != null)
            {
                //EnergyBar.value = _energy;
                EnergyBar.value = ((float) _energyCollector.CurrentCharges / _energyCollector.MaxCharges);
            }
        }

        /// <summary>
        /// Updates the respawn timer.
        /// </summary>
        private void UpdateRespawnTimer()
        {
            _elapsedRespawnTime += World.Instance.DeltaTime;
            if (_elapsedRespawnTime >= _respawnTime)
            {
                //TryRespawnToNPC(); // Only if the game has NPCs the player takes control of
                Respawn();
            }
        }

        public void UseAbility(bool active)
        {
            _abilityActive = active && !_outOfEnergy;
            SetAbilityActive(_abilityActive);
        }

        private void SetAbilityActive(bool active)
        {
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
            if (_weapon != null && !_abilityActive)
            {
                _weapon.Fire();
            }
        }

        /// <summary>
        /// Uses the energy collector.
        /// </summary>
        public void UseEnergyCollector()
        {
            if (_energyCollector != null && !_abilityActive)
            {
                _energyCollector.StartChargingOrEmitting();
                UpdateEnergyBar();
            }
        }

        public void UpdateClosestEnergyNode(EnergyNode node)
        {
            _energyCollector.SetTarget(node);
        }

        public void StartClimb(Climbable climbable)
        {
            if (!Climbing)
            {
                Debug.Log(name + " started climbing " + climbable.name + ".");
                Climbing = true;
                _climbable = climbable;
            }
        }

        public void EndClimb()
        {
            if (Climbing)
            {
                Debug.Log(name + " stopped climbing.");
                Climbing = false;
                _climbable = null;
            }
        }

        private void Climb(Vector3 direction)
        {
            Vector3 movement = Vector3.zero;
            movement.y = direction.y * _climbSpeed * World.Instance.DeltaTime;
            float climbProgress = _climbable.GetClimbProgress(transform.position + movement);

            transform.position = _climbable.GetPositionOnClimbable(climbProgress);

            if ((climbProgress >= 1 && direction.y > 0) || (climbProgress <= 0 && direction.y < 0))
            {
                EndClimb();
            }

            // TODO: The climbable knows which direction is up/forward on it;
            // use that information to determine the right input direction.

            /*
             Vector2 movement = direction * _climbSpeed *
                _climbable.InputForwardVector * World.Instance.DeltaTime;
            float climbProgress = _climbable.GetClimbProgress(transform.position + movement);

            transform.position = _climbable.GetPositionOnClimbable(climbProgress);
            bool goingForward = _climbable.ForwardDirection(direction);

            if ((climbProgress >= 1 && goingForward) || (climbProgress <= 0 && !goingForward))
            {
                EndClimb();
            }
             */
        }

        public void StartPush(Pushable pushable)
        {
            if (!Pushing)
            {
                // TODO: Use the pushable's weight

                Debug.Log(name + " started pushing " + pushable.name + ".");
                Pushing = true;
                //_pushable = pushable;
            }
        }

        public void EndPush()
        {
            if (Pushing)
            {
                Debug.Log(name + " stopped pushing.");
                Pushing = false;
                //_pushable.EndPush();
                //_pushable = null;
            }
        }

        //private void Push(Vector3 direction)
        //{
        //    direction = new Vector3(direction.x, 0, direction.y);
        //    //Debug.Log("dir: " + direction);
        //    //Debug.Log("pushDir: " + _pushable.PushDirection);
        //    //Debug.Log("angle: " + Vector3.Angle(direction, _pushable.PushDirection));
        //    if (Vector3.Angle(direction, _pushable.PushDirection) < 80)
        //    {
        //        _pushable.Move();

        //        float pushDist = 1f;
        //        Vector3 newPosition = _pushable.transform.position - _pushable.PushDirection * pushDist;
        //        newPosition.y = transform.position.y;
        //        transform.position = newPosition;
        //    }
        //    else
        //    {
        //        EndPush();
        //    }
        //}

        public void TryRespawnToNPC()
        {
            NonPlayerCharacter[] npcs = GameManager.Instance.GetNPCs();
            NonPlayerCharacter npc = Utils.GetFirstActiveOrInactiveObject(npcs, true) as NonPlayerCharacter;
            if (npc != null)
            {
                RespawnToNPC(npc);
            }
            else
            {
                Respawn();
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Respawns the player by promoting an NPC to a player character.
        /// </summary>
        /// <param name="npc">The NPC that takes the player character's place.</param>
        public void RespawnToNPC(NonPlayerCharacter npc)
        {
            // Promotes an NPC to a player character
            RespawnPosition = npc.transform.position;
            npc.PromoteToPlayer();
            Respawn();
        }

        /// <summary>
        /// Resets the player character's base values when respawning.
        /// </summary>
        protected override void ResetBaseValues()
        {
            base.ResetBaseValues();
            _jumping = false;
            _energy = 1f;
            _outOfEnergy = false;
            _elapsedRespawnTime = 0f;
            SetAbilityActive(false);
            
            if (_energyCollector != null)
            {
                _energyCollector.ResetEnergyCollector();
            }

            UpdateEnergyBar();
        }

        /// <summary>
        /// Cancels any running actions if the player dies or the level is reset.
        /// </summary>
        public override void CancelActions()
        {
            base.CancelActions();
            _jumping = false;
            SetAbilityActive(false);
            EndClimb();
            EndPush();
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
