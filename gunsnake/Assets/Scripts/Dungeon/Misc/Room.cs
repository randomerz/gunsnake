using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : ScriptableObject
{
    public enum RoomType
    {
        combat,
        shop,
        loot,
        entrance,
        exit,
    }

    [Header("Manually set")]
    public string roomName;
    public RoomType roomType;

    public bool hasNorthDoor;
    public bool hasEastDoor;
    public bool hasSouthDoor;
    public bool hasWestDoor;

    [Header("Automatically set")]
    public GameObject roomObject;

    [TextArea()]
    public string roomString;
    public int width;
    public int height;

    public static char WALLCHAR = '#';
    public static char FLOORCHAR = '.';
    public static char EMPTYCHAR = ' ';

    public void SetData(Room other)
    {
        roomName = other.roomName;
        roomType = other.roomType;
        hasNorthDoor = other.hasNorthDoor;
        hasEastDoor = other.hasEastDoor;
        hasSouthDoor = other.hasSouthDoor;
        hasWestDoor = other.hasWestDoor;

        roomObject = other.roomObject;
        roomString = other.roomString;
        width = other.width;
        height = other.height;
    }
}
