using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class Goal : SwitchExpansion
    {
        protected override void Activate()
        {
            _activated = true;
            GameManager.Instance.CompletePuzzle();
        }

        protected override void Deactivate()
        {
            _activated = false;
        }
    }
}
