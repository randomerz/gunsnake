using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FlowNode
{
    public string name;
    public RoomType type;
    [NonSerialized]
    public List<FlowNode> neighbors = new List<FlowNode>();
}

[CreateAssetMenu(fileName = "New Room Flow", menuName = "Dungeon/Room Flow")]
public class RoomFlow : ScriptableObject
{
    [Header("INPUT THESE")]
    public List<FlowNode> verticies = new List<FlowNode>();
    public List<Edge> edges = new List<Edge>();

    private Dictionary<string, FlowNode> string2node = new Dictionary<string, FlowNode>();

    [System.Serializable]
    public struct Edge
    {
        public string nodeA;
        public string nodeB;
        public bool isOneWay;
    }


    public void Init()
    {
        for (int i = 0; i < verticies.Count; i++)
        {
            FlowNode n = verticies[i];
            n.neighbors = new List<FlowNode>();
            if (!string2node.ContainsKey(n.name))
                string2node.Add(n.name, n);
        }
        foreach (Edge e in edges)
        {
            AddEdge(string2node[e.nodeA], string2node[e.nodeB], e.isOneWay);
        }
    }

    private void AddVertex(FlowNode Key)
    {
        verticies.Add(Key);
    }

    public void AddEdge(FlowNode startKey, FlowNode endKey, bool isOneWay = false)
    {
        if (!startKey.neighbors.Contains(endKey))
            startKey.neighbors.Add(endKey);
        if (!isOneWay && !endKey.neighbors.Contains(startKey))
        {
            endKey.neighbors.Add(startKey);
        }
    }

    public void RemoveVertex(FlowNode Key)
    {
        //First remove the edges / adjacency entries
        int vertexNumAdjacent = Key.neighbors.Count;
        for (int i = 0; i < vertexNumAdjacent; i++)
        {
            FlowNode neighbourVertexKey = Key.neighbors[i];
            RemoveEdge(Key, neighbourVertexKey);
        }

        //Lastly remove the vertex / adj. list
        verticies.Remove(Key);
    }

    public void RemoveEdge(FlowNode startKey, FlowNode endKey)
    {
        startKey.neighbors.Remove(endKey);
        endKey.neighbors.Remove(startKey);
    }

    public bool Contains(FlowNode Key)
    {
        return verticies.Contains(Key);
    }

    public int VertexDegree(FlowNode Key)
    {
        return Key.neighbors.Count;
    }
}
