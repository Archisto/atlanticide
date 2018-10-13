using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public enum PlayerTool
    {
        None = 0,
        EnergyCollector = 1,
        Shield = 2
    }

    public class PlayerCharacter : GameCharacter
    {
        [SerializeField]
        private Shield _shield;

        [SerializeField, Range(0.2f, 20f)]
        private float _jumpHeight = 1f;

        [SerializeField, Range(0.1f, 20f)]
        private float _climbSpeed = 1f;

        [SerializeField]
        private float _respawnTime = 1f;

        private Weapon _weapon;
        private EnergyCollector _energyCollector;
        private Climbable _climbable;
        private float _jumpForce;
        private float _elapsedRespawnTime;

        public int ID { get; set; }

        public PlayerInput Input { get; set; }

        public PlayerTool Tool { get; set; }

        public EnergyCollector EnergyCollector { get { return _energyCollector; } }

        public Shield Shield { get { return _shield; } }

        public Interactable InteractionTarget { get; set; }

        public bool Jumping { get; private set; }

        public bool Climbing { get; private set; }

        public bool Pushing { get; private set; }

        public bool ShieldIsActive
        {
            get
            {
                return (Tool == PlayerTool.Shield && _shield.Active);
            }
        }

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
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        protected override void Update()
        {
            base.Update();

            if (!IsDead)
            {
                UpdateJump();
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
            transform.position = newPosition;

            if (!ShieldIsActive)
            {
                RotateTowards(direction);
            }
        }

        /// <summary>
        /// Rotates the player character.
        /// </summary>
        /// <param name="direction">The looking direction</param>
        public void LookInput(Vector3 direction)
        {
            if (ShieldIsActive)
            {
                RotateTowards(direction);
            }
        }

        /// <summary>
        /// Makes the player character jump.
        /// </summary>
        /// <returns>Does the character jump</returns>
        public bool Jump()
        {
            if ((!Jumping && _groundCollider.onGround) || Climbing)
            {
                if (Climbing)
                {
                    EndClimb();
                }
                if (Pushing)
                {
                    EndPush();
                }

                Jumping = true;
                _jumpForce = _jumpHeight * 4;
                _groundCollider.onGround = false;
                SFXPlayer.Instance.Play(Sound.Jump);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Updates jumping.
        /// </summary>
        private void UpdateJump()
        {
            if (Jumping)
            {
                if (_jumpForce > 0)
                {
                    _groundCollider.Rise(_jumpForce);
                    _jumpForce -= 5 * World.Instance.gravity * World.Instance.DeltaTime;
                }

                if (_jumpForce <= 0)
                {
                    _jumpForce = 0;
                    Jumping = false;
                }
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
                Respawn();
            }
        }

        public bool HandleJumpInput()
        {
            if (Input.GetJumpInput())
            {
                return Jump();
            }

            return false;
        }

        public bool HandleActionInput()
        {
            bool active = Input.GetActionInput();
            bool result = false;

            if (Tool == PlayerTool.EnergyCollector && active)
            {
                // Drain
                result = UseEnergyCollector(drain: true);
            }
            else if (Tool == PlayerTool.Shield)
            {
                UseShield(active);
                result = active;
            }

            return result;
        }

        public bool HandleAltActionInput()
        {
            bool active = Input.GetAltActionInput();
            bool result = false;

            if (active)
            {
                if (Tool == PlayerTool.EnergyCollector)
                {
                    // Emit
                    result = UseEnergyCollector(drain: false);
                }
                else if (Tool == PlayerTool.Shield)
                {
                    result = Shield.Bash();
                }
            }

            return result;
        }

        /// <summary>
        /// Handles interacting with various level objects such as respawn points.
        /// </summary>
        /// <returns>Is the interaction successful</returns>
        public bool HandleInteractionInput()
        {
            bool input = Input.GetInteractInput();
            if (input && InteractionTarget != null)
            {
                // Checks whether the energy cost is not too high
                if (World.Instance.CurrentEnergyCharges >=
                    InteractionTarget.EnergyCost)
                {
                    if (InteractionTarget.Interact())
                    {
                        // Removes the energy cost from the players
                        EnergyCollector.SetCharges
                            (World.Instance.CurrentEnergyCharges - InteractionTarget.EnergyCost, true);

                        // Makes the interaction target forget the player
                        InteractionTarget.SetInteractorTarget(false, true);

                        return true;
                    }
                }
            }

            return false;
        }

        public bool CheckToolSwapInput()
        {
            bool input = Input.GetToolSwapInput();
            return (input && _energyCollector.IsIdle && _shield.IsIdle);
        }

        /// <summary>
        /// Opens or closes the shield.
        /// </summary>
        /// <param name="activate">Should the shield be activated</param>
        /// <returns></returns>
        private bool UseShield(bool activate)
        {
            return _shield.Activate(activate);
        }

        /// <summary>
        /// Uses the energy collector for draining or emitting energy.
        /// Returns whether succeeded in draining or emitting energy.
        /// </summary>
        /// <param name="drain">Should energy be drained</param>
        /// <returns>Is draining or emitting energy successful</returns>
        public bool UseEnergyCollector(bool drain)
        {
            bool result = false;
            if (_energyCollector != null)
            {
                if (drain)
                {
                    result = _energyCollector.TryDraining();
                }
                else
                {
                    result = _energyCollector.TryEmitting();
                }
            }

            return result;
        }

        /// <summary>
        /// Fires the player character's weapon.
        /// </summary>
        public void FireWeapon()
        {
            if (_weapon != null)
            {
                _weapon.Fire();
            }
        }

        public void StartClimb(Climbable climbable)
        {
            if (!Climbing)
            {
                Debug.Log(name + " started climbing " + climbable.name);
                Climbing = true;
                _climbable = climbable;
            }
        }

        public void EndClimb()
        {
            if (Climbing)
            {
                Debug.Log(name + " stopped climbing");
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

                Debug.Log(name + " started pushing " + pushable.name);
                Pushing = true;
                //_pushable = pushable;
            }
        }

        public void EndPush()
        {
            if (Pushing)
            {
                Debug.Log(name + " stopped pushing");
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

        public override void Kill()
        {
            base.Kill();
            GameManager.Instance.DeadPlayerCount++;
        }

        public override void Respawn()
        {
            ResetBaseValues();
            ResetPosition();
            GameManager.Instance.DeadPlayerCount--;
            Debug.Log(name + " respawned");
        }

        public void TryRespawnToNPC()
        {
            NonPlayerCharacter[] npcs = GameManager.Instance.GetNPCs();
            NonPlayerCharacter npc =
                Utils.GetFirstActiveOrInactiveObject(npcs, true) as NonPlayerCharacter;
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
            Jumping = false;
            _elapsedRespawnTime = 0f;
            UseShield(false);
            
            if (_energyCollector != null)
            {
                _energyCollector.ResetEnergyCollector();
            }
        }

        /// <summary>
        /// Cancels any running actions if the player dies or the level is reset.
        /// </summary>
        public override void CancelActions()
        {
            base.CancelActions();
            Jumping = false;
            _shield.ResetShield();
            _energyCollector.ResetEnergyCollector();
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
        }
    }
}
