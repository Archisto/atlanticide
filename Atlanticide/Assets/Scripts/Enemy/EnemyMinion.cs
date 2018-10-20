using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// Small, fast enemy
    /// </summary>
    public class EnemyMinion : EnemyBase
    {
        protected override IEnemyState CreateBirthState()
        {
            return null;
        }

        protected override IEnemyState CreateDeathState()
        {
            return null;
        }
    }
}