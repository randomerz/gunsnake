using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum RoomType
//{
//    normal,
//    challenge,
//    shop,
//    loot,
//    entrance,
//    exit,
//}

public class DungeonRoomTable : MonoBehaviour
{
    public TableEntry[] normalRooms;
    public TableEntry[] challengeRooms;
    public TableEntry[] shopRooms;
    public TableEntry[] lootRooms;
    public TableEntry[] entranceRooms;
    public TableEntry[] exitRooms;

    private TableEntry[][] tables;
    private float[] tableSums;

    [System.Serializable]
    public class TableEntry
    {
        public string name;
        public float freq = 1; // default 1, 0.5 = half as often, 2 = twice as often
        public RoomData roomData;
    }

    private void Start()
    {
        tables = new TableEntry[System.Enum.GetNames(typeof(RoomType)).Length][];
        tables[(int)RoomType.normal] = normalRooms;
        tables[(int)RoomType.challenge] = challengeRooms;
        tables[(int)RoomType.shop] = shopRooms;
        tables[(int)RoomType.loot] = lootRooms;
        tables[(int)RoomType.entrance] = entranceRooms;
        tables[(int)RoomType.exit] = exitRooms;

        tableSums = new float[tables.Length];
        for (int i = 0; i < tables.Length; i++)
        {
            float sum = 0;
            for (int j = 0; j < tables[i].Length; j++)
            {
                sum += tables[i][j].freq;
            }
            tableSums[i] = sum;
        }
    }

    public RoomData GetRoom(RoomType type)
    {
        //if (tables == null)
        //{
        //    Start();
        //}
        Start();
        TableEntry[] currTable = tables[(int)type];
        float currSum = tableSums[(int)type];

        float random = Random.Range(0, currSum);
        RoomData ret = null;

        for (int i = 0; i < currTable.Length; i++)
        {
            if (random < currTable[i].freq)
            {
                ret = currTable[i].roomData;
                break;
            }
            random -= currTable[i].freq;
        }

        return ret;
    }
}
