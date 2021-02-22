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

        currentFlow.Init();
        List<List<FlowNode>> cycles = FindCycles(currentFlow);

        foreach (List<FlowNode> cycle in cycles)
        {
            // TODO: progress cycle a random amount
            ComposeCycle(cycle);
            break;
        }

        //int x = -4;
        //foreach (FlowNode node in currentFlow.verticies)
        //{
        //    RoomData roomData = roomTable.GetRoom(node.type);

        //    //Debug.Log(node.name);
        //    roomPlacer.PlaceRoom(roomData, x, -roomData.height / 2, dungeonContainer);
        //    x += roomData.width;
        //}
    }

    private List<List<FlowNode>> FindCycles(RoomFlow flow)
    {
        List<List<FlowNode>> cycles = new List<List<FlowNode>>();

        Dictionary<FlowNode, FlowNode> child2parent = new Dictionary<FlowNode, FlowNode>();
        HashSet<FlowNode> explored = new HashSet<FlowNode>();
        Stack<FlowNode> frontier = new Stack<FlowNode>();

        frontier.Push(flow.verticies[0]);

        // DFS
        while (frontier.Count > 0)
        {
            FlowNode current = frontier.Pop();
            if (explored.Contains(current))
                continue;
            explored.Add(current);

            foreach (FlowNode adj in current.neighbors)
            {
                if (explored.Contains(adj))
                {
                    if (child2parent[current] == adj)
                        continue;
                    // create a cycle
                    // ** need to update if graph has one directional edges **
                    List<FlowNode> newCycle = new List<FlowNode>();
                    newCycle.Add(current);
                    FlowNode n = current;
                    while (n != adj)
                    {
                        n = child2parent[n];
                        if (current == null)
                        {
                            Debug.LogWarning("Null found while looking for cycles!");
                        }
                        newCycle.Add(n);
                    }
                    cycles.Add(newCycle);
                } 
                else
                {
                    child2parent[adj] = current;
                    frontier.Push(adj);
                }
            }
        }

        return cycles;
    }

    private bool ComposeCycle(List<FlowNode> cycle)
    {
        int MAX_TRIES = 3;

        RoomComposite cycleComposite = null;
        for (int tri = 0; tri < 20 + MAX_TRIES; tri++)
        {
            FlowNode nodeA = cycle[0];
            FlowNode nodeN = cycle[1];
            RoomData roomA = roomTable.GetRoom(nodeA.type);
            RoomData roomN = roomTable.GetRoom(nodeN.type);
            RCObj RCA = new RCObj(roomA);
            RCObj RCN = new RCObj(roomN);
            cycleComposite = new RoomComposite(RCA);

            // pick first pair of rooms
            bool didAdd = false;
            int randDir = Random.Range(0, 4);
            for (int i = 0; i < MAX_TRIES; i++)
            {
                int dirA = (randDir + i) % 4;
                int dirN = (randDir + i + 2) % 4;

                // TODO: pick random connecting points
                int indA = RCA.GetRandomConnection((Direction)dirA);
                int indN = RCN.GetRandomConnection((Direction)dirN);

                if (indA == -1 || indN == -1)
                    continue;

                didAdd = cycleComposite.Add(RCA, indA, RCN, indN);
                if (didAdd)
                    break;
            }

            // failed to place first two rooms
            if (!didAdd)
                continue;

            // place rest of rooms
            for (int n = 2; n < cycle.Count; n++)
            {
                didAdd = false;
                for (int i = 0; i < MAX_TRIES; i++)
                {
                    FlowNode nodeM = cycle[n];
                    RoomData roomM = roomTable.GetRoom(nodeM.type);
                    RCObj RCM = new RCObj(roomM);

                    int indN = -1;
                    // get furthest dir
                    if (n < (cycle.Count + 1) / 2)
                        indN = RCN.GetFurthestConnection(RCA);
                    else // get closest dir
                        indN = RCN.GetClosestConnection(RCA);
                    if (indN == -1)
                        continue;

                    Direction d = (Direction)((int)(RCN.connections[indN].side + 2) % 4);
                    //Debug.Log(n > (cycle.Count + 1) / 2);
                    //Debug.Log(tri + "/" + i + ": " + d);
                    int indM = RCM.GetRandomConnection(d);
                    if (indM == -1)
                        continue;

                    didAdd = cycleComposite.Add(RCN, indN, RCM, indM);
                    if (didAdd)
                    {
                        // progress
                        RCN = RCM;
                        break;
                    }
                }

                if (!didAdd)
                    break;
            }
            if (!didAdd)
                continue;

            break;
        }
        roomPlacer.PlaceComposite(cycleComposite, 0, 0);
        return true;
    }

    public void ClearDungeon()
    {
        if (dungeonContainer == null)
        {
            dungeonContainer = GameObject.Find("DungeonContainer");
        }
        //Debug.Log(dungeonContainer.transform.childCount);
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
            FlowNode n = currentFlow.verticies[i];
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
        foreach (FlowNode a in currentFlow.verticies)
        {
            foreach (FlowNode b in a.neighbors)
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
