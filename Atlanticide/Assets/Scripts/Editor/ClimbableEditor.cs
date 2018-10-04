using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Atlanticide.MyEditor
{
    [CustomEditor(typeof(Climbable))]
    public class ClimbableEditor : Editor
    {
        private Climbable _climbableTarget;

        /// <summary>
        /// Initializes the custom editor.
        /// </summary>
        private void OnEnable()
        {
            _climbableTarget = target as Climbable;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }

        public void OnSceneGUI()
        {
            EditorGUI.BeginChangeCheck();
            Vector3 pos1 = Handles.PositionHandle(_climbableTarget.TopPointWorldSpace, Quaternion.identity);
            Vector3 pos2 = Handles.PositionHandle(_climbableTarget.BottomPointWorldSpace, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Move point");
                _climbableTarget.SetPoints(pos1, pos2);
            }
        }
    }
}
