using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public RoomFlow currentFlow;


    public bool drawGizmos;

    [Header("References")]
    public DungeonRoomPlacer roomPlacer;
    public DungeonRoomTable roomTable;

    public static GameObject dungeonContainer;
    // static instance

    void Awake()
    {
        dungeonContainer = GameObject.Find("DungeonContainer");
        if (dungeonContainer == null)
            dungeonContainer = new GameObject("DungeonContainer");
    }

    private void Start()
    {
        CreateDungeon();
    }


    public void CreateDungeon()
    {
        ClearDungeon();

        int x = -4;
        foreach (RoomFlow.FlowNode node in currentFlow.verticies)
        {
            RoomData roomData = roomTable.GetRoom(node.type);

            //Debug.Log(node.name);
            roomPlacer.PlaceRoom(roomData, x, -roomData.height / 2, dungeonContainer);
            x += roomData.width;
        }
    }


    public void ClearDungeon()
    {
        if (dungeonContainer == null)
        {
            dungeonContainer = GameObject.Find("DungeonContainer");
        }
        Debug.Log(dungeonContainer.transform.childCount);
        for (int i = dungeonContainer.transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(dungeonContainer.transform.GetChild(i).gameObject);
        roomPlacer.ClearTilemaps();
    }

    private void PlaceRoom(RoomData room, int x, int y)
    {
        string roomObjectPath = room.roomObjectPath;

        GameObject roomObj = Resources.Load<GameObject>(roomObjectPath);
        if (roomObj != null)
        {
            Instantiate(roomObj, new Vector3(x, y), Quaternion.identity, dungeonContainer.transform);
            roomPlacer.PlaceRoomTiles(room, x, y);
        }
        else
        {
            Debug.LogError("Error in creating room: No prefab in resources with name " + room.name + " at " + roomObjectPath + "!");
        }
    }


    #region Gizmos

    private void OnDrawGizmos()
    {
        if (!drawGizmos || currentFlow == null)
            return;

        currentFlow.Init();

        Dictionary<string, Vector3> name2pos = new Dictionary<string, Vector3>();
        for (int i = 0; i < currentFlow.verticies.Count; i++)
        {
            RoomFlow.FlowNode n = currentFlow.verticies[i];
            switch (n.type)
            {
                case RoomType.entrance:
                    Gizmos.color = Color.cyan;
                    break;
                case RoomType.exit:
                    Gizmos.color = Color.green;
                    break;
                case RoomType.normal:
                    Gizmos.color = Color.red;
                    break;
                case RoomType.challenge:
                    Gizmos.color = Color.black;
                    break;
                case RoomType.shop:
                    Gizmos.color = new Color(1, .5f, 0); // orange
                    break;
                case RoomType.loot:
                    Gizmos.color = Color.yellow;
                    break;
            }
            float ang = ((float)i / currentFlow.verticies.Count) * 360;
            Vector3 pos = 4 * new Vector3(Mathf.Cos(ang), Mathf.Sin(ang));
            Vector3 offset = 4.5f * new Vector3(1, 1);
            name2pos.Add(n.name, pos + offset);

            Gizmos.DrawSphere(pos + offset, 0.25f);
            Handles.Label(pos + offset + new Vector3(0.5f, 0.25f), n.name);
        }

        Gizmos.color = Color.black;
        foreach (RoomFlow.FlowNode a in currentFlow.verticies)
        {
            foreach (RoomFlow.FlowNode b in a.neighbors)
            {
                Gizmos.DrawLine(name2pos[a.name], name2pos[b.name]);
            }
        }
    }

    #endregion

    #region Old

    public RoomData temp_startRoom;
    public RoomData temp_middleRoom;
    public RoomData temp_endRoom;

    public void TestCreateDungeon()
    {
        ClearDungeon();

        int curX = 0;

        // start room
        curX = -temp_startRoom.width / 2;

        PlaceRoom(temp_startRoom, curX, -temp_startRoom.height / 2);
        curX += temp_startRoom.width;


        int numCombat = Random.Range(4, 6);
        for (int i = 0; i < numCombat; i++)
        {
            PlaceRoom(temp_middleRoom, curX, -temp_middleRoom.height / 2);
            curX += temp_middleRoom.width;
        }


        PlaceRoom(temp_endRoom, curX, -temp_endRoom.height / 2);
        curX += temp_endRoom.width;
    }

    #endregion
}
