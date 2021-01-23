using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : Projectile
{
    public Vector3 direction;

    public override void ProjectileTick(int tick)
    {
        transform.position += direction;
    }
}
