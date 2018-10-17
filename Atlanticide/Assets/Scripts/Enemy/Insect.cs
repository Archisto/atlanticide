using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{

    public class Insect : EnemyCharacter
    {
        #region Public variables

        [Header("Behaviour")]

        // The area around Insect where it wanders without target
        [SerializeField]
        private float IdleRange;

        // The area around Insect where it searches targets from
        [SerializeField]
        private float SearchRange;

        // The distance to Target where Insect starts its attack from
        [SerializeField]
        private float AttackRange;

        #endregion

        #region Private Variables

        // The point in the world where Insect wanders around IdleRange
        private Vector3 IdleAnchor;

        // The point Insect is currently going to
        private Vector3 IdleTarget;

        // Insect falls back after hitting the shield
        private bool FallBack;

        // Where Insect falls back
        private Vector3 FallBackTarget;

        // The point where player is when Insect starts attack
        private Vector3 AttackTarget;

        #endregion

        #region start methods overwritten

        protected override void StartNoTarget()
        {
            base.StartNoTarget();
            IdleAnchor = transform.position;
            IdleTarget = IdleAnchor;
        }

        protected override void StartTargetOn()
        {
            base.StartTargetOn();
        }

        protected override void StartAttack()
        {
            HitCheck = true;
            Vector3 targetPostition = new Vector3(Target.transform.position.x, transform.position.y, Target.transform.position.z);
            transform.LookAt(targetPostition);
        }

        #endregion

        #region run methods overwritten

        protected override void NoTarget()
        {
            // Search for target
            GameCharacter[] players = GameManager.Instance.GetPlayersWithinRange(transform.position, SearchRange);

            if (players[0] != null)
            {
                Target = players[0];
                ChangeState(EnemyState.TARGET_ON);
                return;
            }

            // if Insect is at the idle target, set new target
            if (Distance(false, IdleTarget, transform.position, 0.1f, true))
            {
                transform.position = IdleTarget;
                Vector3 temp = Random.insideUnitCircle * IdleRange;
                Vector3 random = new Vector3(temp.x, 0, temp.y);
                IdleTarget = IdleAnchor + random;
            }

            // Move towards the Target
            MoveToTarget(IdleTarget, 0, _speed * 0.5f);
        }

        protected override void TargetOn()
        {
            // Return TargetOn if target is null or dead
            if(Target == null || Target.IsDead)
            {
                ChangeState(EnemyState.NO_TARGET);
                return;
            }

            // Enemy attacks when he's close enough the target
            if(Distance(false, Target.transform.position, transform.position, AttackRange, true))
            {
                AttackTarget = new Vector3(Target.transform.position.x, transform.position.y, Target.transform.position.z);
                ChangeState(EnemyState.ATTACK);
                return;
            }

            // Approach the target
            MoveToTarget(Target.transform.position, 0, _speed);
        }

        protected override void Attack()
        {
            base.Attack();

            // Return if state has changed
            if (!CurrentState.Equals(EnemyState.ATTACK))
            {
                return;
            }

            if (FallBack)
            {
                // Insect stumbles backwards
                MoveToTarget(FallBackTarget, 0, _speed * 4);
                if(Distance(false, FallBackTarget, transform.position, 0.1f, true))
                {
                    InRecovery = 1;
                    RecoveryTimer = 0;
                    ChangeState(EnemyState.TARGET_ON);
                }
            }
            else
            {

                // Charge towards the target
                MoveToTarget(AttackTarget, 15, _speed * 5f);
                //_groundCollider.GroundHeightDifference

                if(Distance(false, AttackTarget, transform.position, 0.1f, true))
                {
                    InRecovery = 1;
                    RecoveryTimer = 0;
                    ChangeState(EnemyState.TARGET_ON);
                }
            }
        }

        protected override void HitShield(Shield shield)
        {
            shield.Hit();
            HitCheck = false;
            FallBack = true;
            FallBackTarget = transform.position + transform.TransformDirection(Vector3.back * AttackRange * 1.1f);
        }

        #endregion

        #region end methods overwritten

        protected override void EndNoTarget()
        {

        }

        protected override void EndTargetOn()
        {

        }

        protected override void EndAttack()
        {
            HitCheck = false;
            FallBack = false;
        }

        #endregion

        /// <summary>
        /// Draw Insect behaviour areas
        /// </summary>
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            switch (CurrentState)
            {
                case EnemyState.NO_TARGET:
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireCube(IdleTarget, new Vector3(0.5f, 0.5f, 0.5f));
                    Gizmos.DrawWireSphere(IdleAnchor, IdleRange);
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireSphere(transform.position, SearchRange);
                    break;
                case EnemyState.TARGET_ON:
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(Target.transform.position, Target.transform.position + new Vector3(0, 3, 0));
                    Vector3 pos = transform.position + transform.TransformDirection(Vector3.forward * AttackRange);
                    Gizmos.DrawLine(pos + transform.TransformDirection(Vector3.left), pos + transform.TransformDirection(Vector3.right));
                    break;
                case EnemyState.ATTACK:

                    break;
            }
        }

    }
}