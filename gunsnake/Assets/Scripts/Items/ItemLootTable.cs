using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// i dont know how to make this LootTable<T>
public class ItemLootTable : MonoBehaviour
{
    public TableEntry[] weapons;
    public TableEntry[] artifacts;

    private TableEntry[][] tables;
    private float[] tableSums;

    [System.Serializable]
    public class TableEntry
    {
        public string name;
        public float freq = 1; // default 1, 0.5 = half as often, 2 = twice as often
        public Item value;
    }

    private void Awake()
    {
        tables = new TableEntry[2][]; // items are either weapon or artifact
        tables[0] = weapons;
        tables[1] = artifacts;

        UpdateSums();
    }

    private void UpdateSums()
    {
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

    public Item GetEntry(int tableIndex) // make this better lol
    {
        TableEntry[] currTable = tables[tableIndex];
        float currSum = tableSums[tableIndex];

        float random = Random.Range(0, currSum);
        Item ret = default(Item);

        for (int i = 0; i < currTable.Length; i++)
        {
            if (random < currTable[i].freq)
            {
                ret = currTable[i].value;
                break;
            }
            random -= currTable[i].freq;
        }

        return ret;
    }

}
