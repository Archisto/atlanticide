using UnityEngine;
using System.Collections;

namespace Atlanticide
{
    public class OrichalcumPickup : Pickup
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Resets the pickup.
        /// </summary>
        public override void ResetObject()
        {
            IsCollected = false;
            SetToDefaultPosition();
            gameObject.SetActive(true);
            base.ResetObject();
        }
    }
}