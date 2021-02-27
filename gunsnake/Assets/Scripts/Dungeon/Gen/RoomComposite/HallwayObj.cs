using System.Collections.Generic;
using UnityEngine;

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