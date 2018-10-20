using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{

    public class EnemyCharacter : GameCharacter
    {

        #region References

        /// <summary>
        /// The target EnemyCharacter focuses its actions to
        /// </summary>
        protected GameCharacter Target { get; set; }

        [Header("HIT VARIABLES")]

        // ray is casted from the centre of this object, to the direction of forward of this object
        [SerializeField]
        private Transform _hitCentre;

        // range of the hit
        [SerializeField]
        private float _hitRange;

        // damage made on hit
        [SerializeField]
        private int _hitDamage;

        #endregion

        #region Logic

        /// <summary>
        /// Different States EnemyCharacter can act upon
        /// </summary>
        protected enum EnemyState
        {
            NO_TARGET,
            TARGET_ON,
            ATTACK
        }

        /// <summary>
        /// Current EnemyState of EnemyCharacter
        /// </summary>
        protected EnemyState CurrentState { get; private set; }

        /// <summary>
        /// Initialize new EnemyState
        /// </summary>
        /// <param name="state">New EnemyState</param>
        protected void ChangeState(EnemyState state)
        {
            // finish state
            switch (CurrentState)
            {
                case EnemyState.NO_TARGET:
                    EndNoTarget();
                    break;
                case EnemyState.TARGET_ON:
                    EndTargetOn();
                    break;
                case EnemyState.ATTACK:
                    EndAttack();
                    break;
            }

            // set new current state
            CurrentState = state;

            // initialize state
            switch (CurrentState)
            {
                case EnemyState.NO_TARGET:
                    StartNoTarget();
                    break;
                case EnemyState.TARGET_ON:
                    StartTargetOn();
                    break;
                case EnemyState.ATTACK:
                    StartAttack();
                    break;
            }
        }

        // How long enemy is on recovery where it does nothing
        protected float InRecovery;

        // How long enemy has been in recovery
        protected float RecoveryTimer;

        #endregion

        #region Hitray

        // ray is casted from the centre of this object, to the direction of forward of this object
        protected Transform HitCentre { get; private set; }

        // range of the hit
        protected float HitRange
        {
            get { return _hitRange; }
            set { _hitRange = value; }
        }

        // damage made on hit
        protected int HitDamage {
            get { return _hitDamage; }
            set { _hitDamage = value; }
        }

        // when HitRay is being checked
        protected bool HitCheck { get; set; }

        #endregion

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            HitCentre = _hitCentre;
            HitCheck = false;
            InRecovery = 0;
            RecoveryTimer = 0;
            CurrentState = EnemyState.NO_TARGET;

            StartNoTarget();
        }

        // Update is called once per frame  
        protected override void Update()
        {
            base.Update();

            if (IsDead)
            {
                return;
            }

            // If enemy is in recovery, don't execute state actions
            if(RecoveryTimer < InRecovery)
            {
                RecoveryTimer += Time.deltaTime;
                return;
            }

            // perform state specific actions
            switch (CurrentState)
            {
                case EnemyState.NO_TARGET:
                    NoTarget();
                    break;
                case EnemyState.TARGET_ON:
                    TargetOn();
                    break;
                case EnemyState.ATTACK:
                    Attack();
                    break;
            }
        }


        #region state initializations

        /// <summary>
        /// Initialize NO_TARGET state
        /// </summary>
        protected virtual void StartNoTarget()
        {
            Target = null;
        }

        /// <summary>
        /// Initialize TARGET_ON state
        /// </summary>
        protected virtual void StartTargetOn()
        {

        }

        /// <summary>
        /// Initialize ATTACK state
        /// </summary>
        protected virtual void StartAttack()
        {
            HitCheck = true;
        }


        #endregion

        #region state actions

        /// <summary>
        /// Performs the NO_TARGET state of the enemy
        /// </summary>
        protected virtual void NoTarget()
        {
            GameCharacter[] players = GameManager.Instance.GetPlayersWithinRange(transform.position, 10 );

            if (players[0] != null)
            {
                Target = players[0];
                ChangeState(EnemyState.TARGET_ON);
            }
        }

        /// <summary>
        /// Performs the TARGET_ON state of the enemy
        /// </summary>
        protected virtual void TargetOn()
        {
            // enemy attacks when he's close enough the target
            if (Distance(false, Target.transform.position, transform.position, HitRange, true))
            {
                ChangeState(EnemyState.ATTACK);
                return;
            }

            // approach the target
            if (Target != null)
            {
                MoveToTarget(Target.transform.position, 0, _speed);
            }
            else
            {
                ChangeState(EnemyState.NO_TARGET);
            }
        }

        /// <summary>
        /// Performs the ATTACK state of the enemy
        /// </summary>
        protected virtual void Attack()
        {
            // check hit
            if (HitCheck)
            {
                CheckHit();
            }
        }

        #endregion

        #region finishes

        /// <summary>
        /// Finishes the NO_TARGET state
        /// </summary>
        protected virtual void EndNoTarget()
        {

        }

        /// <summary>
        /// Finishes the ON_TARGET state
        /// </summary>
        protected virtual void EndTargetOn()
        {

        }

        /// <summary>
        /// Finishes the ATTACK state
        /// </summary>
        protected virtual void EndAttack()
        {
            HitCheck = false;
        }


        #endregion

        #region Hitting

        /// <summary>
        /// Checks whether enemy hits something
        /// </summary>
        private void CheckHit()
        {
            RaycastHit hit;
            if(Physics.Raycast(HitCentre.position, HitCentre.TransformDirection(Vector3.forward), out hit, HitRange))
            {
                // Check whether shield or character has been hit and react accordingly
                Shield shield = hit.collider.gameObject.GetComponent<Shield>();
                GameCharacter character;

                if(shield != null && shield.BlocksDamage)
                {
                    HitShield(shield);
                } else
                {
                    character = hit.collider.gameObject.GetComponent<GameCharacter>();
                    if(character != null && !character.IsDead)
                    {
                        HitCharacter(character);
                    }
                }
            }
        }

        /// <summary>
        /// What happens when enemy hits a shield
        /// </summary>
        /// <param name="shield">shield script on the object that is hit</param>
        protected virtual void HitShield(Shield shield)
        {
            shield.Hit();
            ChangeState(EnemyState.TARGET_ON);
            transform.position += transform.TransformDirection(Vector3.back * 3);
            Debug.Log("shield hit");
        }

        /// <summary>
        /// What happens when enemy hits a character
        /// </summary>
        /// <param name="character">GameCharacter script on the object that is hit</param>
        protected virtual void HitCharacter(GameCharacter character)
        {
            if (character.Equals(Target))
            {
                character.TakeDamage(HitDamage);
                ChangeState(EnemyState.NO_TARGET);
                Debug.Log("character hit");
            }
        }

        /// <summary>
        /// Draws the HitRay red if enemy is hitting, yellow otherwise
        /// </summary>
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if(HitCheck)
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.yellow;
            }
            if(HitCentre == null)
            {
                HitCentre = _hitCentre;
            }
            Gizmos.DrawRay(HitCentre.position, HitCentre.TransformDirection(Vector3.forward) * HitRange);
        }

        #endregion

        /// <summary>
        /// Moves object towards the target
        /// </summary>
        protected void MoveToTarget(Vector3 target, float rise, float speed)
        {
            Vector3 move = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            move.y = transform.position.y;
            Vector3 targetPostition = new Vector3(target.x, transform.position.y, target.z);
            transform.LookAt(targetPostition);
            transform.position = move;
            _groundCollider.onGround = false;
            _groundCollider.Rise(rise);
        }

        /// <summary>
        /// Checks whether distance between given vectors is bigger/smaller than given value.
        /// Y axis of vectors can be neutralized from the calculation.
        /// </summary>
        /// <param name="bigger">is distance compared by bigger or equal to</param>
        /// <param name="start">start vector</param>
        /// <param name="end">end vector</param>
        /// <param name="distance">to what distance the distance between vectors is compared to</param>
        /// <param name="negateY">Are y axis values neutralized from the calculation</param>
        /// <returns></returns>
        protected bool Distance(bool bigger, Vector3 start, Vector3 end, float distance, bool negateY)
        {
            if (negateY)
            {
                start.Set(start.x, 0, start.z);
                end.Set(end.x, 0, end.z);
            }

            if (bigger)
            {
                return Vector3.Distance(start, end) >= distance;
            } else
            {
                return Vector3.Distance(start, end) <= distance;
            }
        }

    }
}