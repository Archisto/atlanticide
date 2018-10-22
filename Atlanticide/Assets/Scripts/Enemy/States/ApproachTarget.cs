using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{

    public class ApproachTarget : IEnemyState
    {

        private EnemyMinion Host;

        public void Action(float TimeElapsed)
        {
            Host.MoveTowardsTarget(Host.Target.transform.position, true, false, 0, Host.Speed());
        }

        public IEnemyState Conditions(float timeElapsed)
        {
            // check that target is still alive
            if (!Host.ValidateTarget(Host.ChaseRange))
            {
                Host.IdleWanderAnchor = Host.transform.position;
                return new IdleStanding();
            }

            // check if target is withing attack range
            if(Host.Distance(false, Host.transform.position, Host.Target.transform.position, Host.AttackRange * 0.8f, false))
            {
                return new JumpAttackStartup();
            }

            return null;
        }

        public void DrawGizmos()
        {
            Host.AddGizmoAction(new EnemyBase.GizmoAction("wire_sphere", Color.blue, Host.transform.position, Vector3.zero, Host.ChaseRange));
            Host.AddGizmoAction(new EnemyBase.GizmoAction("wire_sphere", Color.red, Host.transform.position, Vector3.zero, Host.AttackRange * 0.8f));
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