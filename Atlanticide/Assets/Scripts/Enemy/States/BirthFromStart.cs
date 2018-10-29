using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{

    public class BirthFromStart : IEnemyState
    {
        public void Action(float TimeElapsed)
        {
            
        }

        public IEnemyState Conditions(float timeElapsed)
        {
            return new IdleStanding();
        }

        public void DrawGizmos()
        {
            throw new System.NotImplementedException();
        }

        public void Finish()
        {
            
        }

        public void Instantiate(EnemyBase enemy)
        {
            ((EnemyMinion)enemy).IdleWanderAnchor = enemy.transform.position;
        }
    }
}