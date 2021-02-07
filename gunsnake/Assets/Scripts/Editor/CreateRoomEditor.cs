using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;

[CustomEditor(typeof(CreateRoom))]
public class CreateRoomEditor : Editor
{
    protected static bool showReferences;
    
    private CreateRoom _target;
    private const string objPath = "Assets/Scriptable Objects/Rooms/";

    private void OnEnable()
    {
        _target = (CreateRoom)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("roomName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("roomType"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("hasNorthDoor"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("hasEastDoor"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("hasSouthDoor"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("hasWestDoor"));


        if (GUILayout.Button("Add Tops"))
        {
            _target.AddTops();
        }
        if (GUILayout.Button("Create New Room from Scene!"))
        {
            Room newRoom = ScriptableObject.CreateInstance<Room>();

            string roomName = serializedObject.FindProperty("roomName").stringValue;
            AssetDatabase.CreateAsset(newRoom, objPath + roomName + ".asset");
            //AssetDatabase.CreateAsset(newRoom, objPath + "tempRoom.asset");
            AssetDatabase.SaveAssets();
            EditorGUIUtility.PingObject(newRoom);

            serializedObject.FindProperty("currentRoom").objectReferenceValue = newRoom;
            serializedObject.ApplyModifiedProperties();

            SaveData();
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("currentRoom"));

        if (GUILayout.Button("Load Current Room Data"))
        {
            LoadData();
        }
        if (GUILayout.Button("Save Data into Current Room"))
        {
            SaveData();
        }


        // fold out
        showReferences = EditorGUILayout.Foldout(showReferences, "References");

        if (showReferences)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("floor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sideWall"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("topWall"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultFloor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultSideWall"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultTopWall"));
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void LoadData()
    {
        SerializedProperty prop = serializedObject.FindProperty("currentRoom");
        SerializedObject propObj = new SerializedObject(prop.objectReferenceValue);

        serializedObject.FindProperty("roomName").stringValue = propObj.FindProperty("roomName").stringValue;
        serializedObject.FindProperty("roomType").enumValueIndex = propObj.FindProperty("roomType").enumValueIndex;
        serializedObject.FindProperty("hasNorthDoor").boolValue = propObj.FindProperty("hasNorthDoor").boolValue;
        serializedObject.FindProperty("hasEastDoor").boolValue = propObj.FindProperty("hasEastDoor").boolValue;
        serializedObject.FindProperty("hasSouthDoor").boolValue = propObj.FindProperty("hasSouthDoor").boolValue;
        serializedObject.FindProperty("hasWestDoor").boolValue = propObj.FindProperty("hasWestDoor").boolValue;

        // loading into scene stuff
        _target.String2Room();

    }

    private void SaveData()
    {
        SerializedProperty prop = serializedObject.FindProperty("currentRoom");
        SerializedObject propObj = new SerializedObject(prop.objectReferenceValue);

        propObj.FindProperty("roomName").stringValue = serializedObject.FindProperty("roomName").stringValue;
        propObj.FindProperty("roomType").enumValueIndex = serializedObject.FindProperty("roomType").enumValueIndex;
        propObj.FindProperty("hasNorthDoor").boolValue = serializedObject.FindProperty("hasNorthDoor").boolValue;
        propObj.FindProperty("hasEastDoor").boolValue = serializedObject.FindProperty("hasEastDoor").boolValue;
        propObj.FindProperty("hasSouthDoor").boolValue = serializedObject.FindProperty("hasSouthDoor").boolValue;
        propObj.FindProperty("hasWestDoor").boolValue = serializedObject.FindProperty("hasWestDoor").boolValue;

        propObj.ApplyModifiedProperties();

        _target.Room2String();
    }
}
