using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{

    public class ShieldStun : IEnemyState
    {
        private EnemyMinion Host;
        private Vector3 FallBackPoint;

        public void Action(float TimeElapsed)
        {
            Host.MoveTowardsTarget(FallBackPoint, false, false, 0, 8);
        }

        public IEnemyState Conditions(float timeElapsed)
        {
            // Get back chasing target, when fallbackpoint is reached
            if(Host.Distance(false, Host.transform.position, FallBackPoint, 0.1f, true))
            {
                return new ApproachTarget();
            }

            return null;
        }

        public void DrawGizmos()
        {
            
        }

        public void Finish()
        {
            
        }

        public void Instantiate(EnemyBase enemy)
        {
            Host = (EnemyMinion)enemy;
            FallBackPoint = Host.transform.position + Host.transform.TransformDirection(Vector3.back * Host.AttackRange * 0.8f);
        }
    }
}