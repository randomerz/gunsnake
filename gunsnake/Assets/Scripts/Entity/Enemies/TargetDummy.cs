using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDummy : Enemy
{
    public override void EnemyTick(int tick)
    {
        // do nothing
    }

    public override void TakeDamage(int damage)
    {
        health -= damage;
        StrobeWhite(1);

        Debug.Log("Ouch! I just took " + damage + " damage.");
    }
}
