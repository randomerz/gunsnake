using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public static LayerMask wallLayerMask;
    public static LayerMask halfHeightEntitiesMask;
    public static LayerMask fullHeightEntitiesMask;
    public static LayerMask playerLayerMask;
    public static LayerMask highCollidableMask;
    public static LayerMask fullCollidableMask;


    protected Direction currDir;

    protected virtual void Awake()
    {
        //wallLayerMask = GameHandler.wallLayerMask;
        wallLayerMask = LayerMask.GetMask("Walls");
        halfHeightEntitiesMask = LayerMask.GetMask("Half-Height Entity");
        fullHeightEntitiesMask = LayerMask.GetMask("Full-Height Entity");
        playerLayerMask = LayerMask.GetMask("Player");
        highCollidableMask = LayerMask.GetMask("Walls", "Full-Height Entity");
        fullCollidableMask = LayerMask.GetMask("Walls", "Half-Height Entity", "Full-Height Entity");
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
        rh = Physics2D.Raycast(pos, rcDir, 1, fullCollidableMask);
        return rh.collider == null;
    }

    protected static bool CanMove(Vector3 pos)
    {
        Collider2D rh = Physics2D.OverlapPoint(pos, fullHeightEntitiesMask);
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
