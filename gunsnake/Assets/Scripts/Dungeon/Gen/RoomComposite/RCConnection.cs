using UnityEngine;

[System.Serializable]
public class RCConnection
{
    public Vector3Int pos;
    public Direction side;
    public bool isAvailable;
    public bool isWall;

    public RCConnection(Vector3Int relativePosition, Direction relativeSide)
    {
        pos = relativePosition;
        side = relativeSide;
        isAvailable = true;
    }

    public RCConnection(Vector3Int relativePosition, Direction relativeSide, bool isAvail)
    {
        pos = relativePosition;
        side = relativeSide;
        isAvailable = isAvail;
    }

    public RCConnection Copy()
    {
        return new RCConnection(pos, side, isAvailable);
    }
}
