using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    public static int bonusDamage = 0;
    public static int bonusPierce = 0;
    public int baseDamage;
    public int basePierce; // a bullet with 1 pierce goes through 1 enemy

    //private int pierce;
    //private int damage;

    public void Awake()
    {
        //damage = CalculateDamage();
       // pierce = CalculatePierce();
    }

    public abstract void ProjectileTick(int tick);
    public abstract void SetValues(Projectile other);

    protected int CalculateDamage()
    {
        return baseDamage + bonusDamage;
    }

    protected int CalculatePierce()
    {
        return basePierce + bonusPierce;
    }
}
