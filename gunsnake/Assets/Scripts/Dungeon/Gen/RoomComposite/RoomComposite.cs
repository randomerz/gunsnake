using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomComposite
{
    private const int MAX_SIZE = 300;
    private const int MID = 150;
    private const int MIN_HALLWAY = 5;
    private const int MAX_HALLWAY = 12;
    private const int SIMPLE_HALLWAY_LIMIT = 100;
    private const int COMPLEX_HALLWAY_LIMIT = 1000;
    
    public Vector3Int basePos; // value such that all Room's pos are >= 0
    public List<RCObj> rooms = new List<RCObj>(); // pos relative to basePos
    public List<HallwayObj> hallways = new List<HallwayObj>(); // pos relative to basePos

    private bool[,] grid = new bool[MAX_SIZE, MAX_SIZE]; // [x, y] = true if wall

    public RoomComposite(RCObj rco)
    {
        rooms.Add(rco);

        AddRoomToGrid(rco);
    }



    // assumes origRoom is already in rooms
    public bool Add(RCObj origRoom, int origConnectionIndex, RCObj newRoom, int newConnectionIndex)
    {
        if (!rooms.Contains(origRoom) || !origRoom.connections[origConnectionIndex].isAvailable
            || !newRoom.connections[newConnectionIndex].isAvailable)
        {
            Debug.LogWarning("Something went wrong when adding a room to composite!");
        }

        Direction dirOut = origRoom.connections[origConnectionIndex].side;
        Vector3Int nearestPos = origRoom.pos + origRoom.connections[origConnectionIndex].pos -
            newRoom.connections[newConnectionIndex].pos + MIN_HALLWAY * DirectionUtil.Convert(dirOut);

        HallwayObj hallway = new HallwayObj(origRoom, newRoom,
            origRoom.connections[origConnectionIndex], newRoom.connections[newConnectionIndex], true);

        for (int i = 0; i < MAX_HALLWAY; i++)
        {
            if (Constants.doRandom)
                if (Random.Range(0, 1f) < 0.3f)
                    continue;

            newRoom.pos = nearestPos + (i * DirectionUtil.Convert(dirOut));
            if (CanAddRoom(newRoom))
            {
                if (TryGenerateSimpleHallway(hallway))
                {
                    hallway.roomA.connectedObjects.Add(hallway.roomB);
                    hallway.roomB.connectedObjects.Add(hallway.roomA);

                    rooms.Add(newRoom);
                    origRoom.connections[origConnectionIndex].isAvailable = false;
                    newRoom.connections[newConnectionIndex].isAvailable = false;

                    AddRoomToGrid(newRoom);
                    AddHalwayToGrid(hallway);
                    UpdateDoorAvailability();
                    return true;
                }
            }
        }

        return false;
    }

    public void UpdateDoorAvailability()
    {
        foreach (RCObj rco in rooms)
        {
            foreach (RCConnection connection in rco.connections)
            {
                if (connection.isAvailable)
                {
                    //Debug.Log("Checking at " + (basePos + rco.pos + connection.pos));
                    if (IsConnectionBlocked(basePos + rco.pos + connection.pos, connection.side))
                    {
                        //Debug.Log("Made door unavailable at " + (basePos + rco.pos + connection.pos));
                        connection.isAvailable = false;
                        connection.isWall = true;
                    }
                }
            }
        }
    }

    private bool IsConnectionBlocked(Vector3Int pos, Direction dir)
    {
        // check 3x3 outside
        // #.......#
        // ###-P-###
        //    ???
        //    ???
        //    ???

        pos += new Vector3Int(MID, MID, 0);
        Vector3Int posP = pos + DirectionUtil.Convert(DirectionUtil.PrevDir(dir));
        Vector3Int posN = pos + DirectionUtil.Convert(DirectionUtil.NextDir(dir));
        Vector3Int dirVec = DirectionUtil.Convert(dir);

        for (int i = 0; i < 3; i++)
        {
            pos  += dirVec;
            posP += dirVec;
            posN += dirVec;

            if (grid[pos.x, pos.y] || grid[posP.x, posP.y] || grid[posN.x, posN.y])
            {
                return true;
            }
        }

        return false;
    }

    private bool CanAddRoom(RCObj rco)
    {
        int width = rco.roomData.width;
        int height = rco.roomData.height;

        for (int r = 0; r < height; r++)
            for (int c = 0; c < width; c += 7) // check every 7 columns maybe itll be faster
                if (grid[MID + basePos.x + rco.pos.x + c, MID + basePos.y + rco.pos.y + r])
                    return false;

        for (int r = 0; r < height; r++) // this might be wrong
            if (grid[MID + basePos.x + rco.pos.x + width - 1, MID + basePos.y + rco.pos.y + r])
                return false;

        return true;
    }

    private void AddRoomToGrid(RCObj rco)
    {
        int width = rco.roomData.width;
        int height = rco.roomData.height;
        char[] roomChar = rco.roomData.roomString.ToCharArray();

        for (int r = 0; r < height; r++)
        {
            for (int c = 0; c < width; c++)
            {
                char t = roomChar[r * (width + 1) + c];
                if (t == RoomData.WALLCHAR || t == RoomData.FLOORCHAR)
                {
                    // does this mirror rooms?
                    grid[MID + basePos.x + rco.pos.x + c, MID + basePos.y + rco.pos.y + (height - r - 1)] = true; 
                }
            }
        }
    }

    private bool TryGenerateSimpleHallway(HallwayObj hallway)
    {
        Vector3Int start = basePos + hallway.roomA.pos + hallway.connectionA.pos;
        Vector3Int end = basePos + hallway.roomB.pos + hallway.connectionB.pos;

        if (start.x != end.x && start.y != end.y)
        {
            Debug.LogWarning("Simple hallway couldn't be made because points aren't in a line!");
            return false;
        }

        List<Vector3Int> path = new List<Vector3Int>();
        Vector3Int dir = DirectionUtil.Convert(hallway.connectionA.side);
        Vector3Int pos = start + dir;
        int lim = 0;
        while (pos != end && lim < SIMPLE_HALLWAY_LIMIT)
        {
            if (grid[MID + pos.x, MID + pos.y])
            {
                return false;
            }

            path.Add(pos - basePos);
            pos += dir;
            lim += 1;
        }

        if (lim >= SIMPLE_HALLWAY_LIMIT)
        {
            Debug.LogWarning("Exceeded limit on simple path gen!");
            return false;
        }
        hallway.path = path;
        return true;
    }

    public bool TryGenerateComplexHallway(RCObj RCA, RCObj RCB)
    {
        int[] lastRoomInds = RCB.GetClosestConnections(RCA, 2);
        int[] firstRoomInds = RCA.GetClosestConnections(RCB, 2);

        if (lastRoomInds[0] == -1 || firstRoomInds[0] == -1) // no available connections, restart composite
            return false;

        for (int lastInd = 0; lastInd < 2; lastInd++)
        {
            if (lastRoomInds[lastInd] == -1)
                continue;
            for (int firstInd = 0; firstInd < 2; firstInd++)
            {
                if (firstRoomInds[firstInd] == -1)
                    continue;

                HallwayObj complexHallway = new HallwayObj(RCA, RCB,
                    RCA.connections[firstRoomInds[firstInd]], RCB.connections[lastRoomInds[lastInd]], false);
                if (TryGenerateComplexHallwayHelper(complexHallway))
                {
                    AddHalwayToGrid(complexHallway);
                    complexHallway.connectionA.isAvailable = false;
                    complexHallway.connectionB.isAvailable = false;
                    UpdateDoorAvailability();
                    RCA.connectedObjects.Add(RCB);
                    RCB.connectedObjects.Add(RCA);
                    return true;
                }
            }
        }

        return false;
    }

    private bool TryGenerateComplexHallwayHelper(HallwayObj hallway)
    {
        Vector3Int rawStart = basePos + hallway.roomA.pos + hallway.connectionA.pos;
        Vector3Int rawEnd = basePos + hallway.roomB.pos + hallway.connectionB.pos;

        Direction startDir = hallway.connectionA.side;
        Direction endDir = hallway.connectionB.side;

        Vector3Int start = rawStart + DirectionUtil.Convert(startDir);
        Vector3Int end = rawEnd + DirectionUtil.Convert(endDir);

        List<Vector3Int> path = new List<Vector3Int>();

        int lim = 0; // nodes expanded

        // pos, dir
        Queue<System.Tuple<Vector3Int, Direction>> frontier = new Queue<System.Tuple<Vector3Int, Direction>>();
        Dictionary<Vector3Int, Vector3Int> child2parent = new Dictionary<Vector3Int, Vector3Int>();

        frontier.Enqueue(new System.Tuple<Vector3Int, Direction>(start, startDir));
        child2parent[start] = start;
        bool foundPath = false;

        // BFS -- add direction first
        //   note: this should be an A* search but i don't want to implement a priority queue
        while (frontier.Count > 0 && lim < COMPLEX_HALLWAY_LIMIT)
        {
            System.Tuple<Vector3Int, Direction> node = frontier.Dequeue();
            Vector3Int pos = node.Item1;
            Direction dir = node.Item2;
            lim += 1;

            // go through neighbors in order of: dir => dir + 1 => dir - 1
            // check
            //   not in dictionary
            //   is legal spot (check new tiles on grid)
            //     if (grid[MID + pos.x, MID + pos.y])

            Direction nextDir = dir;
            if (ComplexHallwayHelperHelper(pos, nextDir, end, child2parent, frontier))
            {
                foundPath = true;
                break;
            }

            nextDir = DirectionUtil.NextDir(dir);
            if (ComplexHallwayHelperHelper(pos, nextDir, end, child2parent, frontier))
            {
                foundPath = true;
                break;
            }

            nextDir = DirectionUtil.PrevDir(dir);
            if (ComplexHallwayHelperHelper(pos, nextDir, end, child2parent, frontier))
            {
                foundPath = true;
                break;
            }

        }


        //if (lim >= COMPLEX_HALLWAY_LIMIT)
        //    Debug.LogWarning("Exceeded limit on complex path gen!");
        if (!foundPath)
            return false;

        //Debug.Log("Succeeded in generating complex hallway!");

        Vector3Int p = end;
        path.Add(p - basePos);
        while (p != start)
        {
            p = child2parent[p];
            path.Add(p - basePos);
        }


        hallway.path = path;
        return true;
    }

    // returns if found end
    private bool ComplexHallwayHelperHelper(Vector3Int pos, Direction dir, Vector3Int end,
        Dictionary<Vector3Int, Vector3Int> child2parent, Queue<System.Tuple<Vector3Int, Direction>> frontier)
    {
        Vector3Int nextPos = pos + DirectionUtil.Convert(dir);
        if (nextPos == end)
        {
            child2parent[nextPos] = pos;
            return true;
        }
        if (!child2parent.ContainsKey(nextPos) && ComplexHallwayCheckSpot(nextPos, dir))
        {
            frontier.Enqueue(new System.Tuple<Vector3Int, Direction>(nextPos, dir));
            child2parent[nextPos] = pos;
        }
        return false;
    }

    private bool ComplexHallwayCheckSpot(Vector3Int pos, Direction dir)
    {
        // Check these positions
        //   . . .
        //   . v .
        //   # # #

        pos += new Vector3Int(MID, MID, 0);
        Vector3Int posA = pos  + DirectionUtil.Convert(dir);
        Vector3Int posB = posA + DirectionUtil.Convert(DirectionUtil.NextDir(dir));
        Vector3Int posC = posA + DirectionUtil.Convert(DirectionUtil.PrevDir(dir));

        // hacky code
        if (pos.x < 1 || MAX_SIZE - 2 < pos.x || pos.y < 1 || MAX_SIZE - 2 < pos.y)
            return false;

        if (grid[posA.x, posA.y] || grid[posB.x, posB.y] || grid[posC.x, posC.y])
            return false;

        return true;
    }

    private void AddHalwayToGrid(HallwayObj hallway)
    {
        hallways.Add(hallway);

        for (int i = 1; i < hallway.path.Count - 1; i++)
        {
            Vector3Int pos = hallway.path[i] + basePos + new Vector3Int(MID, MID, 0);

            for (int x = -2; x <= 2; x++)
                for (int y = -2; y <= 2; y++)
                    grid[pos.x + x, pos.y + y] = true;
        }

        // need to check more than just one spot
        foreach (Vector3Int p in hallway.path)
        {
            grid[MID + basePos.x + p.x, MID + basePos.y + p.y] = true;
        }
    }



    #region Debug

    public void PrintGrid()
    {
        string s = "";
        for (int r = 100; r < MAX_SIZE - 100; r++) {
            for (int c = 100; c < MAX_SIZE - 100; c++)
            {
                if (grid[c, r])
                    s += "X";
                else
                    s += ".";
            }
            s += "\n";
        }
        Debug.Log(s);
    }

    #endregion
}
