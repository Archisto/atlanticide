using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{

    public interface IEnemyState
    {

        void Instantiate(EnemyBase enemy);
        IEnemyState Conditions(float timeElapsed);
        void Action(float TimeElapsed);
        void Finish();
        void DrawGizmos();
    }
}