using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{

    public class JumpAttackStartup : IEnemyState
    {

        private EnemyMinion Host;

        public void Action(float TimeElapsed)
        {
            Host.MoveTowardsTarget(Host.Target.transform.position, true, false, 0, 0);
        }

        public IEnemyState Conditions(float timeElapsed)
        {
            // If target is not inside AttackRange, return to chasing
            if(!Host.ValidateTarget(Host.AttackRange))
            {
                return new ApproachTarget();
            }

            // after 0.3s, start jump attack
            if(timeElapsed > 0.3f)
            {
                return new JumpAttack();
            }

            return null;
        }

        public void DrawGizmos()
        {
            Host.AddGizmoAction(new EnemyBase.GizmoAction("wire_sphere", Color.red, Host.transform.position, Vector3.zero, Host.AttackRange));
        }

        public void Finish()
        {
        }

        public void Instantiate(EnemyBase enemy)
        {
            Host = (EnemyMinion)enemy;
        }
    }
}