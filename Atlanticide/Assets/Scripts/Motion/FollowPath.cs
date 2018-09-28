using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Atlanticide.WaypointSystem;

namespace Atlanticide
{
    public class FollowPath : Motion
    {
        public Path path;

        [SerializeField]
        private float _topSpeed = 1f;

        private bool _onPath;
        private bool _usePathSpeed;
        private Waypoint _startWaypoint;
        private Waypoint _prevWaypoint;
        private Direction _startDirection;
        private Direction _direction;
        private float _rampGravityMultiplier;
        private float _leftoverDistance;
        private bool _getNextWaypoint;

        /// <summary>
        /// is the object on a path.
        /// </summary>
        public bool IsOnPath
        {
            get { return _onPath; }
        }

        /// <summary>
        /// The current waypoint.
        /// </summary>
        public Waypoint CurrentWaypoint { get; private set; }

        /// <summary>
        /// The current speed.
        /// </summary>
        public float CurrentSpeed { get; private set; }

        /// <summary>
        /// The top speed. Either the object's own or the path's.
        /// </summary>
        public float TopSpeed { get; private set; }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected override void Start()
        {
            base.Start();
        }

        /// <summary>
        /// Starts moving the object. If <paramref name="instant"/>
        /// is true, the object instantly has the top speed.
        /// </summary>
        /// <param name="instant">Should the object instantly
        /// have the top speed</param>
        public override void StartMoving(bool instant)
        {
            if (_onPath)
            {
                CurrentSpeed = (instant ? TopSpeed : 0);
                base.StartMoving(instant);
            }
        }

        /// <summary>
        /// Decelerates the object until it stops moving.
        /// If <paramref name="instant"/> is true, the object instantly stops.
        /// </summary>
        /// <param name="instant">Should the object instantly stop</param>
        public override void StopMoving(bool instant)
        {
            base.StopMoving(instant);
        }

        public void EnterPath(Path path, Direction direction,
            Waypoint startWaypoint, bool skipFirstWaypoint,
            bool instantTopSpeed, float pathTopSpeed = 0f)
        {
            this.path = path;
            _onPath = true;
            _usePathSpeed = (pathTopSpeed > 0f);
            TopSpeed = (_usePathSpeed ? pathTopSpeed : _topSpeed);
            StartMoving(instantTopSpeed);

            _getNextWaypoint = skipFirstWaypoint;
            _startDirection = direction;
            _direction = direction;
            _startWaypoint = startWaypoint;
            _prevWaypoint = startWaypoint;
            CurrentWaypoint = startWaypoint;
            _leftoverDistance = 0;
            //Debug.Log("Direction on path: " + direction);
        }

        public void ExitPath()
        {
            //Debug.Log("Path exited");
            path = null;
            _onPath = false;
            _getNextWaypoint = false;
        }

        protected override void MoveObject()
        {
            MoveAlongPath();
        }

        public bool MoveAlongPath()
        {
            // Are we close enough to the current waypoint?
            //    If yes, get the next waypoint
            // Move towards the current waypoint
            // Did we reach the next waypoint but didn't move as far as we could?
            //    If yes, get the next waypoint and move again using the leftover movement
            //    Repeat until there's no leftover movement
            // Return whether the object is still on the ramp

            bool wpReached = false;
            int repeats = 0;

            do
            {
                Waypoint newWaypoint = GetWaypoint();
                CurrentWaypoint = (newWaypoint == null ?
                    CurrentWaypoint : newWaypoint);
                if (_onPath)
                {
                    if (repeats == 0)
                    {
                        wpReached = MoveUsingSpeed(CurrentWaypoint.Position, false);
                    }
                    else if (_leftoverDistance > 0)
                    {
                        wpReached = MoveUsingSpeed(CurrentWaypoint.Position, true);
                    }
                    else
                    {
                        wpReached = false;
                    }
                }

                //Debug.Log(string.Format("Repeat: {0}; dist left: {1}", repeats, _leftoverDistance));
                repeats++;
            }
            while (_onPath && wpReached);

            return _onPath;
        }

        protected override void UpdateCurrentSpeed()
        {
            if (ReachedTargetSpeed)
            {
                return;
            }

            if (_active)
            {
                SpeedRatio += _acceleration * World.Instance.DeltaTime;

                if (SpeedRatio >= 1f)
                {
                    SpeedRatio = 1f;
                    CurrentSpeed = TopSpeed;
                    ReachedTargetSpeed = true;
                }
            }
            else
            {
                SpeedRatio += _deceleration * World.Instance.DeltaTime;

                if (SpeedRatio <= 0f)
                {
                    SpeedRatio = 0f;
                    CurrentSpeed = 0f;
                    ReachedTargetSpeed = true;
                    Moving = false;
                }
            }

            if (!ReachedTargetSpeed)
            {
                CurrentSpeed = SpeedRatio * TopSpeed;
            }
        }

        private Waypoint GetWaypoint()
        {
            Waypoint result = CurrentWaypoint;

            if (_getNextWaypoint)
            {
                _getNextWaypoint = false;
                result = path.GetNextWaypoint(CurrentWaypoint, ref _direction);

                if (result == null)
                {
                    ExitPath();
                }
                else
                {
                    _prevWaypoint = CurrentWaypoint;
                }
            }

            return result;
        }

        private bool MoveUsingSpeed(Vector3 waypointPos, bool leftOverOnly)
        {
            float movingDistance;

            Vector3 startPosition = transform.position;
            Vector3 targetPosition = waypointPos;

            //Vector3 direction = GetPathSegmentDirection();

            if (leftOverOnly)
            {
                movingDistance = _leftoverDistance;
            }
            else
            {
                movingDistance = CurrentSpeed * Time.deltaTime;
                //CurrentSpeed = GetSpeedAffectedByGravity(direction);
            }

            // The direction on the path has not changed
            if (_direction == _startDirection)
            {
                // Changes direction on the path
                if (CurrentSpeed < 0)
                {
                    ChangeDirection();
                    targetPosition = CurrentWaypoint.Position;
                }
            }
            // The direction on the path has changed
            else
            {
                // The maximum distance from the start of
                // the path where the object can exit it
                float pathExitDistance = 2f;

                // Checks if the object returned to the start of
                // the path, and if so, exits the path 
                if (Vector3.Distance(transform.position, _startWaypoint.Position) < pathExitDistance)
                {
                    //Debug.Log("Returned to the start of the path");
                    ExitPath();
                    return true;
                }
            }

            // Moves the object
            transform.position = Vector3.MoveTowards(
                startPosition, targetPosition, movingDistance);

            // Updates the leftover distance
            _leftoverDistance = movingDistance -
                Vector3.Distance(startPosition, transform.position);

            // Checks if the segment is finished
            float segmentFinishDistance = 0.1f;
            bool segmentFinished =
                Vector3.Distance(transform.position, targetPosition)
                < segmentFinishDistance;
            if (segmentFinished)
            {
                // Enables getting the next waypoint
                _getNextWaypoint = true;
                return true;
            }

            return false;
        }

        public Vector3 GetPathSegmentDirection()
        {
            return (CurrentWaypoint.Position - _prevWaypoint.Position).normalized;
        }

        private void ChangeDirection()
        {
            //Debug.Log("Direction changed");

            CurrentSpeed = -1f * CurrentSpeed;
            _direction = (_direction == Direction.Forward ?
                Direction.Backward : Direction.Forward);

            Waypoint temp = _prevWaypoint;
            _prevWaypoint = CurrentWaypoint;
            CurrentWaypoint = temp;
        }
    }
}
