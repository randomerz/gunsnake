using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField]
    private LayerMask wallLayerMask;

    public enum directions
    {
        right,
        down,
        left,
        up,
    }

    protected directions currDir;

    protected virtual void Awake()
    {
        //wallLayerMask = GameHandler.wallLayerMask;
        wallLayerMask = LayerMask.GetMask("Walls");
    }


    protected bool IsOppositeDirection(directions d1, directions d2)
    {
        return Mathf.Abs((int)d1 - (int)d2) == 2;
    }

    protected bool IsWallAhead(Vector3 pos, directions dir)
    {
        RaycastHit2D rh;
        Vector3 rcDir = Vector3.zero;
        switch (dir)
        {
            case directions.right:
                rcDir = Vector3.right;
                break;
            case directions.down:
                rcDir = Vector3.down;
                break;
            case directions.left:
                rcDir = Vector3.left;
                break;
            case directions.up:
                rcDir = Vector3.up;
                break;
        }
        rh = Physics2D.Raycast(pos, rcDir, 1, wallLayerMask);
        return rh.collider != null;
    }
}
