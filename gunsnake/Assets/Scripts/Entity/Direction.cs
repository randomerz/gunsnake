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

    public static Direction Convert(Vector3Int dir)
    {
        if (dir.magnitude > 1)
        {
            Debug.LogError("DirectionUtil.Convert() shouldn't be used with Vector's who's magnitude is greater than 1!");
        }

        if (dir.x > 0)
            return Direction.right;
        if (dir.y > 0)
            return Direction.up;
        if (dir.x < 0)
            return Direction.left;
        if (dir.y < 0)
            return Direction.down;

        return 0;
    }

    public static Direction Convert(Vector3 dir)
    {
        if (dir.magnitude > 1)
        {
            Debug.LogError("DirectionUtil.Convert() shouldn't be used with Vector's who's magnitude is greater than 1!");
        }

        if (dir.x > 0)
            return Direction.right;
        if (dir.y > 0)
            return Direction.up;
        if (dir.x < 0)
            return Direction.left;
        if (dir.y < 0)
            return Direction.down;

        return 0;
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
