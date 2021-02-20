using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;

[CustomEditor(typeof(DungeonGenerator))]
public class DungeonGenEditor : Editor
{
    //protected static bool showReferences;

    private DungeonGenerator _target;
    private GameObject dungeonContainer;

    private void OnEnable()
    {
        _target = (DungeonGenerator)target;
        dungeonContainer = GameObject.Find("DungeonContainer");
        if (dungeonContainer == null)
            dungeonContainer = new GameObject("DungeonContainer");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Clear Everything"))
        {
            Clear();
        }
        if (GUILayout.Button("Create New Dungeon!"))
        {
            _target.CreateDungeon();
        }

        // fold out
        //showReferences = EditorGUILayout.Foldout(showReferences, "References");

        //if (showReferences)
        //{
        //    EditorGUILayout.PropertyField(serializedObject.FindProperty("floor"));
        //    EditorGUILayout.PropertyField(serializedObject.FindProperty("sideWall"));
        //    EditorGUILayout.PropertyField(serializedObject.FindProperty("topWall"));
        //    EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultFloor"));
        //    EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultSideWall"));
        //    EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultTopWall"));
        //    EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultRoomGameObj"));
        //}

    }

    private void Clear()
    {
        _target.ClearDungeon();
    }
}
