using System.Collections;
using UnityEngine;
using UnityEditor;
using Atlanticide.WaypointSystem;

namespace Atlanticide.MyEditor
{
    [CustomEditor(typeof(Path))]
    public class PathEditor : Editor
    {
        private Path targetPath;

        protected void OnEnable()
        {
            targetPath = target as Path;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            AddWaypointButton();
            FixWaypointNamesButton();
        }

        private void AddWaypointButton()
        {
            if (GUILayout.Button("Add Waypoint"))
            {
                targetPath.AddWaypoint();
            }
        }

        private void FixWaypointNamesButton()
        {
            if (GUILayout.Button("Fix Waypoint Names"))
            {
                targetPath.UpdateWaypointNames();
            }
        }
    }
}
