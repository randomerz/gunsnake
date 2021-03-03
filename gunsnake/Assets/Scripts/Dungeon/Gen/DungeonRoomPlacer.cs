using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonRoomPlacer : MonoBehaviour
{
    public Tilemap floor;
    public Tilemap sideWall;
    public Tilemap topWall;
    public TileBase defaultFloor;
    public TileBase defaultSideWall;
    public TileBase defaultTopWall;


    public void PlaceComposite(RoomComposite composite, int x, int y, GameObject dungeonContainer = null)
    {
        if (dungeonContainer == null)
            dungeonContainer = GameObject.Find("DungeonContainer"); // maybe do a new GameObject() here

        Vector3Int offset = composite.basePos + new Vector3Int(x, y, 0);
        foreach (RCObj rco in composite.rooms)
        {
            GameObject roomGameObj = PlaceRoom(rco.roomData, offset.x + rco.pos.x, offset.y + rco.pos.y, dungeonContainer);
            Room room = roomGameObj.GetComponent<Room>();

            for (int doorInd = 0; doorInd < rco.connections.Count; doorInd++)
            {
                UpdateDoorWallCheck(room.doors[doorInd], rco.connections[doorInd], offset + rco.pos);
            }
        }
        foreach (HallwayObj h in composite.hallways)
        {
            PlaceHallway(h, offset);
        }
    }

    public bool CanPlaceRoom(RoomData room, int x, int y)
    {
        for (int r = y; r < y + room.height; r++)
        {
            for (int c = x; c < x + room.width; c++)
            {
                Vector3Int pos = new Vector3Int(c, r, 0);
                TileBase sideTile = sideWall.GetTile(pos);
                TileBase topTile = topWall.GetTile(pos);
                TileBase floorTile = floor.GetTile(pos);

                if (sideTile != null || topTile != null || floorTile != null)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public GameObject PlaceRoom(RoomData room, int x, int y, GameObject dungeonContainer=null)
    {
        if (dungeonContainer == null)
            dungeonContainer = GameObject.Find("DungeonContainer"); // maybe do a new GameObject() here

        string roomObjectPath = room.roomObjectPath;
        GameObject roomObj = Resources.Load<GameObject>(roomObjectPath);
        if (roomObj != null)
        {
            GameObject roomGameObj = Instantiate(roomObj, new Vector3(x, y), Quaternion.identity, dungeonContainer.transform);
            PlaceRoomTiles(room, x, y);
            return roomGameObj;
        }
        else
        {
            Debug.LogError("Error in creating room: No prefab in resources with name " + room.name + " at " + roomObjectPath + "!");
            return null;
        }
    }


    public void PlaceRoomTiles(RoomData room, int x, int y)
    {
        String2Room(room.roomString, x, y, room.width, room.height);
    }

    private void String2Room(string roomString, int x, int y, int width, int height)
    {
        if (roomString == null)
            roomString = "";
        char[] roomChar = roomString.ToCharArray();

        for (int r = 0; r < height; r++)
        {
            for (int c = 0; c < width; c++)
            {
                char t = roomChar[r * (width + 1) + c];
                Vector3Int pos = new Vector3Int(x + c, y + height - r - 1, 0);

                if (t == RoomData.WALLCHAR)
                {
                    sideWall.SetTile(pos, defaultSideWall);
                    topWall.SetTile(pos, defaultTopWall);
                }
                if (t == RoomData.FLOORCHAR)
                {
                    floor.SetTile(pos, defaultFloor);
                }
            }
        }
    }


    private void PlaceHallway(HallwayObj hallway, Vector3Int offset)
    {
        foreach (Vector3Int p in hallway.path)
        {
            PlaceHallwayWalls(offset + p);
        }
        foreach (Vector3Int p in hallway.path)
        {
            for (int r = -1; r <= 1; r++)
                for (int c = -1; c <= 1; c++)
                    ClearWallAddFloor(offset + p + new Vector3Int(c, r, 0));
        }
    }

    private void PlaceHallwayWalls(Vector3Int pos)
    {
        for (int i = -2; i <= 2; i++)
        {
            PlaceWallIfEmpty(pos + new Vector3Int(i, 2, 0));
            PlaceWallIfEmpty(pos + new Vector3Int(i, -2, 0));
            PlaceWallIfEmpty(pos + new Vector3Int(2, i, 0));
            PlaceWallIfEmpty(pos + new Vector3Int(-2, i, 0));
        }
    }

    private void PlaceWallIfEmpty(Vector3Int pos)
    {
        TileBase sideTile = sideWall.GetTile(pos);
        TileBase topTile = topWall.GetTile(pos);
        TileBase floorTile = floor.GetTile(pos);

        if (sideTile == null && topTile == null && floorTile == null)
        {
            sideWall.SetTile(pos, defaultSideWall);
            topWall.SetTile(pos, defaultTopWall);
        }
    }


    private void UpdateDoorWallCheck(Door door, RCConnection connection, Vector3Int offset)
    {
        bool shouldWall = connection.isWall || connection.isAvailable;
        door.SetIsWall(shouldWall);
        if (shouldWall)
        {
            sideWall.SetTile(offset + connection.pos, defaultSideWall);
            topWall.SetTile(offset + connection.pos, defaultTopWall);
            floor.SetTile(offset + connection.pos, null);

            if (door.isVertical)
            {
                sideWall.SetTile(offset + Vector3Int.up + connection.pos, defaultSideWall);
                topWall.SetTile(offset + Vector3Int.up + connection.pos, defaultTopWall);
                sideWall.SetTile(offset + Vector3Int.down + connection.pos, defaultSideWall);
                topWall.SetTile(offset + Vector3Int.down + connection.pos, defaultTopWall);
                floor.SetTile(offset + Vector3Int.up + connection.pos, null);
                floor.SetTile(offset + Vector3Int.down + connection.pos, null);
            }
            else
            {
                sideWall.SetTile(offset + Vector3Int.left + connection.pos, defaultSideWall);
                topWall.SetTile(offset + Vector3Int.left + connection.pos, defaultTopWall);
                sideWall.SetTile(offset + Vector3Int.right + connection.pos, defaultSideWall);
                topWall.SetTile(offset + Vector3Int.right + connection.pos, defaultTopWall);
                floor.SetTile(offset + Vector3Int.left + connection.pos, null);
                floor.SetTile(offset + Vector3Int.right + connection.pos, null);
            }
        }
    }


    private void ClearWallAddFloor(Vector3Int pos)
    {
        sideWall.SetTile(pos, null);
        topWall.SetTile(pos, null);
        floor.SetTile(pos, defaultFloor);
    }

    public void ClearTilemaps()
    {
        floor.ClearAllTiles();
        sideWall.ClearAllTiles();
        topWall.ClearAllTiles();
    }
}
