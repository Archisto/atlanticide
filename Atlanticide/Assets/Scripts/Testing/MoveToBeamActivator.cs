using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Atlanticide.WaypointSystem;
using System;

namespace Atlanticide
{
    public class MoveToBeamActivator : MonoBehaviour, ILinkInteractable
    {
        public float speed = 1f;
        public float rangeCubeRadius = 0.3f;
        public bool stayUpright = true;

        private MoveToBeam mtb;
        private Transform _movableObject;

        public bool BeamReached { get; private set; }

        public void Init(Transform movableObject)
        {
            mtb = new MoveToBeam(speed, rangeCubeRadius);
            _movableObject = movableObject;
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (!World.Instance.GamePaused)
            {
                if (mtb.isMoving)
                {
                    _movableObject.position = mtb.Move();
                    if (mtb.beamReached)
                    {
                        mtb.beamReached = false;
                        BeamReached = true;
                    }
                }

                if (stayUpright)
                {
                    //transform.rotation = Quaternion.Euler(Vector3.zero);
                    transform.LookAt(transform.position + Vector3.up);
                }
            }
        }

        public bool TryInteract(LinkBeam linkBeam)
        {
            if (!mtb.isMoving)
            {
                mtb.StartMoving(linkBeam, _movableObject.position);
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

        public void ResetMoveToBeam()
        {
            if (mtb != null)
            {
                mtb.StopMoving();
            }

            BeamReached = false;
        }
    }
}
