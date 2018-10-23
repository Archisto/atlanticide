using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{

    public class JumpAttackRecovery : IEnemyState
    {
        public void Action(float TimeElapsed)
        {
        }

        public IEnemyState Conditions(float timeElapsed)
        {
            // after 0.5s, return to chasing
            if(timeElapsed > 0.5f)
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
            
        }
    }
}