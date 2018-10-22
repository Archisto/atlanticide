using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{

    public class IdleWander : IEnemyState
    {
        private EnemyMinion Host;
        private Vector3 IdleTarget;

        public void Action(float TimeElapsed)
        {
            // Move towards the Target
            Host.MoveTowardsTarget(IdleTarget, true, false, 0, Host.Speed() * 0.5f);
        }

        public IEnemyState Conditions(float timeElapsed)
        {
            // search for target
            if (Host.SearhForTarget(Host.SearchRange))
            {
                return new ApproachTarget();
            }

            // check if IdleTarget is reached
            if(Host.Distance(false, IdleTarget, Host.transform.position, 0.1f, true))
            {
                return new IdleStanding();
            }

            return null;
        }

        public void DrawGizmos()
        {
            Host.AddGizmoAction(new EnemyBase.GizmoAction("wire_sphere", Color.blue, Host.IdleWanderAnchor, Vector3.zero, Host.IdleWanderRange));
        }

        public void Finish()
        {
            Host = null;
        }

        public void Instantiate(EnemyBase enemy)
        {
            Host = (EnemyMinion)enemy;
            Vector3 temp = Random.insideUnitCircle * Host.IdleWanderRange;
            Vector3 random = new Vector3(temp.x, 0, temp.y);
            IdleTarget = Host.IdleWanderAnchor + random;
        }
        
    }
}