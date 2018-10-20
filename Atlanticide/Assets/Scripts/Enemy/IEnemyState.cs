using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyState {

    void Instantiate();
    IEnemyState Conditions(float timeElapsed);
    void Action(float TimeElapsed);
    void Finish();
	
}
