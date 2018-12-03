using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Atlanticide.WaypointSystem;
using System;

namespace Atlanticide
{
    public class MoveToBeamTester : MonoBehaviour, ILinkInteractable
    {
        public float speed = 1f;
        public float rangeCubeRadius = 0.3f;

        MoveToBeam mtb;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            mtb = new MoveToBeam(speed, rangeCubeRadius);
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (mtb.isMoving)
            {
                transform.position = mtb.Move();
            }
        }

        public bool TryInteract(LinkBeam linkBeam)
        {
            if (!mtb.isMoving)
            {
                mtb.StartMoving(linkBeam, transform.position);
            }
            return true;
        }

        public bool TryInteractInstant(LinkBeam linkBeam)
        {
            return false;
        }

        public bool GivePulse(LinkBeam linkBeam, float speedModifier)
        {
            return false;
        }
    }
}
