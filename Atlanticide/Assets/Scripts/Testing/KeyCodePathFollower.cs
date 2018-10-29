using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Atlanticide.WaypointSystem;

namespace Atlanticide
{
    [RequireComponent(typeof(FollowPath))]
    public class KeyCodePathFollower : MonoBehaviour
    {
        [SerializeField]
        bool forwardOnDefault = true;

        private bool forward;

        private bool activated;

        [SerializeField]
        bool skipFirstWP = false;

        [SerializeField]
        bool instantTopSpeed = false;

        [SerializeField]
        float rampSpeed = 0;

        [SerializeField]
        int _keyCode;

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
            CheckForKey();
        }

        private void CheckForKey()
        {
            bool keyMatch = false;


            foreach (int ownedKeyCode in World.Instance.keyCodes)
            {
                if (_keyCode == ownedKeyCode)
                {
                    keyMatch = true;
                    if (!activated)
                    {
                        forward = forwardOnDefault;
                        EnterPath();
                        activated = true;
                    }
                }
            }


            if (!keyMatch && activated)
            {
                activated = false;
                forward = !forwardOnDefault;
                EnterPath();
            }
        }

    }
}
