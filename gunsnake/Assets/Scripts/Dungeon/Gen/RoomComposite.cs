using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomComposite
{
    private const int MAX_SIZE = 300;
    private const int MID = 150;
    private bool DO_RANDOM = false;
    
    public Vector3Int basePos; // value such that all Room's pos are >= 0
    public List<RCObj> rooms = new List<RCObj>();
    public List<HallwayObj> hallways = new List<HallwayObj>();

    private bool[,] grid = new bool[MAX_SIZE, MAX_SIZE]; // [x, y]

    public RoomComposite(RCObj rco)
    {
        rooms.Add(rco);

        AddRoomToGrid(rco);
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
        Vector3Int end   = basePos + hallway.roomB.pos + hallway.connectionB.pos;

        if (start.x != end.x && start.y != end.y)
        {
            Debug.LogWarning("Simple hallway couldn't be made because points aren't in a line!");
            return false;
        }

        List<Vector3Int> path = new List<Vector3Int>();
        Vector3Int dir = DirectionToVector.Convert(hallway.connectionA.side);
        Vector3Int pos = start + dir;
        int lim = 0;
        while (pos != end && lim < 100)
        {
            if (grid[MID + pos.x, MID + pos.y])
            {
                return false;
            }

            path.Add(pos - basePos);
            pos += dir;
            lim += 1;
        }

        if (lim >= 100)
        {
            Debug.LogWarning("Exceeded limit on simple path gen!");
            return false;
        }
        hallway.path = path;
        return true;
    }

    private void AddHalwayToGrid(HallwayObj hallway)
    {
        hallways.Add(hallway);
        // need to check more than just one spot
        foreach (Vector3Int p in hallway.path)
        {
            grid[MID + basePos.x + p.x, MID + basePos.y + p.y] = true;
        }
    }

    // assumes origRoom is already in rooms
    public bool Add(RCObj origRoom, int origConnectionIndex, RCObj newRoom, int newConnectionIndex, 
        bool doSimpleConnect=true)
    {
        int MIN_HALLWAY = 6; // 1 means that doors overlap
        int MAX_HALLWAY = 12;

        if (!rooms.Contains(origRoom) || !origRoom.connections[origConnectionIndex].isAvailable
            || !newRoom.connections[newConnectionIndex].isAvailable)
        {
            Debug.LogWarning("Something went wrong when adding a room to composite!");
        }

        Direction dirOut = origRoom.connections[origConnectionIndex].side;
        Vector3Int nearestPos = origRoom.pos + origRoom.connections[origConnectionIndex].pos -
            newRoom.connections[newConnectionIndex].pos + MIN_HALLWAY * DirectionToVector.Convert(dirOut);

        HallwayObj hallway = new HallwayObj(origRoom, newRoom,
            origRoom.connections[origConnectionIndex], newRoom.connections[newConnectionIndex], true);

        for (int i = 0; i < MAX_HALLWAY; i++)
        {
            if (DO_RANDOM)
                if (Random.Range(0, 1f) < 0.2f)
                    continue;

            newRoom.pos = nearestPos + (i * DirectionToVector.Convert(dirOut));
            if (CanAddRoom(newRoom))
            {
                if (TryGenerateSimpleHallway(hallway))
                {
                    //Debug.Log("Added new room to grid!");

                    rooms.Add(newRoom);
                    origRoom.connections[origConnectionIndex].isAvailable = false;
                    newRoom.connections[newConnectionIndex].isAvailable = false;

                    AddRoomToGrid(newRoom);
                    AddHalwayToGrid(hallway);
                    return true;
                }
            }
        }

        return false;
    }
}


public class RCObj
{
    public RoomData roomData;
    public Vector3Int pos;
    public List<RCConnection> connections = new List<RCConnection>();

    public RCObj(RoomData r)
    {
        roomData = r;
        pos = new Vector3Int(0, 0, 0);
        connections = new List<RCConnection>(roomData.GetDefaultConnections());
    }

    public RCObj(RoomData r, Vector3Int p)
    {
        roomData = r;
        pos = p;
        connections = new List<RCConnection>(roomData.GetDefaultConnections());
    }

    public int GetRandomConnection(Direction dir)
    {
        List<int> valid = new List<int>();
        for (int i = 0; i < connections.Count; i++)
        {
            if (connections[i].side == dir && connections[i].isAvailable)
            {
                valid.Add(i);
            }
        }
        if (valid.Count == 0)
            return -1;
        int rand = Random.Range(0, valid.Count);
        return valid[rand];
    }

    public int GetFurthestConnection(RCObj other)
    {
        int maxManhattan = -1;
        int furthestInd = -1;
        for (int i = 0; i < connections.Count; i++)
        {
            if (!connections[i].isAvailable)
                continue;
            for (int j = 0; j < other.connections.Count; j++)
            {
                if (!other.connections[j].isAvailable)
                    continue;
                Vector3Int d = (pos + connections[i].pos) - (other.pos + other.connections[j].pos);
                int manhattan = Mathf.Abs(d.x) + Mathf.Abs(d.y);
                if (manhattan > maxManhattan)
                {
                    maxManhattan = manhattan;
                    furthestInd = i;
                }
            }
        }
        return furthestInd;
    }

    public int GetClosestConnection(RCObj other)
    {
        int minManhattan = 999999;
        int furthestInd = -1;
        for (int i = 0; i < connections.Count; i++)
        {
            if (!connections[i].isAvailable)
                continue;
            for (int j = 0; j < other.connections.Count; j++)
            {
                if (!other.connections[j].isAvailable)
                    continue;
                Vector3Int d = (pos + connections[i].pos) - (other.pos + other.connections[j].pos);
                int manhattan = Mathf.Abs(d.x) + Mathf.Abs(d.y);
                if (manhattan < minManhattan)
                {
                    minManhattan = manhattan;
                    furthestInd = i;
                }
            }
        }
        return furthestInd;
    }
}

[System.Serializable]
public class RCConnection
{
    public Vector3Int pos;
    public Direction side;
    public bool isAvailable;

    public RCConnection(Vector3Int relativePosition, Direction relativeSide)
    {
        pos = relativePosition;
        side = relativeSide;
        isAvailable = true;
    }

    public RCConnection(Vector3Int relativePosition, Direction relativeSide, bool isAvail)
    {
        pos = relativePosition;
        side = relativeSide;
        isAvailable = isAvail;
    }

    public RCConnection Copy()
    {
        return new RCConnection(pos, side, isAvailable);
    }
}


public class HallwayObj
{
    public RCObj roomA;
    public RCObj roomB;
    public RCConnection connectionA;
    public RCConnection connectionB;
    public bool isPathStraight;
    public List<Vector3Int> path = new List<Vector3Int>(); // exclusive

    public HallwayObj(RCObj RCA, RCObj RCB, RCConnection conA, RCConnection conB, bool isStraight)
    {
        roomA = RCA;
        roomB = RCB;
        connectionA = conA;
        connectionB = conB;
        isPathStraight = isStraight;
    }
}