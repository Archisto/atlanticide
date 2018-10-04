using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// An expansion to a pickup.
    /// </summary>
    [RequireComponent(typeof(Pickup))]
    public class PickupExpansion : MonoBehaviour
    {
        public virtual void OnPickupCollected()
        {
        }

        public virtual void OnPickupDestroyed()
        {
        }

        public virtual void OnPickupReset()
        {
        }
    }
}
