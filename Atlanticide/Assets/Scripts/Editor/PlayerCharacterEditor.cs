using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Atlanticide.MyEditor
{
    [CustomEditor(typeof(PlayerCharacter))]
    public class PlayerCharacterEditor : Editor
    {
        private PlayerCharacter _playerTarget;

        /// <summary>
        /// Initializes the custom editor.
        /// </summary>
        private void OnEnable()
        {
            _playerTarget = target as PlayerCharacter;
        }

        //public override void OnInspectorGUI()
        //{
        //    base.OnInspectorGUI();

        //    string label = "Tool: " + _playerTarget.Tool.ToString();
        //    EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        //}
    }
}
