using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Atlanticide.Persistence;
using System;

namespace Atlanticide
{
    public enum PlayerTool
    {
        None = 0,
        EnergyCollector = 1,
        Shield = 2
    }

    public class PlayerCharacter : GameCharacter, ISavable
    {
        [SerializeField, Range(0.2f, 20f)]
        private float _jumpHeight = 1f;

        [SerializeField, Range(0.1f, 20f)]
        private float _climbSpeed = 1f;

        [SerializeField]
        private float _respawnTime = 1f;

        private Weapon _weapon;
        private Climbable _climbable;
        private Interactable _interactionTarget;
        private float _jumpForce;
        private float _elapsedRespawnTime;

        public int ID { get; set; }

        public PlayerInput Input { get; set; }

        public PlayerTool Tool { get; set; }

        public EnergyCollector EnergyCollector { get; private set; }

        public Shield Shield { get; private set; }

        public Interactable InteractionTarget
        {
            get
            {
                return _interactionTarget;
            }
            set
            {
                _interactionTarget = value;
                GameManager.Instance.
                    ActivateTargetIcon(_interactionTarget != null, ID, _interactionTarget);
            }
        }

        public bool Jumping { get; private set; }

        public bool Climbing { get; private set; }

        public bool Pushing { get; private set; }

        public bool Respawning { get; set; }

        #region Tool States

        public bool ToolIsIdle
        {
            get
            {
                return EnergyCollectorIsIdle || ShieldIsIdle; 
            }
        }

        public bool EnergyCollectorIsIdle
        {
            get
            {
                return (Tool == PlayerTool.EnergyCollector && EnergyCollector.IsIdle);
            }
        }

        public bool EnergyCollectorIsEmitting
        {
            get
            {
                return (Tool == PlayerTool.EnergyCollector && EnergyCollector.IsEmitting);
            }
        }

        public bool EnergyCollectorIsDraining
        {
            get
            {
                return (Tool == PlayerTool.EnergyCollector && EnergyCollector.IsDraining);
            }
        }

        public bool ShieldIsIdle
        {
            get
            {
                return (Tool == PlayerTool.Shield && Shield.IsIdle);
            }
        }

        public bool ShieldIsActive
        {
            get
            {
                return (Tool == PlayerTool.Shield && Shield.Active);
            }
        }

        public bool ShieldBlocksDamage
        {
            get
            {
                return (Tool == PlayerTool.Shield && Shield.BlocksDamage);
            }
        }

        #endregion Tool States

        public bool IsAvailableForActions()
        {
            return (!IsDead && !IsImmobile && 
                    !Climbing && !Pushing && ToolIsIdle);
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            _weapon = GetComponentInChildren<Weapon>();
            EnergyCollector = GetComponentInChildren<EnergyCollector>();
            Shield = GetComponentInChildren<Shield>();
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
        /// <param name="input">Movement input</param>
        private void Move(Vector3 input)
        {
            Vector3 movement = new Vector3(input.x, 0, input.y) * _speed * World.Instance.DeltaTime;
            Vector3 newPosition = transform.position + movement * (Pushing ? 0.3f : 1f);
            transform.position = newPosition;

            if (!ShieldIsActive)
            {
                RotateTowards(input);
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
                SFXPlayer.Instance.Play(Sound.Jump_1);
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

        #region Input

        /// <summary>
        /// Moves the player character.
        /// </summary>
        /// <param name="input">The moving input</param>
        public void MoveInput(Vector3 input)
        {
            if (!IsImmobile)
            {
                if (Climbing)
                {
                    Climb(input);
                }
                else
                {
                    Move(input);
                }
            }
        }

        /// <summary>
        /// Rotates the player character.
        /// </summary>
        /// <param name="input">The looking direction</param>
        public void LookInput(Vector3 input)
        {
            if (!ShieldIsIdle)
            {
                RotateTowards(input);
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
            bool inputHeld = false;
            bool inputjustReleased = false;
            bool active = Input.GetActionInput(out inputHeld, out inputjustReleased);
            bool result = false;

            if (Tool == PlayerTool.EnergyCollector && active)
            {
                // Drain
                result = UseEnergyCollector(drain: true);
            }
            else if (Tool == PlayerTool.Shield)
            {
                // Open the shield if the action button is held down
                if (inputHeld && !Shield.Active)
                {
                    Shield.Activate(true);
                }
                // Open or close the shield if pressed once,
                // close if released after holding
                else if (inputjustReleased)
                {
                    Shield.Activate(!Shield.Active);
                }

                result = !Shield.IsIdle;
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
        /// Handles changing the player character's stance.
        /// Only the player with shield can change stance
        /// by raising the shield above his/her head.
        /// </summary>
        /// <returns>Does the stance change</returns>
        public bool HandleStanceInput()
        {
            if (Tool == PlayerTool.Shield &&
                (Shield.BlocksDamage || Shield.Raised))
            {
                bool input = Input.GetStanceInput();
                if (input)
                {
                    Shield.RaiseAboveHead(!Shield.Raised);
                    return true;
                }
            }

            return false;
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
                        if (InteractionTarget.EnergyCost != 0)
                        {
                            // Removes the energy cost from the players
                            EnergyCollector.ChangeCharges(-1 * InteractionTarget.EnergyCost);
                        }

                        // Makes the interaction target forget the player
                        InteractionTarget.TryRemoveInteractorAfterInteraction();

                        return true;
                    }
                }
            }

            return false;
        }

        public bool CheckToolSwapInput()
        {
            bool input = Input.GetToolSwapInput();
            return (input && ToolIsIdle);
        }

        #endregion Input

        /// <summary>
        /// Uses the energy collector for draining or emitting energy.
        /// Returns whether succeeded in draining or emitting energy.
        /// </summary>
        /// <param name="drain">Should energy be drained</param>
        /// <returns>Is draining or emitting energy successful</returns>
        public bool UseEnergyCollector(bool drain)
        {
            bool result = false;
            if (EnergyCollector != null)
            {
                if (drain)
                {
                    result = EnergyCollector.TryDraining();
                }
                else
                {
                    result = EnergyCollector.TryEmitting();
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

                SFXPlayer.Instance.Play(Sound.Climbing_Slower);
            }
        }

        public void EndClimb()
        {
            if (Climbing)
            {
                Debug.Log(name + " stopped climbing");
                Climbing = false;
                _climbable = null;

                SFXPlayer.Instance.StopAllSFXPlayback();
            }
        }

        private void Climb(Vector3 input)
        {
            Vector3 movement = Vector3.zero;
            movement.y = input.y * _climbSpeed * World.Instance.DeltaTime;
            float climbProgress = _climbable.GetClimbProgress(transform.position + movement);

            transform.position = _climbable.GetPositionOnClimbable(climbProgress);

            if ((climbProgress >= 1 && input.y > 0) || (climbProgress <= 0 && input.y < 0))
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
            Respawning = false;
            _elapsedRespawnTime = 0f;
            
            if (EnergyCollector != null)
            {
                EnergyCollector.ResetEnergyCollector();
            }

            if (Shield != null)
            {
                Shield.ResetShield();
            }
        }

        /// <summary>
        /// Cancels any running actions if the player dies or the level is reset.
        /// </summary>
        public override void CancelActions()
        {
            base.CancelActions();
            Input.ResetInput();
            Jumping = false;
            Respawning = false;
            EnergyCollector.ResetEnergyCollector();
            Shield.ResetShield();
            EndClimb();
            EndPush();
            InteractionTarget = null;
        }

        #region Persistence

        /// <summary>
        /// Returns the object's save data.
        /// </summary>
        public ISaveData GetSaveData()
        {
            return new PlayerData
            {
                ID = ID,
                Tool = Tool
            };
        }

        /// <summary>
        /// Sets the object's values from the save data.
        /// </summary>
        /// <param name="data">Save data</param>
        public void SetData(ISaveData data)
        {
            PlayerData playerData = (PlayerData) data;
            if (playerData != null)
            {
                Tool = playerData.Tool;
            }
        }

        /// <summary>
        /// Returns the correct save data type.
        /// </summary>
        /// <returns>The save data type</returns>
        public Type GetSaveDataType()
        {
            return typeof(PlayerData);
        }

        #endregion Persistence

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
