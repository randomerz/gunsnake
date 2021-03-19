using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    public int baseDamage;
    public int basePierce; // a bullet with 1 pierce goes through 1 enemy
    private int pierce;

    public void Awake()
    {
        pierce = basePierce;
    }

    public abstract void ProjectileTick(int tick);
    public abstract void SetValues(Projectile other);

    protected int CalculateDamage()
    {
        return baseDamage;
    }

    protected int CalculatePierce()
    {
        return basePierce;
    }
}
