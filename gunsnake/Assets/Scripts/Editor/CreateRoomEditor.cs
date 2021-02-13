using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;

[CustomEditor(typeof(CreateRoom))]
public class CreateRoomEditor : Editor
{
    private const string sObjPath = "Assets/Scriptable Objects/Rooms/";
    private const string gObjPath = "SObj Rooms/"; // make sure is in "Resources" folder
    private const string roomObjGameObj = "CurrentRoomObject";

    protected static bool showReferences;

    private CreateRoom _target;
    GameObject roomObjContainer;

    private void OnEnable()
    {
        _target = (CreateRoom)target;
        roomObjContainer = GameObject.Find(roomObjGameObj);
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
            RoomData newRoom = ScriptableObject.CreateInstance<RoomData>();

            string roomName = serializedObject.FindProperty("roomName").stringValue;
            AssetDatabase.CreateAsset(newRoom, sObjPath + roomName + ".asset");
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

        if (propObj.FindProperty("roomName").stringValue == "")
            serializedObject.FindProperty("roomName").stringValue = propObj.FindProperty("m_Name").stringValue;
        else
            serializedObject.FindProperty("roomName").stringValue = propObj.FindProperty("roomName").stringValue;
        serializedObject.FindProperty("roomType").enumValueIndex = propObj.FindProperty("roomType").enumValueIndex;
        serializedObject.FindProperty("hasNorthDoor").boolValue = propObj.FindProperty("hasNorthDoor").boolValue;
        serializedObject.FindProperty("hasEastDoor").boolValue = propObj.FindProperty("hasEastDoor").boolValue;
        serializedObject.FindProperty("hasSouthDoor").boolValue = propObj.FindProperty("hasSouthDoor").boolValue;
        serializedObject.FindProperty("hasWestDoor").boolValue = propObj.FindProperty("hasWestDoor").boolValue;


        string rName = serializedObject.FindProperty("roomName").stringValue;
        string roomObjectPath = propObj.FindProperty("roomObjectPath").stringValue;

        foreach (Transform t in roomObjContainer.GetComponentInChildren<Transform>())
            DestroyImmediate(t.gameObject); // maybe it can just be Destroy()

        GameObject roomObj = Resources.Load<GameObject>(roomObjectPath);
        if (roomObj != null)
        {
            Instantiate(roomObj, roomObjContainer.transform);
        }
        else
        {
            Debug.LogError("No prefab in resour ces with name " + rName + " at " + roomObjectPath + "!");
        }

        // loading into scene stuff
        _target.String2Room();

    }

    private void SaveData()
    {
        if (roomObjContainer.transform.childCount == 0)
        {
            Debug.LogError("No Room Game Object to save into room under " + roomObjGameObj + "!");
            return;
        }
        GameObject roomObj = roomObjContainer.transform.GetChild(0).gameObject;


        // serialized data i think
        SerializedProperty prop = serializedObject.FindProperty("currentRoom");
        SerializedObject propObj = new SerializedObject(prop.objectReferenceValue);

        propObj.FindProperty("roomName").stringValue = serializedObject.FindProperty("roomName").stringValue;
        propObj.FindProperty("roomType").enumValueIndex = serializedObject.FindProperty("roomType").enumValueIndex;
        propObj.FindProperty("hasNorthDoor").boolValue = serializedObject.FindProperty("hasNorthDoor").boolValue;
        propObj.FindProperty("hasEastDoor").boolValue = serializedObject.FindProperty("hasEastDoor").boolValue;
        propObj.FindProperty("hasSouthDoor").boolValue = serializedObject.FindProperty("hasSouthDoor").boolValue;
        propObj.FindProperty("hasWestDoor").boolValue = serializedObject.FindProperty("hasWestDoor").boolValue;

        // tried doing this stuff originally: 
        // https://answers.unity.com/questions/778647/objectreferencevalue-in-serializedproperty.html
        // switching to just saving with roomName

        string rName = serializedObject.FindProperty("roomName").stringValue;
        propObj.FindProperty("roomObjectPath").stringValue = gObjPath + rName;

        roomObj.name = rName;
        bool created = false;
        string pathName = "Assets/Resources/" + gObjPath + rName + ".prefab";
        // should i check overriding?
        PrefabUtility.SaveAsPrefabAssetAndConnect(roomObj, pathName, InteractionMode.UserAction, out created);



        propObj.ApplyModifiedProperties();

        _target.Room2String();
    }
}
