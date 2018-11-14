using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class Goal : SwitchExpansion
    {
        protected override bool CanCheckSwitch()
        {
            return base.CanCheckSwitch() &&
                GameManager.Instance.LevelWinConditionsMet();
        }

        protected override void Activate()
        {
            _activated = true;
            GameManager.Instance.EndLevel(true);
        }

        protected override void Deactivate()
        {
            _activated = false;
        }
    }
}
