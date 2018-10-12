using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{

    public class EnemyCharacter : GameCharacter
    {

        #region References

        /// <summary>
        /// Reference to the DamageDealer of _hitbox
        /// </summary>
        protected DamageDealer DamageDealer { get; private set; }

        /// <summary>
        /// The target EnemyCharacter focuses its actions to
        /// </summary>
        protected GameObject _target { get; set; }

        /// <summary>
        /// Hitbox gameObject
        /// </summary>
        [SerializeField] private GameObject _hitbox;

        protected GameObject Hitbox
        {
            get { return _hitbox; }
        }

        [Header("hitbox transform")]

        /// <summary>
        /// Position of the hitbox
        /// </summary>
        [SerializeField] private Vector3 _hitboxPosition;

        /// <summary>
        /// Rotation of the hitbox
        /// </summary>
        [SerializeField] private Quaternion _hitboxRotation;

        /// <summary>
        /// Scale of the hitbox
        /// </summary>
        [SerializeField] private Vector3 _hitboxScale;

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
                    endNoTarget();
                    break;
                case EnemyState.TARGET_ON:
                    endTargetOn();
                    break;
                case EnemyState.ATTACK:
                    endAttack();
                    break;
            }

            // set new current state
            CurrentState = state;

            // initialize state
            switch (CurrentState)
            {
                case EnemyState.NO_TARGET:
                    startNoTarget();
                    break;
                case EnemyState.TARGET_ON:
                    startTargetOn();
                    break;
                case EnemyState.ATTACK:
                    startAttack();
                    break;
            }
        }

        #endregion

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            DamageDealer = _hitbox.GetComponent<DamageDealer>();
            _hitbox.SetActive(false);
            CurrentState = EnemyState.NO_TARGET;
            startNoTarget();
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
                    noTarget();
                    break;
                case EnemyState.TARGET_ON:
                    targetOn();
                    break;
                case EnemyState.ATTACK:
                    attack();
                    break;
            }
        }


        #region state initializations

        /// <summary>
        /// Initialize NO_TARGET state
        /// </summary>
        protected virtual void startNoTarget()
        {
            _target = null;
        }

        /// <summary>
        /// Initialize TARGET_ON state
        /// </summary>
        protected virtual void startTargetOn()
        {

        }

        /// <summary>
        /// Initialize ATTACK state
        /// </summary>
        protected virtual void startAttack()
        {
            _hitbox.SetActive(true);
        }


        #endregion

        #region state actions

        /// <summary>
        /// Performs the NO_TARGET state of the enemy
        /// </summary>
        protected virtual void noTarget()
        {

            GameCharacter[] players = GameManager.Instance.GetPlayersWithinRange(transform.position, 4);

            if(players[0] != null)
            {
                _target = players[0].gameObject;
                ChangeState(EnemyState.TARGET_ON);
            }
        }

        /// <summary>
        /// Performs the TARGET_ON state of the enemy
        /// </summary>
        protected virtual void targetOn()
        {
            // enemy attacks when he's close enough the target
            if (Vector3.Distance(_target.transform.position, transform.position) < 1.5f)
            {
                ChangeState(EnemyState.ATTACK);
                return;
            }

            // approach the target
            if (_target != null)
            {
                Vector3 move = Vector3.MoveTowards(transform.position, _target.transform.position, base._speed * Time.deltaTime);
                move.y = transform.position.y;
                Vector3 targetPostition = new Vector3(_target.transform.position.x, transform.position.y, _target.transform.position.z);
                transform.LookAt(targetPostition);
                transform.position = move;
            } else
            {
                ChangeState(EnemyState.NO_TARGET);
            }

        }

        /// <summary>
        /// Performs the ATTACK state of the enemy
        /// </summary>
        protected virtual void attack()
        {
            // adjust hitbox
            _hitbox.transform.localPosition = _hitboxPosition;
            _hitbox.transform.localRotation = _hitboxRotation;
            _hitbox.transform.localScale = _hitboxScale;

            ChangeState(EnemyState.NO_TARGET);
        }

        #endregion

        #region finishes

        /// <summary>
        /// Finishes the NO_TARGET state
        /// </summary>
        protected virtual void endNoTarget()
        {

        }

        /// <summary>
        /// Finishes the ON_TARGET state
        /// </summary>
        protected virtual void endTargetOn()
        {

        }

        /// <summary>
        /// Finishes the ATTACK state
        /// </summary>
        protected virtual void endAttack()
        {
            _hitbox.SetActive(false);
        }


        #endregion

    }
}