using System.Collections.Generic;
using UnityEngine;

public class RCObj
{
    public RoomData roomData;
    public Vector3Int pos;
    public List<RCConnection> connections = new List<RCConnection>();
    public List<RCObj> connectedObjects = new List<RCObj>();

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

    private int[] Shuffle(int[] arr)
    {
        int temp;
        for (int i = 0; i < arr.Length; i++)
        {
            int rand = Random.Range(0, arr.Length);
            temp = arr[rand];
            arr[rand] = arr[i];
            arr[i] = temp;
        }
        return arr;
    }

    public int[] GetAvailableConnections()
    {
        List<int> valid = new List<int>();
        for (int i = 0; i < connections.Count; i++)
        {
            if (connections[i].isAvailable)
                valid.Add(i);
        }
        int[] shuffled = Shuffle(valid.ToArray());
        return shuffled;
    }

    public int[] GetRandomConnection(Direction dir)
    {
        List<int> valid = new List<int>();
        for (int i = 0; i < connections.Count; i++)
        {
            if (connections[i].side == dir && connections[i].isAvailable)
            {
                valid.Add(i);
            }
        }
        int[] shuffled = Shuffle(valid.ToArray());
        return shuffled;
    }

    public int[] GetFurthestConnection(RCObj other, int n)
    {
        List<System.Tuple<int, int>> distIndexList = new List<System.Tuple<int, int>>();

        for (int i = 0; i < connections.Count; i++)
        {
            if (!connections[i].isAvailable)
                continue;
            int maxManhattan = 0;
            int furthestInd = -1;

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

            distIndexList.Add(new System.Tuple<int, int>(maxManhattan, furthestInd));
        }
        distIndexList.Sort(); // TODO: randomize if the values are about the same
        distIndexList.Reverse();
        int[] sortedIndexes = new int[n];
        int count = 0;
        while (count < distIndexList.Count && count < n)
        {
            sortedIndexes[count] = distIndexList[count].Item2;
            count++;
        }
        while (count < n)
        {
            sortedIndexes[count] = -1;
            count++;
        }

        return sortedIndexes;
    }

    public int[] GetClosestConnections(RCObj other, int n)
    {
        List<System.Tuple<int, int>> distIndexList = new List<System.Tuple<int, int>>();

        for (int i = 0; i < connections.Count; i++)
        {
            if (!connections[i].isAvailable)
                continue;
            int minManhattan = 999999;
            int closestInd = -1;

            for (int j = 0; j < other.connections.Count; j++)
            {
                if (!other.connections[j].isAvailable)
                    continue;
                Vector3Int d = (pos + connections[i].pos) - (other.pos + other.connections[j].pos);
                int manhattan = Mathf.Abs(d.x) + Mathf.Abs(d.y);
                if (manhattan < minManhattan)
                {
                    minManhattan = manhattan;
                    closestInd = i;
                }
            }

            distIndexList.Add(new System.Tuple<int, int>(minManhattan, closestInd));
        }
        distIndexList.Sort(); // TODO: randomize if the values are about the same
        int[] sortedIndexes = new int[n];
        int count = 0;
        while (count < distIndexList.Count && count < n)
        {
            sortedIndexes[count] = distIndexList[count].Item2;
            count++;
        }
        while (count < n)
        {
            sortedIndexes[count] = -1;
            count++;
        }

        return sortedIndexes;
    }
}
