using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    right,
    down,
    left,
    up,
}

public static class DirectionUtil
{
    public static Vector3Int Convert(Direction dir)
    {
        switch (dir)
        {
            case Direction.right:
                return Vector3Int.right;
            case Direction.down:
                return Vector3Int.down;
            case Direction.left:
                return Vector3Int.left;
            case Direction.up:
                return Vector3Int.up;
        }
        return Vector3Int.zero;
    }

    public static Direction NextDir(Direction dir)
    {
        return (Direction)(((int)dir + 1) % 4);
    }

    public static Direction PrevDir(Direction dir)
    {
        return (Direction)(((int)dir + 3) % 4);
    }
}

public class Entity : MonoBehaviour
{
    protected static LayerMask wallLayerMask;
    protected static LayerMask collidableEntityLayerMask;
    protected static LayerMask playerLayerMask;
    protected static LayerMask collidableLayerMask;


    protected Direction currDir;

    protected virtual void Awake()
    {
        //wallLayerMask = GameHandler.wallLayerMask;
        wallLayerMask = LayerMask.GetMask("Walls");
        collidableEntityLayerMask = LayerMask.GetMask("Collidable Entity");
        playerLayerMask = LayerMask.GetMask("Player");
        collidableLayerMask = LayerMask.GetMask("Walls", "Collidable Entity");
    }


    protected static bool IsOppositeDirection(Direction d1, Direction d2)
    {
        return Mathf.Abs((int)d1 - (int)d2) == 2;
    }

    public static float Manhattan(Vector3 a, Vector3 b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    protected static bool CanMove(Vector3 pos, Direction dir)
    {
        RaycastHit2D rh;
        Vector3 rcDir = Vector3.zero;
        switch (dir)
        {
            case Direction.right:
                rcDir = Vector3.right;
                break;
            case Direction.down:
                rcDir = Vector3.down;
                break;
            case Direction.left:
                rcDir = Vector3.left;
                break;
            case Direction.up:
                rcDir = Vector3.up;
                break;
        }
        rh = Physics2D.Raycast(pos, rcDir, 1, collidableLayerMask);
        return rh.collider == null;
    }

    protected static bool CanMove(Vector3 pos)
    {
        Collider2D rh = Physics2D.OverlapPoint(pos, collidableEntityLayerMask);
        return rh == null;
    }

    protected static bool IsWallAhead(Vector3 pos, Direction dir)
    {
        RaycastHit2D rh;
        Vector3 rcDir = Vector3.zero;
        switch (dir)
        {
            case Direction.right:
                rcDir = Vector3.right;
                break;
            case Direction.down:
                rcDir = Vector3.down;
                break;
            case Direction.left:
                rcDir = Vector3.left;
                break;
            case Direction.up:
                rcDir = Vector3.up;
                break;
        }
        rh = Physics2D.Raycast(pos, rcDir, 1, wallLayerMask);
        return rh.collider != null;
    }

}
