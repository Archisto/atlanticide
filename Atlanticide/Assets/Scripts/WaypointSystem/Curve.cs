using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide.WaypointSystem
{
    public class Curve : MonoBehaviour
    {
        [SerializeField]
        private Waypoint _startWaypoint;

        [SerializeField]
        private Waypoint _endWaypoint;

        [SerializeField]
        private Vector3[] _points;

        [SerializeField]
        private Waypoint[] _waypoints;

        [SerializeField, Range(5, 50)]
        private int _lineSteps = 10;

        private Path _parentPath;

        private void Start()
        {
            // TODO: Fix forgetting created waypoints when play mode is started.
            // This does not affect the path but makes it impossible to remove them
            // using the Destroy Waypoints button after returning to the editor mode.

            if (_startWaypoint != null && _endWaypoint != null)
            {
                _parentPath = _startWaypoint.transform.parent.GetComponent<Path>();

                if (_parentPath == null)
                {
                    Debug.LogError("Parent path not found.");
                }
            }
            else
            {
                Debug.LogError
                    ("Start and/or end waypoint is not set for this curve.");
            }
        }

        public Vector3 GetPoint(float t)
        {
            return transform.TransformPoint(
                Utils.GetCurvePoint(Points[0], Points[1], Points[2], t));
        }

        public Vector3[] Points
        {
            get
            {
                if (_points == null)
                {
                    _points = new Vector3[3];
                }

                return _points;
            }
            set
            {
                _points = value;
            }
        }

        public Waypoint StartWaypoint
        {
            get
            {
                return _startWaypoint;
            }
        }

        public Waypoint EndWaypoint
        {
            get
            {
                return _endWaypoint;
            }
        }

        public int LineSteps
        {
            get
            {
                return _lineSteps;
            }
        }

        public Vector3 MidPoint { get; set; }

        public int PathChildIndex { get; set; }

        public Waypoint[] Waypoints
        {
            get
            {
                return _waypoints;
            }
            set
            {
                _waypoints = value;
            }
        }

        public bool WaypointsCreated
        {
            get
            {
                return (_waypoints != null &&
                        _waypoints.Length > 0);
            }
        }

        public void ResetPoints()
        {
            Points = new Vector3[3];
            _waypoints = null;

            FixEnds();

            if (_startWaypoint != null && _endWaypoint != null)
            {
                Points[1] =
                    (_endWaypoint.Position - _startWaypoint.Position)
                    * 0.5f;
                MidPoint = Points[1];
            }
        }

        public void FixEnds()
        {
            if (_startWaypoint != null)
            {
                transform.position = _startWaypoint.Position;
                Points[0] = Vector3.zero;

                if (_endWaypoint != null)
                {
                    Points[2] = _endWaypoint.Position - _startWaypoint.Position;
                }
            }
        }
    }
}
