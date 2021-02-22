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
        foreach (RCObj o in composite.rooms)
        {
            PlaceRoom(o.roomData, offset.x + o.pos.x, offset.y + o.pos.y, dungeonContainer);
        }
        foreach (HallwayObj h in composite.hallways)
        {
            Debug.Log("doing hallway!");
            foreach (Vector3Int p in h.path)
            {
                floor.SetTile(offset + p, defaultFloor);
            }
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

    public void PlaceRoom(RoomData room, int x, int y, GameObject dungeonContainer=null)
    {
        string roomObjectPath = room.roomObjectPath;
        if (dungeonContainer == null)
            dungeonContainer = GameObject.Find("DungeonContainer"); // maybe do a new GameObject() here

        GameObject roomObj = Resources.Load<GameObject>(roomObjectPath);
        if (roomObj != null)
        {
            Instantiate(roomObj, new Vector3(x, y), Quaternion.identity, dungeonContainer.transform);
            PlaceRoomTiles(room, x, y);
        }
        else
        {
            Debug.LogError("Error in creating room: No prefab in resources with name " + room.name + " at " + roomObjectPath + "!");
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

    public void ClearTilemaps()
    {
        floor.ClearAllTiles();
        sideWall.ClearAllTiles();
        topWall.ClearAllTiles();
    }
}
