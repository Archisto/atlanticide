using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Atlanticide.WaypointSystem;

namespace Atlanticide
{
    [RequireComponent(typeof(FollowPath))]
    public class EnergyTargetPathFollower : LevelObject
    {
        [SerializeField]
        bool forwardOnDefault = true;

        private bool forward;

        private bool activated;

        [SerializeField]
        bool permanent;

        [SerializeField]
        bool skipFirstWP = false;

        [SerializeField]
        bool instantTopSpeed = false;

        [SerializeField]
        float rampSpeed = 0;

        //[SerializeField]
        //EnergyTarget _target;

        FollowPath _pathFollower;
        Path path;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            activated = false;
            _pathFollower = GetComponent<FollowPath>();
            path = _pathFollower.path;
        }

        private void EnterPath()
        {
            Direction dir = (forward ? Direction.Forward : Direction.Backward);
            Waypoint wp = (forward ? path.GetFirstWaypoint() : path.GetLastWaypoint());
            _pathFollower.EnterPath(path, dir, wp, skipFirstWP, instantTopSpeed, rampSpeed);
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if(permanent && activated)
            {
                return;
            }

            CheckEnergyLevel();
        }

        private void CheckEnergyLevel()
        {
            //if (!activated && _target.MaxCharge)
            //{
            //    activated = true;
            //    forward = forwardOnDefault;
            //    EnterPath();
            //}

            //if (activated && !World.Instance.EmittingEnergy)
            //{
            //    _target.currentCharges = 0;
            //    forward = !forwardOnDefault;
            //    EnterPath();
            //    activated = false;
            //}
        }

        public override void ResetObject()
        {
            activated = false;
            base.ResetObject();
        }

    }
}
