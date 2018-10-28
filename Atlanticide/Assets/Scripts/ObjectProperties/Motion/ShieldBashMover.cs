using UnityEngine;
using Atlanticide.WaypointSystem;

namespace Atlanticide
{
    [RequireComponent(typeof(FollowPath))]
    [RequireComponent(typeof(ShieldBashSwitch))]
    public class ShieldBashMover : LevelObject
    {
        [SerializeField]
        private float _moveDistance = 1f;

        private FollowPath _pathFollower;
        private ShieldBashSwitch _sbSwitch;
        private Path _path;
        private bool _shieldBashed;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _pathFollower = GetComponent<FollowPath>();
            _sbSwitch = GetComponent<ShieldBashSwitch>();
            _path = GetComponentInChildren<Path>();
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (!_shieldBashed)
            {
                if (_sbSwitch.Activated)
                {
                    _shieldBashed = true;
                    EnterPath();
                }
            }
            else if (_pathFollower.Moving)
            {
                _path.transform.position = _defaultPosition;
                if (_pathFollower.TotalMovedDistance >= _moveDistance)
                {
                    _pathFollower.StopMoving(false);
                }
            }
        }

        private void EnterPath()
        {
            Waypoint firstWP = _path.GetFirstWaypoint();
            _defaultPosition = transform.position;
            _pathFollower.EnterPath(_path, Direction.Forward, firstWP, false, false);
        }

        public override void ResetObject()
        {
            _shieldBashed = false;
            _pathFollower.ResetMotion();
            SetToDefaultPosition();
            _path.transform.position = _defaultPosition;
            base.ResetObject();
        }
    }
}
