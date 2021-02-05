using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    protected static LayerMask wallLayerMask;
    protected static LayerMask collidableEntityLayerMask;
    protected static LayerMask playerLayerMask;
    protected static LayerMask collidableLayerMask;

    public enum Directions
    {
        right,
        down,
        left,
        up,
    }

    protected Directions currDir;

    protected virtual void Awake()
    {
        //wallLayerMask = GameHandler.wallLayerMask;
        wallLayerMask = LayerMask.GetMask("Walls");
        collidableEntityLayerMask = LayerMask.GetMask("Collidable Entity");
        playerLayerMask = LayerMask.GetMask("Player");
        collidableLayerMask = LayerMask.GetMask("Walls", "Collidable Entity");
    }


    protected static bool IsOppositeDirection(Directions d1, Directions d2)
    {
        return Mathf.Abs((int)d1 - (int)d2) == 2;
    }

    public static float Manhattan(Vector3 a, Vector3 b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    protected static bool CanMove(Vector3 pos, Directions dir)
    {
        RaycastHit2D rh;
        Vector3 rcDir = Vector3.zero;
        switch (dir)
        {
            case Directions.right:
                rcDir = Vector3.right;
                break;
            case Directions.down:
                rcDir = Vector3.down;
                break;
            case Directions.left:
                rcDir = Vector3.left;
                break;
            case Directions.up:
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

    protected static bool IsWallAhead(Vector3 pos, Directions dir)
    {
        RaycastHit2D rh;
        Vector3 rcDir = Vector3.zero;
        switch (dir)
        {
            case Directions.right:
                rcDir = Vector3.right;
                break;
            case Directions.down:
                rcDir = Vector3.down;
                break;
            case Directions.left:
                rcDir = Vector3.left;
                break;
            case Directions.up:
                rcDir = Vector3.up;
                break;
        }
        rh = Physics2D.Raycast(pos, rcDir, 1, wallLayerMask);
        return rh.collider != null;
    }

}
