using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{

    public class DeathFreeze : IEnemyState
    {
        public void Action(float TimeElapsed)
        {
            
        }

        public IEnemyState Conditions(float timeElapsed)
        {
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
            enemy.Hitcast.Hitting = false;
        }
    }
}
