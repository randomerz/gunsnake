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

    // should be normal rooms with 8+ exits, just looks at rooms with "hub" in name right now
    private List<TableEntry> hubRooms = new List<TableEntry>();
    private float hubSum;

    [System.Serializable]
    public class TableEntry
    {
        public string name;
        public float freq = 1; // default 1, 0.5 = half as often, 2 = twice as often
        public RoomData roomData;
    }

    private bool didInit = false;

    private void Start()
    {
        if (didInit)
            return;

        didInit = true;

        tables = new TableEntry[System.Enum.GetNames(typeof(RoomType)).Length][];
        tables[(int)RoomType.normal] = normalRooms;
        tables[(int)RoomType.challenge] = challengeRooms;
        tables[(int)RoomType.shop] = shopRooms;
        tables[(int)RoomType.loot] = lootRooms;
        tables[(int)RoomType.entrance] = entranceRooms;
        tables[(int)RoomType.exit] = exitRooms;

        for (int i = 0; i < normalRooms.Length; i++)
        {
            if (normalRooms[i].name.ToLower().Contains("hub"))
            {
                hubRooms.Add(normalRooms[i]);
                normalRooms[i].freq /= 10;
            }
        }

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

        float s = 0;
        for (int j = 0; j < hubRooms.Count; j++)
        {
            s += hubRooms[j].freq;
        }
        hubSum = s;
    }

    public RoomData GetRoom(RoomType type)
    {
        Start();

        //Debug.Log(type + " " + (int)type);
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

    public RoomData GetHubRoom()
    {
        Start();

        List<TableEntry> currTable = hubRooms;
        float currSum = hubSum;

        float random = Random.Range(0, currSum);
        RoomData ret = null;

        for (int i = 0; i < currTable.Count; i++)
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
