using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public abstract class LevelObject : MonoBehaviour, ILevelObject
    {
        public virtual void ResetObject()
        {
        }
    }
}
