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

    public class PlayerCharacter : GameCharacter, ILinkTarget
    {
        [SerializeField, Range(0.2f, 20f)]
        private float _jumpHeight = 1f;

        [SerializeField, Range(0.1f, 20f)]
        private float _climbSpeed = 1f;

        [SerializeField]
        private float _respawnTime = 1f;

        PlayerCharacter _otherPlayer;
        private Vector3 _size;
        private Climbable _climbable;
        private Interactable _interactionTarget;
        private float _jumpForce;
        private float _elapsedRespawnTime;

        public int ID { get; set; }

        public PlayerInput Input { get; set; }

        public PlayerTool Tool { get; set; }

        public EnergyCollector EnergyCollector { get; private set; }

        public Shield Shield { get; private set; }

        /// <summary>
        /// The player's link beam.
        /// </summary>
        public LinkBeam LinkBeam { get; set; }

        /// <summary>
        /// The link beam's game object, the object
        /// with which a link can be established.
        /// </summary>
        public GameObject LinkObject { get; set; }

        /// <summary>
        /// The link beam which is currently
        /// linked to the player's link beam.
        /// </summary>
        public LinkBeam LinkedLinkBeam { get; set; }

        /// <summary>
        /// Is the player currently a link target.
        /// </summary>
        public bool IsLinkTarget { get; set; }

        /// <summary>
        /// Is either the player's link beam
        /// activated or the player a link target.
        /// </summary>
        public bool BeamOn
        {
            get { return LinkBeam.Active || IsLinkTarget; }
        }

        public Animator Animator { get; private set; }

        public GameObject ShieldModel;

        public Animator ShieldAnimator { get; private set; }

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

        private bool CanJump()
        {
            bool restrictions = Jumping || !_groundCollider.AbleToJump ||
                !Shield.IsIdle;
            bool permissions = Climbing;
            return !restrictions || permissions;
        }

        private bool CanClimb()
        {
            bool restrictions = Climbing || !EnergyCollector.IsIdle ||
                !Shield.IsIdle;
            return !restrictions;
        }

        private bool CanUseEnergyCollector()
        {
            bool restrictions = Climbing;
            return !restrictions;
        }

        private bool CanActivateShield()
        {
            bool restrictions = Climbing || Shield.Active;
            return !restrictions;
        }

        private bool CanActivateLink()
        {
            bool restrictions = Climbing || LinkBeam.Active;
            return !restrictions;
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            EnergyCollector = GetComponentInChildren<EnergyCollector>();
            Shield = GetComponentInChildren<Shield>();
            LinkBeam = GetComponentInChildren<LinkBeam>();
            LinkBeam.Init(this, Shield.Activate);
            LinkObject = LinkBeam.gameObject;
            Animator = GetComponentInChildren<Animator>();
            ShieldAnimator = ShieldModel.GetComponent<Animator>();
            _otherPlayer = GameManager.Instance.GetAnyOtherPlayer(this, true);
        }

        #region Updating

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        protected override void Update()
        {
            base.Update();

            if (!IsDead)
            {
                UpdateJump();
                UpdateRotation();
                UpdateShield();
            }
        }

        /// <summary>
        /// Moves the player character.
        /// </summary>
        /// <param name="input">Movement input</param>
        private void Move(Vector3 input)
        {
            float speed = _speed;

            if (!Shield.IsIdle)
            {
                speed = _speed * Shield.PlayerSpeedModifier;
            }
            else if (!LinkBeam.Active && !IsLinkTarget)
            {
                RotateTowards(input, true);
            }

            Vector3 movement = new Vector3(input.x, 0, input.y) * speed * World.Instance.DeltaTime;
            Vector3 newPosition = transform.position + movement * (Pushing ? 0.3f : 1f);
            transform.position = newPosition;

            Vector3 inputDirection = input.normalized;
            inputDir = new Vector3(inputDirection.x, 0, inputDirection.y);
            forwardDir = transform.forward;
            if (!Shield.IsIdle)
            {
                float angle = 0;
                if (transform.forward.x >= 0)
                {
                    angle = Vector3.Angle(transform.forward, Vector3.forward);
                }
                else
                {
                    angle = 360 - Vector3.Angle(transform.forward, Vector3.forward);
                }
                //Debug.Log("angle: " + angle);
                inputDirection = Quaternion.AngleAxis(angle, Vector3.forward) * inputDirection;
                newInputDir = new Vector3(inputDirection.x, 0, inputDirection.y);
            }
            else
            {
                newInputDir = Vector3.zero;
            }

            Animator.SetFloat("Horizontal", inputDirection.x);
            Animator.SetFloat("Vertical", inputDirection.y);
            Animator.speed = input.magnitude * (speed / _speed);
        }

        Vector3 inputDir;
        Vector3 forwardDir;
        Vector3 newInputDir;

        private void UpdateRotation()
        {
            if (LinkBeam.Active)
            {
                LookTowards(LinkBeam.TargetPosition - transform.position,
                    false, true);
            }
            else if (IsLinkTarget)
            {
                LookTowards(LinkedLinkBeam.transform.position - transform.position,
                    false, true);
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

        private void UpdateShield()
        {
            if (ShieldIsActive)
            {
                Animator.SetBool("Shield Active", true);
                ShieldAnimator.SetBool("Shield Active", true);
            }
            else
            {
                Animator.SetBool("Shield Active", false);
                ShieldAnimator.SetBool("Shield Active", false);
            }
        }

        #endregion Updating

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
                RotateTowards(input, true);
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

            if (LinkBeam != null)
            {
                if (inputjustReleased)
                {
                    if (!IsLinkTarget)
                    {
                        // TODO: Can the link beam link also to certain level objects?
                        LinkBeam.Activate(!LinkBeam.Active, _otherPlayer);
                    }
                    else
                    {
                        LinkedLinkBeam.Activate(false);
                    }
                }

                // Starts link start-up/shutdown animation
                //Animator.SetBool("Link Active", Link.Active);

                result = LinkBeam.Active;
            }

            return result;
        }

        public bool HandleAltActionInput()
        {
            bool active = Input.GetAltActionInput();
            bool result = false;
            if (active && BeamOn)
            {
                if (LinkBeam != null)
                {
                    LinkBeam.Pulse();
                }
            }

            return result;
        }

        public bool Old_HandleActionInput()
        {
            bool inputHeld = false;
            bool inputjustReleased = false;
            bool active = Input.GetActionInput(out inputHeld, out inputjustReleased);
            bool result = false;

            if (Tool == PlayerTool.EnergyCollector &&
                CanUseEnergyCollector())
            {
                // Drain
                if (active)
                {
                    result = UseEnergyCollector(drain: true);
                }
            }
            else if (Tool == PlayerTool.Shield &&
                (Shield.Active || CanActivateShield()))
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

                // Starts shield opening/closing animation
                //Animator.SetBool("Shield Active", Shield.Active);

                result = !Shield.IsIdle;
            }

            return result;
        }

        public bool Old_HandleAltActionInput()
        {
            bool active = Input.GetAltActionInput();
            bool result = false;

            if (active)
            {
                if (Tool == PlayerTool.EnergyCollector &&
                    CanUseEnergyCollector())
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
            if (EnergyCollector != null && EnergyCollector.IsIdle)
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
        /// Makes the player character jump.
        /// </summary>
        /// <returns>Does the character jump</returns>
        public bool Jump()
        {
            if (CanJump())
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
                _groundCollider.JumpOffGround();
                SFXPlayer.Instance.Play(Sound.Jump_1);
                return true;
            }

            return false;
        }

        public void StartClimb(Climbable climbable)
        {
            if (CanClimb())
            {
                Debug.Log(name + " started climbing " + climbable.name);
                Climbing = true;
                _climbable = climbable;

                SFXPlayer.Instance.PlayLooped(Sound.Climbing_Slower);
            }
        }

        public void EndClimb()
        {
            if (Climbing)
            {
                Debug.Log(name + " stopped climbing");
                Climbing = false;
                _climbable = null;

                SFXPlayer.Instance.StopIndividualSFX("Climbing Pillar (Shorter)");
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

        #region Reseting

        /// <summary>
        /// Resets the player character's base values when respawning.
        /// </summary>
        protected override void ResetBaseValues()
        {
            base.ResetBaseValues();
            _elapsedRespawnTime = 0f;
            ResetCommon();
        }

        /// <summary>
        /// Cancels any running actions if the player dies or the level is reset.
        /// </summary>
        public override void CancelActions()
        {
            base.CancelActions();
            ResetCommon();
            Input.ResetInput();
            ResetAnimatorMovementAxis();
            EndClimb();
            EndPush();
            InteractionTarget = null;
        }

        /// <summary>
        /// Resets things common to both ResetBaseValues() and CancelActions().
        /// </summary>
        private void ResetCommon()
        {
            IsLinkTarget = false;
            LinkedLinkBeam = null;
            Jumping = false;
            Respawning = false;

            if (LinkBeam != null)
            {
                LinkBeam.ResetLinkBeam();
            }

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
        /// Resets the animator's movement axis', so the character stops walking.
        /// </summary>
        public void ResetAnimatorMovementAxis()
        {
            if (Animator != null)
            {
                Animator.SetFloat("Horizontal", 0f);
                Animator.SetFloat("Vertical", 0f);
            }
        }

        #endregion Reseting

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (IsDead)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, 1);
            }

            //DrawWalkingDirAndRotGizmos()
        }

        private void DrawWalkingDirAndRotGizmos()
        {
            float dist = 3;
            Vector3 startPos = transform.position + Vector3.up * 0.2f;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(startPos, startPos + inputDir * dist);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(startPos, startPos + forwardDir * dist);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(startPos, startPos + newInputDir * dist);
        }
    }
}
