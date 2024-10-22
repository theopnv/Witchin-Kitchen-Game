﻿#if UNITY_EDITOR
using UnityEditor;

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
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty(
                    "_MinX"),
                true);
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty(
                    "_MaxX"),
                true);
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty(
                    "_MinZ"),
                true);
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty(
                    "_MaxZ"),
                true);
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty(
                    "_ForbiddenAreas"),
                true);
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty(
                    "SpawnableItemsList"),
                true);
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty(
                    "SpawnFromStart"),
                true);
            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif