using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Atlanticide.WaypointSystem;

namespace Atlanticide
{
    [RequireComponent(typeof(FollowPath))]
    public class PathFollowerTester : MonoBehaviour
    {
        [SerializeField]
        bool forward = true;

        [SerializeField]
        bool skipFirstWP = false;

        [SerializeField]
        bool instantTopSpeed = false;

        [SerializeField]
        float rampSpeed = 0;

        FollowPath _pathFollower;
        Path path;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _pathFollower = GetComponent<FollowPath>();
            path = _pathFollower.path;
            EnterPath();
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
            if (!_pathFollower.IsOnPath)
            {
                forward = !forward;
                EnterPath();
            }
        }
    }
}
