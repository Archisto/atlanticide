using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Atlanticide.WaypointSystem;

namespace Atlanticide.MyEditor
{
    [CustomEditor(typeof(Curve))]
    public class CurveEditor : Editor
    {
        private Curve targetCurve;
        private Transform handleTransform;
        private Quaternion handleQuaternion;

        protected void OnEnable()
        {
            targetCurve = target as Curve;
            handleTransform = targetCurve.transform;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            FixEndsButton();
            ResetButton();
            CreateWaypointsButton();
            DestroyWaypointsButton();

            int createdWaypoints = (targetCurve.Waypoints == null ?
                0 : targetCurve.Waypoints.Length);
            EditorGUILayout.LabelField(
                string.Format("Waypoints created: {0}", createdWaypoints));
        }

        private void OnSceneGUI()
        {
            DisplayCurveHandle();
        }

        private void CreateWaypointsButton()
        {
            if (targetCurve.StartWaypoint != null &&
                targetCurve.EndWaypoint != null)
            {
                if (GUILayout.Button("Create Waypoints"))
                {
                    if (targetCurve.WaypointsCreated)
                    {
                        Debug.LogWarning("Be aware that waypoints created " +
                                         "from this curve may already exist.");
                    }

                    // The parent path
                    Path path = targetCurve.transform.
                    parent.parent.GetComponentInChildren<Path>();

                    Vector3[] waypointPositions =
                        new Vector3[targetCurve.LineSteps];

                    // Adds points to the waypoint position array
                    // (the start and end points are not included
                    // because they already exist as waypoints)
                    for (int i = 0; i < targetCurve.LineSteps; i++)
                    {
                        waypointPositions[i] = targetCurve.GetPoint
                            ((i + 1) / (float) (targetCurve.LineSteps + 1));
                    }

                    // Inserts waypoints on the curve
                    Waypoint[] newWaypoints = path.InsertWaypoints(targetCurve.StartWaypoint,
                        waypointPositions, targetCurve.name);
                    PassWaypointsToCurve(newWaypoints);
                }
            }
        }

        private void DestroyWaypointsButton()
        {
            if (targetCurve.WaypointsCreated)
            {
                if (GUILayout.Button
                        ("Destroy Waypoints Created from This Curve"))
                {
                    //bool alreadyDestroyed = false;

                    for (int i = 0; i < targetCurve.Waypoints.Length; i++)
                    {
                        if (targetCurve.Waypoints[i] != null)
                        {
                            DestroyImmediate
                                (targetCurve.Waypoints[i].gameObject);
                        }
                    }

                    targetCurve.Waypoints = null;

                    // The parent path
                    Path path = targetCurve.transform.
                        parent.parent.GetComponentInChildren<Path>();

                    // Updates each waypoint's name
                    path.UpdateWaypointNames();
                }
            }
        }

        private void PassWaypointsToCurve(Waypoint[] newWaypoints)
        {
            if (targetCurve.WaypointsCreated)
            {
                // The waypoints previously created from the curve
                List<Waypoint> oldWaypoints =
                    new List<Waypoint>(targetCurve.Waypoints.Length);
                for (int i = 0; i < targetCurve.Waypoints.Length; i++)
                {
                    if (targetCurve.Waypoints[i] != null)
                    {
                        oldWaypoints.Add(targetCurve.Waypoints[i]);
                    }
                }

                targetCurve.Waypoints = new Waypoint
                    [oldWaypoints.Count + newWaypoints.Length];

                for (int i = 0; i < oldWaypoints.Count; i++)
                {
                    targetCurve.Waypoints[i] = oldWaypoints[i];
                    //Debug.Log("old -> " + targetCurve.Waypoints[i].name);
                }
                for (int i = 0; i < newWaypoints.Length; i++)
                {
                    targetCurve.Waypoints[oldWaypoints.Count + i] = newWaypoints[i];
                    //Debug.Log("new -> " + targetCurve.Waypoints[oldWaypoints.Count + i].name);
                }
            }
            else
            {
                targetCurve.Waypoints = newWaypoints;
            }
        }

        private void FixEndsButton()
        {
            if (GUILayout.Button("Fix Ends"))
            {
                targetCurve.FixEnds();
            }
        }

        private void ResetButton()
        {
            if (GUILayout.Button("Reset"))
            {
                //targetCurve.ResetWhenPossible = true;
                targetCurve.ResetPoints();

                // Makes changes appear instantly in the Scene view
                // (a stupid trick I wish wasn't necessary)
                Vector3 temp = targetCurve.transform.position;
                targetCurve.transform.position += Vector3.up;
                targetCurve.transform.position = temp;
            }
        }

        private void DisplayCurveHandle()
        {
            // Sets the handle's rotation based on
            // if Local or Global mode is active
            handleQuaternion = Tools.pivotRotation == PivotRotation.Local ?
                handleTransform.rotation : Quaternion.identity;

            Vector3 p0 = ShowPoint(0, false);
            Vector3 p1 = ShowPoint(1, true);
            Vector3 p2 = ShowPoint(2, false);

            Handles.color = Color.gray;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p1, p2);

            Handles.color = Color.white;
            Vector3 lineStart = targetCurve.GetPoint(0f);
            for (int i = 0; i < targetCurve.LineSteps; i++)
            {
                Vector3 lineEnd = targetCurve.GetPoint
                    ((i + 1) / (float) (targetCurve.LineSteps + 1));
                Handles.DrawLine(lineStart, lineEnd);
                lineStart = lineEnd;
            }

            Handles.DrawLine(lineStart, targetCurve.GetPoint(1));
        }

        private Vector3 ShowPoint(int index, bool showHandle)
        {
            if (index >= targetCurve.Points.Length)
            {
                return Vector3.zero;
            }

            // Transforms the position into world coordinates
            Vector3 point = handleTransform.
                TransformPoint(targetCurve.Points[index]);

            if (showHandle)
            {
                EditorGUI.BeginChangeCheck();
                point = Handles.DoPositionHandle(point, handleQuaternion);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(targetCurve, "Move Point");
                    //EditorUtility.SetDirty(targetCurve);
                    targetCurve.Points[index] =
                        handleTransform.InverseTransformPoint(point);
                }
            }

            return point;
        }
    }
}
