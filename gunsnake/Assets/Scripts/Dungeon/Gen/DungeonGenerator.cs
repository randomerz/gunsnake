using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    private const int MAX_TRIES = 3;

    public RoomFlow currentFlow;
    public RoomFlow[] flows;

    public bool drawGizmos;

    [Header("References")]
    public DungeonRoomPlacer roomPlacer;
    public DungeonRoomTable roomTable;

    public static GameObject dungeonContainer;
    // static instance


    private RoomComposite dungeonComposite;
    private List<List<FlowNode>> addedCycles;
    private Dictionary<FlowNode, RCObj> addedNodes;
    private List<FlowNode> remainingNodes;

    void Awake()
    {
        dungeonContainer = GameObject.Find("DungeonContainer");
        if (dungeonContainer == null)
            dungeonContainer = new GameObject("DungeonContainer");
    }

    private void Start()
    {

    }


    public RoomComposite CreateDungeon(RoomFlow flow)
    {
        Debug.Log("Creating dungeon!");
        ClearDungeon();
        //int seed = Random.Range(0, 32768);
        //seed = 32024;
        //Random.InitState(seed);
        //Debug.Log("Random seed: " + seed);

        //Debug.Log(flow.name);
        currentFlow = flow;
        currentFlow.Init();
        List<List<FlowNode>> cycles = FindCycles(currentFlow);
        cycles.Sort((a, b) => b.Count.CompareTo(a.Count)); // sort in decending order

        for (int numTries = 0; numTries < 100; numTries++)
        {
            if (TryGenerating(cycles))
            {
                Debug.Log("Created dungeon in " + numTries + " tries!");
                //dungeonComposite.PrintGrid();
                roomPlacer.PlaceComposite(dungeonComposite, 0, 0);
                return dungeonComposite;
            }
        }
        // if failed, dungeonComposite will still be null
        return null;
    }

    public RoomComposite CreateDungeon()
    {
        RoomFlow flow = flows[Random.Range(0, flows.Length)];
        return CreateDungeon(flow);
    }

    private bool TryGenerating(List<List<FlowNode>> cycles)
    {
        addedCycles = new List<List<FlowNode>>();
        addedNodes = new Dictionary<FlowNode, RCObj>();
        remainingNodes = new List<FlowNode>();
        foreach (FlowNode node in currentFlow.verticies)
            remainingNodes.Add(node.Copy());

        dungeonComposite = null;
        bool didGen = true;

        if (cycles.Count > 0)
        {
            // TODO: progress cycle a random amount
            if (Constants.doRandom)
            {
                int n = Random.Range(0, cycles.Count);
                for (int i = 0; i < n; i++)
                    cycles[0].Add(cycles[0][0]);
                cycles[0].RemoveRange(0, n);
            }

            FlowNode nodeInit = cycles[0][0];
            RoomData roomInit = roomTable.GetRoom(nodeInit.type);
            RCObj RCInit = new RCObj(roomInit);
            dungeonComposite = new RoomComposite(RCInit);

            AddNode(nodeInit, RCInit);
            dungeonComposite = AddCycleToComposite(dungeonComposite, RCInit, cycles[0]);
            if (dungeonComposite == null)
                return false;

            addedCycles.Add(cycles[0]);

            didGen = CheckOverlappingCycles(cycles);
            if (!didGen)
                return false;
        }
        else
        {
            FlowNode maxNode = remainingNodes[0];
            foreach (FlowNode node in remainingNodes)
                if (node.neighbors.Count > maxNode.neighbors.Count)
                    maxNode = node;

            RoomData roomInit = roomTable.GetRoom(maxNode.type);
            RCObj RCInit = new RCObj(roomInit);
            dungeonComposite = new RoomComposite(RCInit);
            didGen = AddNode(maxNode, RCInit);
            if (!didGen)
                return false;
        }

        //Debug.Log(remainingNodes.Count + " remaining!");

        int limitCount = 0;
        while (remainingNodes.Count > 0 && limitCount < 1000000)
        {
            limitCount++;

            List<FlowNode> frontierNodes = new List<FlowNode>();
            foreach (FlowNode node in addedNodes.Keys)
                foreach (FlowNode adj in node.neighbors)
                    if (remainingNodes.Contains(adj))
                        frontierNodes.Add(adj);

            FlowNode maxNode = remainingNodes[0];
            foreach (FlowNode node in frontierNodes)
                if (node.neighbors.Count > maxNode.neighbors.Count)
                    maxNode = node;

            FlowNode origNode = null;
            foreach (FlowNode node in addedNodes.Keys)
                if (node.neighbors.Contains(maxNode))
                    origNode = node;


            didGen = AddRoomRandom(dungeonComposite, addedNodes[origNode], maxNode);
            if (!didGen)
                return false;

            if (addedCycles.Count < cycles.Count)
            {
                didGen = CheckOverlappingCycles(cycles);
                if (!didGen)
                    return false;
            }
        }


        if (dungeonComposite == null || limitCount > 1000000)
            return false;

        return true;
    }

    #region Cycles

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

    private bool DoesCycleContain(List<List<FlowNode>> container, List<FlowNode> containee)
    {
        foreach (List<FlowNode> cycle in container)
            if (DoCyclesMatch(cycle, containee))
                return true;
        return false;
    }

    private bool DoCyclesMatch(List<FlowNode> a, List<FlowNode> b)
    {
        if (a.Count != b.Count)
            return false;
        for (int i = 0; i < a.Count; i++)
        {
            if (a[i] != b[i])
                return false;
        }
        return true;
    }

    private bool CheckOverlappingCycles(List<List<FlowNode>> cycles)
    {
        // check for overlapping nodes in other cycles 
        bool hadOverlap = true;
        while (hadOverlap)
        {
            bool properlyGenerated = true;
            hadOverlap = CheckOverlappingCyclesHelper(cycles, out properlyGenerated);
            if (!properlyGenerated) // THIS IS UNTESTED
            {
                //Debug.Log("Not properly generated!");
                return false;
            }
        }
        return true;
    }

    private bool CheckOverlappingCyclesHelper(List<List<FlowNode>> cycles, out bool properlyGenerated)
    {
        properlyGenerated = true;
        foreach (List<FlowNode> cycle in cycles)
        {
            if (DoesCycleContain(addedCycles, cycle))
                continue;

            foreach (FlowNode currentNode in addedNodes.Keys)
            {
                if (cycle.Contains(currentNode))
                {
                    while (cycle[0] != currentNode) // cycle until currentNode is in the front
                    {
                        cycle.Add(cycle[0]);
                        cycle.RemoveAt(0);
                    }
                    RoomComposite rc = AddCycleToComposite(dungeonComposite, addedNodes[currentNode], cycle);
                    if (rc == null)
                    {
                        properlyGenerated = false;
                        return false;
                    }
                    else
                    {
                        addedCycles.Add(cycle);
                        properlyGenerated = true;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private RoomComposite AddCycleToComposite(RoomComposite composite, RCObj RCA, List<FlowNode> cycle)
    {
        FlowNode nodeN = cycle[1];
        RoomData roomN = null;
        RCObj RCN = null;

        bool didAdd = false;

        if (!addedNodes.ContainsKey(cycle[1]))
        {
            // try adding first 2 rooms together
            for (int i = 0; i < MAX_TRIES; i++)
            {
                roomN = roomTable.GetRoom(nodeN.type);
                RCN = new RCObj(roomN);

                didAdd = TryCombiningRooms(composite, RCA, RCN, RCA.GetAvailableConnections());
                if (didAdd)
                {
                    if (!AddNode(nodeN, RCN))
                        return null; // couldn't generate complex hallway
                    break;
                }
            }

            // failed to place first two rooms
            if (!didAdd)
                return null;
        }
        else
        {
            RCN = addedNodes[nodeN];
        }


        // place rest of rooms
        for (int n = 2; n < cycle.Count; n++)
        {
            didAdd = false;

            FlowNode nodeM = cycle[n];

            if (!addedNodes.ContainsKey(nodeM))
            {
                int[] indsN;
                // get furthest dir
                if (n < (cycle.Count + 1) / 2)
                    indsN = RCN.GetFurthestConnection(RCA, 3);
                else // get closest dir
                    indsN = RCN.GetClosestConnections(RCA, 3);
                if (indsN[0] == -1)
                    continue;

                for (int i = 0; i < MAX_TRIES; i++)
                {
                    RoomData roomM = roomTable.GetRoom(nodeM.type);
                    RCObj RCM = new RCObj(roomM);

                    didAdd = TryCombiningRooms(composite, RCN, RCM, indsN);

                    if (didAdd)
                    {
                        if (!AddNode(nodeM, RCM))
                            return null; // couldn't generate complex hallway
                        RCN = RCM;
                        break;
                    }
                }

                if (!didAdd)
                    return null; // restart composite
            }
            else
            {
                // already added
                RCN = addedNodes[nodeM];
            }
        }

        //if (!composite.TryGenerateComplexHallway(RCN, RCA))
        //    return null; // couldnt do the last hallway :(

        // succeeded
        //Debug.Log("Rooms added successfully!");
        return composite;
    }

    #endregion

    private bool AddRoomRandom(RoomComposite composite, RCObj RCOrig, FlowNode nodeNew)
    {
        RoomData roomNew;
        RCObj RCNew;
        // try adding first 2 rooms together
        for (int i = 0; i < MAX_TRIES; i++)
        {
            roomNew = roomTable.GetRoom(nodeNew.type);
            RCNew = new RCObj(roomNew);

            bool didAdd = TryCombiningRooms(composite, RCOrig, RCNew, RCOrig.GetAvailableConnections());
            if (didAdd)
            {
                if (!AddNode(nodeNew, RCNew))
                    return false; // couldn't generate complex hallway
                return true;
            }
        }
        return false;
    }

    private bool TryCombiningRooms(RoomComposite composite, RCObj RCOrig, RCObj RCNew, int[] indsOrig)
    {
        foreach (int indA in indsOrig)
        {
            Direction dirA = RCOrig.connections[indA].side;
            Direction dirN = (Direction)(((int)dirA + 2) % 4);

            int[] indsN = RCNew.GetRandomConnections(dirN);

            foreach (int indN in indsN)
            {
                if (indA == -1 || indN == -1) // should be unneccesary
                    continue;

                bool didAdd = composite.Add(RCOrig, indA, RCNew, indN);
                if (didAdd)
                    return true;
            }
        }
        return false;
    }

    private bool AddNode(FlowNode node, RCObj rco)
    {
        addedNodes[node] = rco;
        remainingNodes.Remove(node);

        foreach (FlowNode adj in node.neighbors)
        {
            if (addedNodes.ContainsKey(adj))
            { 
                if (!rco.connectedObjects.Contains(addedNodes[adj]))
                {
                    bool didGen = dungeonComposite.TryGenerateComplexHallway(rco, addedNodes[adj]);

                    if (!didGen)
                    {
                        return false;
                    }
                }
            }
        }
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

    //public RoomData temp_startRoom;
    //public RoomData temp_middleRoom;
    //public RoomData temp_endRoom;

    //public void TestCreateDungeon()
    //{
    //    ClearDungeon();

    //    int curX = 0;

    //    // start room
    //    curX = -temp_startRoom.width / 2;

    //    PlaceRoom(temp_startRoom, curX, -temp_startRoom.height / 2);
    //    curX += temp_startRoom.width;


    //    int numCombat = Random.Range(4, 6);
    //    for (int i = 0; i < numCombat; i++)
    //    {
    //        PlaceRoom(temp_middleRoom, curX, -temp_middleRoom.height / 2);
    //        curX += temp_middleRoom.width;
    //    }


    //    PlaceRoom(temp_endRoom, curX, -temp_endRoom.height / 2);
    //    curX += temp_endRoom.width;
    //}

    //private RoomComposite ComposeCycle(List<FlowNode> cycle)
    //{
    //    RoomComposite cycleComposite = null;
    //    for (int tri = 0; tri < 100; tri++)
    //    {
    //        FlowNode nodeA = cycle[0];
    //        FlowNode nodeN = cycle[1];
    //        RoomData roomA = roomTable.GetRoom(nodeA.type);
    //        RoomData roomN = roomTable.GetRoom(nodeN.type);
    //        RCObj RCA = new RCObj(roomA);
    //        RCObj RCN = new RCObj(roomN);
    //        cycleComposite = new RoomComposite(RCA);

    //        // pick first pair of rooms
    //        bool didAdd = false;
    //        int randDir = Random.Range(0, 4);
    //        for (int i = 0; i < MAX_TRIES; i++)
    //        {
    //            int dirA = (randDir + i) % 4;
    //            int dirN = (randDir + i + 2) % 4;

    //            // TODO: pick random connecting points
    //            int indA = RCA.GetRandomConnection((Direction)dirA)[0];
    //            int indN = RCN.GetRandomConnection((Direction)dirN)[0];

    //            //Debug.Log("ind " + indA);

    //            if (indA == -1 || indN == -1)
    //                continue;

    //            didAdd = cycleComposite.Add(RCA, indA, RCN, indN);
    //            if (didAdd)
    //                break;
    //        }

    //        // failed to place first two rooms
    //        if (!didAdd)
    //            continue;

    //        //Debug.Log("adding rest of rooms");
    //        // place rest of rooms
    //        for (int n = 2; n < cycle.Count; n++)
    //        {
    //            didAdd = false;
    //            for (int i = 0; i < MAX_TRIES; i++)
    //            {
    //                FlowNode nodeM = cycle[n];
    //                RoomData roomM = roomTable.GetRoom(nodeM.type);
    //                RCObj RCM = new RCObj(roomM);

    //                int[] indNs = new int[] { -1 };
    //                // get furthest dir
    //                if (n < (cycle.Count + 1) / 2)
    //                    indNs = RCN.GetFurthestConnection(RCA, 3);
    //                else // get closest dir
    //                    indNs = RCN.GetClosestConnections(RCA, 3);
    //                if (indNs[0] == -1)
    //                    continue;
    //                int closestIndA = RCA.GetClosestConnections(RCN, 1)[0];

    //                for (int doorInd = 0; doorInd < indNs.Length; doorInd++)
    //                {
    //                    if (indNs[doorInd] == -1)
    //                        continue;

    //                    // trashy code
    //                    if (n >= (cycle.Count + 1) / 2 && // this is kinda hacky, prevents rooms from going far
    //                        RCN.connections[indNs[doorInd]].side == RCA.connections[closestIndA].side)
    //                        continue;

    //                    Direction d = (Direction)((int)(RCN.connections[indNs[doorInd]].side + 2) % 4);
    //                    // TODO get a list instead
    //                    int indM = RCM.GetRandomConnection(d)[0];

    //                    if (indM == -1)
    //                        continue;

    //                    didAdd = cycleComposite.Add(RCN, indNs[doorInd], RCM, indM);
    //                    if (didAdd)
    //                    {
    //                        // progress
    //                        RCN = RCM;
    //                        break;
    //                    }
    //                }
    //                if (didAdd)
    //                    break;
    //            }

    //            if (!didAdd)
    //                break; // restart trying
    //        }
    //        if (!didAdd)
    //            continue; // restart composite

    //        if (!cycleComposite.TryGenerateComplexHallway(RCN, RCA))
    //            continue;

    //        //Debug.Log("Rooms added successfully!");
    //        // succeeded
    //        Debug.Log("Succeeded in creating composite after " + tri + " attempts!");
    //        roomPlacer.PlaceComposite(cycleComposite, 0, 0); // temp
    //        return cycleComposite;
    //    }

    //    return null;
    //}

    #endregion
}
