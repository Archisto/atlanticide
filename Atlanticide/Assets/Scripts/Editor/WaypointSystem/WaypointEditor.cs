using System.Collections;
using UnityEngine;
using UnityEditor;
using Atlanticide.WaypointSystem;

namespace Atlanticide.MyEditor
{
    [CustomEditor(typeof(Waypoint))]
    public class WaypointEditor : Editor
    {
        private Waypoint targetWaypoint;

        protected void OnEnable()
        {
            targetWaypoint = target as Waypoint;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            AddWaypointButton();
        }

        private void AddWaypointButton()
        {
            if (GUILayout.Button("Add a New Waypoint After This"))
            {
                // The parent path
                Path parentPath = targetWaypoint.GetComponentInParent<Path>();

                bool lastWaypoint = parentPath.WaypointIsLast(targetWaypoint);

                Waypoint newWaypoint = null;

                // Adds a new waypoint
                if (lastWaypoint)
                {
                    newWaypoint = parentPath.AddWaypoint();
                }
                // Inserts a new waypoint
                else
                {
                    newWaypoint = parentPath.InsertWaypoint(targetWaypoint);
                }

                if (newWaypoint != null)
                {
                    // Selects the new waypoint in editor
                    Selection.activeGameObject = newWaypoint.gameObject;
                }
            }
        }
    }
}
