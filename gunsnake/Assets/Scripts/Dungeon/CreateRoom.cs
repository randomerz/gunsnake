using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.Tilemaps;

public class CreateRoom : MonoBehaviour
{
    public const int MAX_ROOM_SIZE = 100;

    public string roomName;
    public RoomType roomType;

    public bool isJungle;
    public bool isDungeon;
    public bool isTemple;

    public bool hasNorthDoor;
    public bool hasEastDoor;
    public bool hasSouthDoor;
    public bool hasWestDoor;

    public RoomData currentRoom;


    public Tilemap floor;
    public Tilemap sideWall;
    public Tilemap topWall;
    public TileBase defaultFloor;
    public TileBase defaultSideWall;
    public TileBase defaultTopWall;

    public GameObject defaultRoomGameObj;


    public void String2Room(string roomString, int width, int height)
    {
        if (roomString == null)
            roomString = "";
        char[] roomChar = roomString.ToCharArray();

        string s = "";
        ClearTilemaps();
        for (int r = 0; r < height; r++)
        {
            for (int c = 0; c < width; c++)
            {
                char t = roomChar[r * (width + 1) + c];

                if (t == RoomData.WALLCHAR)
                {
                    sideWall.SetTile(new Vector3Int(c, height - r - 1, 0), defaultSideWall);
                    topWall.SetTile(new Vector3Int(c, height - r - 1, 0), defaultTopWall);
                }
                if (t == RoomData.FLOORCHAR)
                {
                    floor.SetTile(new Vector3Int(c, height - r - 1, 0), defaultFloor);
                }
                s += t;
            }
            s += '\n';
        }

        Debug.Log(s);
    }

    //public void String2Room(string roomString)
    //{
    //    ClearTilemaps();
    //}

    public void String2Room()
    {
        String2Room(currentRoom.roomString, currentRoom.width, currentRoom.height);
    }

    public string Room2String()
    {
        string roomString = "";

        int width = 0;
        int height = 0;

        for (int r = 0; r < MAX_ROOM_SIZE; r++)
        {
            for (int c = 0; c < MAX_ROOM_SIZE; c++)
            {
                Vector3Int pos = new Vector3Int(c, MAX_ROOM_SIZE - r - 1, 0);
                TileBase sideTile = sideWall.GetTile(pos);
                TileBase topTile = topWall.GetTile(pos);
                TileBase floorTile = floor.GetTile(pos);

                if (sideTile != null || topTile != null || floorTile != null)
                {
                    width = Mathf.Max(width, c);
                    height = Mathf.Max(height, MAX_ROOM_SIZE - r - 1);
                }
            }
        }

        width++;
        height++;

        for (int r = 0; r < height; r++)
        {
            for (int c = 0; c < width; c++)
            {
                Vector3Int pos = new Vector3Int(c, height - r - 1, 0);
                TileBase sideTile = sideWall.GetTile(pos);
                TileBase topTile = topWall.GetTile(pos);
                TileBase floorTile = floor.GetTile(pos);

                if (sideTile != null || topTile != null)
                {
                    roomString += RoomData.WALLCHAR;
                    sideWall.SetTile(pos, defaultSideWall);
                    topWall.SetTile(pos, defaultTopWall);
                }
                else if (floorTile != null)
                {
                    roomString += RoomData.FLOORCHAR;
                    floor.SetTile(pos, defaultFloor);
                }
                else
                {
                    roomString += RoomData.EMPTYCHAR;
                }
            }
            roomString += '\n';
        }

        Debug.Log("Saved a room of width " + width + " and height " + height + "!");
        currentRoom.roomString = roomString;
        currentRoom.width = width;
        currentRoom.height = height;
        return roomString;
    }


    public void AddTops()
    {
        for (int r = 0; r < MAX_ROOM_SIZE; r++)
        {
            for (int c = 0; c < MAX_ROOM_SIZE; c++)
            {
                Vector3Int pos = new Vector3Int(c, MAX_ROOM_SIZE - r - 1, 0);
                TileBase sideTile = sideWall.GetTile(pos);
                TileBase topTile = topWall.GetTile(pos);

                if (sideTile != null || topTile != null)
                {
                    sideWall.SetTile(pos, defaultSideWall);
                    topWall.SetTile(pos, defaultTopWall);
                    floor.SetTile(pos, null);
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