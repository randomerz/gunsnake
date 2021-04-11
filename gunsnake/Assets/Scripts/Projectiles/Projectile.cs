using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Projectile : MonoBehaviour
{
    public static int bonusDamage = 0;
    public static int bonusPierce = 0;
    public int baseDamage;
    public int basePierce; // a bullet with 1 pierce goes through 1 enemy

    public LayerMask targets;

    protected List<Collider2D> ignoredColliders = new List<Collider2D>();
    private Collider2D myCollider;

    //private int pierce;
    //private int damage;

    public void Awake()
    {
        //damage = CalculateDamage();
        // pierce = CalculatePierce();
        myCollider = GetComponent<Collider2D>();

        if (myCollider != null)
        {
            foreach (Collider2D c in ignoredColliders)
            {
                Physics2D.IgnoreCollision(myCollider, c, false);
            }
        }
        ignoredColliders.Clear();
    }

    public abstract void ProjectileTick(int tick);
    public abstract void SetValues(Projectile other);

    public void IgnoreCollision(Collider2D other)
    {
        ignoredColliders.Add(other);
        if (myCollider != null)
        {
            Physics2D.IgnoreCollision(myCollider, other);
        }
    }


    protected int CalculateDamage()
    {
        return baseDamage + bonusDamage;
    }

    protected int CalculatePierce()
    {
        return basePierce + bonusPierce;
    }
}
