using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide.WaypointSystem
{
    public enum PathType
    {
        Loop, // After reaching the last waypoint, the user moves to the first
        PingPong, // The user changes direction after reaching the last waypoint
        OneShot // After reaching the last waypoint, the user exits the path
    }

    public enum Direction
    {
        Forward,
        Backward
    }

    public class Path : MonoBehaviour
    {
        [SerializeField]
        private PathType _pathType;

        [SerializeField]
        private Color _pathColor = Color.red;

        [SerializeField]
        private bool _drawPath = true;

        [SerializeField]
        private float _waypointRadius = 0.5f;

        private List<Waypoint> _waypoints;

        public List<Waypoint> Waypoints
        {
            get
            {
                // Populates the waypoints if not done that yet and in editor every time
                if (_waypoints == null ||
                    _waypoints.Count == 0 ||
                    !Application.isPlaying)
                {
                    PopulateWaypoints();
                }
                return _waypoints;
            }
        }

        public Waypoint GetFirstWaypoint()
        {
            if (Waypoints.Count > 0)
            {
                return Waypoints[0];
            }
            else
            {
                return null;
            }
        }

        public Waypoint GetLastWaypoint()
        {
            if (Waypoints.Count > 0)
            {
                return Waypoints[Waypoints.Count - 1];
            }
            else
            {
                return null;
            }
        }

        public Waypoint GetClosestWaypoint(Vector3 position)
        {
            float smallestSqrMagnitude = float.PositiveInfinity;
            Waypoint closest = null;
            for (int i = 0; i < Waypoints.Count; i++)
            {
                Waypoint waypoint = Waypoints[i];
                Vector3 toWaypointVector = waypoint.Position - position;
                float currentSqrMagnitude = toWaypointVector.sqrMagnitude;
                if (currentSqrMagnitude < smallestSqrMagnitude)
                {
                    closest = waypoint;
                    smallestSqrMagnitude = currentSqrMagnitude;
                }
            }

            return closest;
        }

        public Waypoint GetNextWaypoint(Waypoint currentWaypoint,
            ref Direction direction)
        {
            Waypoint nextWaypoint = null;
            for (int i = 0; i < Waypoints.Count; i++)
            {
                if (Waypoints[i] == currentWaypoint)
                {
                    switch (_pathType)
                    {
                        case PathType.Loop:
                        {
                            nextWaypoint = GetNextWaypointLoop(i, direction);
                            break;
                        }
                        case PathType.PingPong:
                        {
                            nextWaypoint = GetNextWaypointPingPong(i, ref direction);
                            break;
                        }
                        case PathType.OneShot:
                        {
                            nextWaypoint = GetNextWaypointOneShot(i, direction);
                            break;
                        }
                    }
                    break;
                }
            }
            return nextWaypoint;
        }

        private Waypoint GetNextWaypointPingPong(int currentWaypointIndex,
            ref Direction direction)
        {
            Waypoint nextWaypoint = null;
            switch (direction)
            {
                case Direction.Forward:
                {
                    if (currentWaypointIndex < Waypoints.Count - 1)
                    {
                        nextWaypoint = Waypoints[currentWaypointIndex + 1];
                    }
                    else
                    {
                        nextWaypoint = Waypoints[currentWaypointIndex - 1];
                        direction = Direction.Backward;
                    }
                    break;
                }
                case Direction.Backward:
                {
                    if (currentWaypointIndex > 0)
                    {
                        nextWaypoint = Waypoints[currentWaypointIndex - 1];
                    }
                    else
                    {
                        nextWaypoint = Waypoints[1];
                        direction = Direction.Forward;
                    }
                    break;
                }
            }
            return nextWaypoint;
        }

        private Waypoint GetNextWaypointLoop(int currentWaypointIndex,
            Direction direction)
        {
            Waypoint nextWaypoint = direction == Direction.Forward
                ? Waypoints[++currentWaypointIndex % Waypoints.Count]
                : Waypoints[((--currentWaypointIndex >= 0) ? currentWaypointIndex : Waypoints.Count - 1) % Waypoints.Count];
            return nextWaypoint;
        }

        private Waypoint GetNextWaypointOneShot(int currentWaypointIndex,
            Direction direction)
        {
            Waypoint nextWaypoint = null;
            switch (direction)
            {
                case Direction.Forward:
                {
                    if (currentWaypointIndex < Waypoints.Count - 1)
                    {
                        nextWaypoint = Waypoints[currentWaypointIndex + 1];
                    }
                    break;
                }
                case Direction.Backward:
                {
                    if (currentWaypointIndex > 0)
                    {
                        nextWaypoint = Waypoints[currentWaypointIndex - 1];
                    }
                    break;
                }
            }
            return nextWaypoint;
        }

        private void PopulateWaypoints()
        {
            int childCount = transform.childCount;
            _waypoints = new List<Waypoint>(childCount);
            for (int i = 0; i < childCount; i++)
            {
                Transform childTransform = transform.GetChild(i);

                Waypoint waypoint = childTransform.GetComponent<Waypoint>();
                if (waypoint != null)
                {
                    _waypoints.Add(waypoint);
                }
            }
        }

        public float Length
        {
            get
            {
                float length = 0;

                for (int i = 1; i < Waypoints.Count; i++)
                {
                    length += Vector3.Distance(Waypoints[i + 1].Position, Waypoints[i].Position);
                }

                return length;
            }
        }

        public float GetSegmentLength(Waypoint waypoint, bool start)
        {
            Waypoint startWaypoint = waypoint;
            Waypoint endWaypoint = null;
            int otherWaypointIndex = Waypoints.IndexOf(waypoint) + (start ? 1 : -1);

            if (otherWaypointIndex < 0 || otherWaypointIndex >= Waypoints.Count)
            {
                return 0;
            }

            if (start)
            {
                endWaypoint = Waypoints[otherWaypointIndex];
            }
            else
            {
                startWaypoint = Waypoints[otherWaypointIndex];
                endWaypoint = waypoint;
            }

            // Length for a straight segment
            float length = Vector3.Distance(startWaypoint.Position,
                endWaypoint.Position);

            return length;
        }

        public Color PathColor
        {
            get
            {
                return _pathColor;
            }
            set
            {
                _pathColor = value;
            }
        }

        private Waypoint CreateWaypoint(string name, Vector3 position)
        {
            // Creates the new waypoint
            GameObject wpObj = new GameObject(name);
            Waypoint waypoint = wpObj.AddComponent<Waypoint>();
            waypoint.transform.SetParent(transform);

            // Sets the waypoint's position and rotation
            waypoint.transform.position = position;
            waypoint.transform.rotation = new Quaternion(0, 0, 0, 0);

            return waypoint;
        }

        public Waypoint AddWaypoint()
        {
            int waypointCount = transform.childCount;

            Waypoint[] waypoints =
                GetComponentsInChildren<Waypoint>();

            Waypoint prevWaypoint =
                (waypoints.Length > 0 ?
                    waypoints[waypoints.Length - 1] : null);

            // The name of the waypoint
            string waypointName = string.Format
                ("Waypoint{0}", (waypointCount + 1).ToString("D3"));

            // The position of the waypoint
            Vector3 waypointPosition = (prevWaypoint != null ?
                prevWaypoint.Position : transform.position);

            // Creates the new waypoint
            Waypoint waypoint = CreateWaypoint(waypointName, waypointPosition);

            return waypoint;
        }

        /// <summary>
        /// Inserts a default waypoint after the given waypoint.
        /// </summary>
        /// <param name="prevWaypoint">
        /// The waypoint after which a new
        /// waypoint will be inserted
        /// </param>
        /// <returns>The inserted waypoint</returns>
        public Waypoint InsertWaypoint(Waypoint prevWaypoint)
        {
            return InsertWaypoints(prevWaypoint, new Vector3[0], "")[0];
        }

        /// <summary>
        /// Inserts waypoints in the given positions after the given waypoint.
        /// </summary>
        /// <param name="prevWaypoint">
        /// The waypoint after which a new
        /// waypoint will be inserted
        /// </param>
        /// <param name="waypointPositions">
        /// The positions the new waypoints will be in
        /// </param>
        /// <param name="curveName">
        /// The name of the curve the new waypoints belong to;
        /// can be left empty
        /// </param>
        /// <returns>The inserted waypoints</returns>
        public Waypoint[] InsertWaypoints(Waypoint prevWaypoint,
            Vector3[] waypointPositions, string curveName)
        {
            bool curve = (curveName.Length > 0);

            // Default insert: one waypoint to insert
            bool defaultInsert = false;
            if (waypointPositions.Length < 1)
            {
                defaultInsert = true;
                waypointPositions = new Vector3[1];
            }

            int targetWPIndex = GetWaypointIndex(prevWaypoint);
            int waypointsAfterCount = _waypoints.Count - targetWPIndex - 1;

            // Checks if the target waypoint is valid and returns if not
            if (targetWPIndex == -1 || waypointsAfterCount < 1)
            {
                Debug.LogError("Cannot insert a new waypoint after the " +
                               "last one. To do this, use the Add " +
                               "Waypoint button.");
                return null;
            }

            // List of waypoints with the new ones included
            Waypoint[] allWaypoints =
                new Waypoint[_waypoints.Count + waypointPositions.Length];

            // List of inserted waypoints
            Waypoint[] newWaypoints = new Waypoint[waypointPositions.Length];

            for (int i = 0; i < allWaypoints.Length; i++)
            {
                // The name of the current waypoint
                string waypointName =
                    string.Format("Waypoint{0}", (i + 1).ToString("D3"));

                // If a waypoint comes before the target
                // waypoint, it is only added to the new list
                if (i <= targetWPIndex)
                {
                    allWaypoints[i] = _waypoints[i];
                }
                else
                {
                    // The inserted waypoint's index
                    int insertedIndex = i - targetWPIndex - 1;

                    // Creates a new waypoint
                    if (i > targetWPIndex &&
                        i <= targetWPIndex + waypointPositions.Length)
                    {
                        // Adds the name of the possible curve
                        // to the end of the waypoint's name
                        if (curve)
                        {
                            waypointName += string.Format(" ({0})", curveName);
                        }

                        // The position of the waypoint
                        Vector3 waypointPosition = transform.position;
                        if (!defaultInsert)
                        {
                            waypointPosition = waypointPositions[insertedIndex];
                        }
                        // Default position
                        else
                        {
                            waypointPosition = Vector3.Lerp(
                                prevWaypoint.Position, _waypoints[i].Position, 0.5f);
                        }

                        // Creates the new waypoint
                        Waypoint newWaypoint =
                            CreateWaypoint(waypointName, waypointPosition);
                        newWaypoint.IsPartOfCurve = curve;
                        newWaypoint.CurveName = curveName;

                        allWaypoints[i] = newWaypoint;
                        newWaypoints[insertedIndex] = newWaypoint;
                    }
                    // Renames following waypoints to keep them in order
                    else
                    {
                        allWaypoints[i] = _waypoints[i - waypointPositions.Length];

                        if (allWaypoints[i].IsPartOfCurve)
                        {
                            waypointName += string.Format(" ({0})",
                                allWaypoints[i].CurveName);
                        }
                        allWaypoints[i].gameObject.name = waypointName;
                    }

                    // Reorganizes the waypoint list in editor
                    allWaypoints[i].transform.SetSiblingIndex(i);
                }
            }

            return newWaypoints;
        }

        private int GetWaypointIndex(Waypoint waypoint)
        {
            int targetWPIndex = -1;

            for (int i = 0; i < _waypoints.Count; i++)
            {
                if (_waypoints[i] == waypoint)
                {
                    targetWPIndex = i;
                    break;
                }
            }

            return targetWPIndex;
        }

        public bool WaypointIsLast(Waypoint waypoint)
        {
            // The index of the waypoint
            int wpIndex = GetWaypointIndex(waypoint);

            // The number of waypoints after this one
            int waypointsAfterCount = _waypoints.Count - wpIndex - 1;

            if (wpIndex == -1)
            {
                Debug.LogError("The waypoint does not belong to this path.");
                return false;
            }
            else if (waypointsAfterCount > 0)
            {
                return false;
            }

            return true;
        }

        public void UpdateWaypointNames()
        {
            UpdateWaypointNames(_waypoints.ToArray());
        }

        public void UpdateWaypointNames(Waypoint[] waypoints)
        {
            int runningNumber = 1;

            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] == null)
                {
                    continue;
                }

                // The name of the current waypoint
                string waypointName = string.Format("Waypoint{0}",
                    (runningNumber).ToString("D3"));

                // Adds a possible curve's name to
                // the end of the waypoint's name
                if (waypoints[i].IsPartOfCurve)
                {
                    waypointName += string.Format(" ({0})",
                        waypoints[i].CurveName);
                }

                // Sets the waypoint's name
                waypoints[i].name = waypointName;

                runningNumber++;
            }
        }

        /// <summary>
        /// Draws lines between waypoints
        /// </summary>
        protected void OnDrawGizmos()
        {
            if (_drawPath)
            {
                DrawPath();
                DrawWaypoints();
            }
        }

        private void DrawPath()
        {
            Gizmos.color = _pathColor;

            if (Waypoints.Count > 1)
            {
                for (int i = 1; i < Waypoints.Count; i++)
                {
                    // Draws a line from previous waypoint to current
                    Gizmos.DrawLine(Waypoints[i - 1].Position, Waypoints[i].Position);
                }
                if (_pathType == PathType.Loop)
                {
                    // From last waypoint to first 
                    Gizmos.DrawLine(Waypoints[Waypoints.Count - 1].Position,
                        Waypoints[0].Position);
                }
            }
        }

        private void DrawWaypoints()
        {
            Gizmos.color = _pathColor;

            if (Waypoints.Count > 1 && _waypointRadius > 0f)
            {
                for (int i = 1; i < Waypoints.Count; i++)
                {
                    DrawWaypointMarker
                        (Waypoints[i].Position, _waypointRadius);
                }
            }
        }

        private void DrawWaypointMarker(Vector3 position, float radius)
        {
            // Draw lines on x-axis
            Gizmos.DrawLine(position + Vector3.right * radius, position + Vector3.right * radius * 0.75f);
            Gizmos.DrawLine(position - Vector3.right * radius, position - Vector3.right * radius * 0.75f);

            // Draw lines on y-axis
            Gizmos.DrawLine(position + Vector3.up * radius, position + Vector3.up * radius * 0.75f);
            Gizmos.DrawLine(position - Vector3.up * radius, position - Vector3.up * radius * 0.75f);

            // Draw lines on z-axis
            Gizmos.DrawLine(position + Vector3.forward * radius, position + Vector3.forward * radius * 0.75f);
            Gizmos.DrawLine(position - Vector3.forward * radius, position - Vector3.forward * radius * 0.75f);
        }
    }
}
