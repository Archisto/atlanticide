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

        [Header("Hit variables")]

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
            if (Vector3.Distance(Target.transform.position, transform.position) < HitRange)
            {
                ChangeState(EnemyState.ATTACK);
                return;
            }

            // approach the target
            if (Target != null)
            {
                Vector3 move = Vector3.MoveTowards(transform.position, Target.transform.position, base._speed * Time.deltaTime);
                move.y = transform.position.y;
                Vector3 targetPostition = new Vector3(Target.transform.position.x, transform.position.y, Target.transform.position.z);
                transform.LookAt(targetPostition);
                transform.position = move;
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
                    shield.Hit();
                    ChangeState(EnemyState.TARGET_ON);
                    transform.position += transform.TransformDirection(Vector3.back * 3);
                    Debug.Log("shield hit");
                } else
                {
                    character = hit.collider.gameObject.GetComponent<GameCharacter>();
                    if(character != null && !character.IsDead)
                    {
                        character.TakeDamage(HitDamage);
                        ChangeState(EnemyState.NO_TARGET);
                        Debug.Log("character hit");
                    }
                }

                Debug.Log("hit: " + hit.collider.transform.name);
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

    }
}