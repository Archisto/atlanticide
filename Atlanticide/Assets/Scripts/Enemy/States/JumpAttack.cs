using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{

    public class JumpAttack : IEnemyState
    {

        private EnemyMinion Host;
        private Vector3 AttackTarget;

        public void Action(float TimeElapsed)
        {
            // Jump towards target
            Host.MoveTowardsTarget(AttackTarget, true, false, 15, Host.Speed() * 5);
        }

        public IEnemyState Conditions(float timeElapsed)
        {
            // End attack when AttackTarget is reached
            if(Host.Distance(false, AttackTarget, Host.transform.position, 0.1f, true))
            {
                return new JumpAttackRecovery();

                // backup return if over 3s has elapsed
            }else if(timeElapsed > 3)
            {
                return new JumpAttackRecovery();
            }

            // Go to shield hit stun if Shield is hit
            if (Host.Hitcast.HitType.Equals(HitCast.HitCastType.SHIELD))
            {
                return new ShieldStun();
            } else if (Host.Hitcast.HitType.Equals(HitCast.HitCastType.PLAYER))
            {
                return new JumpAttackRecovery();
            }

            return null;
        }

        public void DrawGizmos()
        {
            
        }

        public void Finish()
        {
            Host.Hitcast.Hitting = false;
        }

        public void Instantiate(EnemyBase enemy)
        {
            Host = (EnemyMinion)enemy;
            Host.Hitcast.Hitting = true;
            AttackTarget = new Vector3(Host.Target.transform.position.x, Host.transform.position.y, Host.Target.transform.position.z);
        }
    }
}