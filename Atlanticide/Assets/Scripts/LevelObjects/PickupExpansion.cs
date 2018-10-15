using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// An expansion to a pickup.
    /// </summary>
    [RequireComponent(typeof(Pickup))]
    public abstract class PickupExpansion : LevelObjectExpansion
    {
        public abstract void OnPickupCollected();
    }
}
