using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{

    public class IdleStanding : IEnemyState
    {

        private EnemyMinion Host;

        public void Action(float TimeElapsed)
        {
            
        }

        public IEnemyState Conditions(float timeElapsed)
        {
            // Search for Target
            if (Host.SearhForTarget(Host.SearchRange))
            {
                return new ApproachTarget();
            }

            // Start Wandering
            if(timeElapsed > (Random.value * 5 +1))
            {
                return new IdleWander();
            }

            return null;
        }

        public void DrawGizmos()
        {

        }

        public void Finish()
        {
            Host = null;
        }

        public void Instantiate(EnemyBase enemy)
        {
            Host = (EnemyMinion)enemy;
            Host.Target = null;
            Host.Hitcast.Hitting = false;
        }

    }
}