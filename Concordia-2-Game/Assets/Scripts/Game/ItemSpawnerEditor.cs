using System.Collections;
using System.Collections.Generic;
using con2.game;
using UnityEditor;
using UnityEngine;


namespace con2.game
{
    /// <summary>
    /// Custom editor to expose public fields of spawnable items in the
    /// spawnable items list. Because Unity doesn't do it by default.
    /// </summary>
    [CustomEditor(typeof(ItemSpawner))]
    public class ItemSpawnerEditorGUILayoutPropertyField : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty(
                    "Radius"),
                true);
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty(
                    "UseTimerMode"),
                true);
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty(
                    "SpawnableItems"),
                true);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
