using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeaProj : BasicProjectile
{
    public GameObject thisPrefab;
    public static int split = 0;
    private int toSplit;
    // Start is called before the first frame update

    // Update is called once per frame
    new void Awake()
    {
        SetPierce(split);
        base.Awake();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (ignoredColliders.Contains(other))
        {
            return;
        }

        if (other.tag == "Enemy" && !hitEnemyThisTile)
        {
            Enemy e = other.gameObject.GetComponent<Enemy>();
            e.TakeDamage(CalculateDamage(), direction);
            if (toSplit > 0)
            {
                GameObject pea1 = ProjectileManager.CreateProjectile(thisPrefab);
                GameObject pea2 = ProjectileManager.CreateProjectile(thisPrefab);
                pea1.transform.Rotate(0f, 0f, -90f);
                pea2.transform.Rotate(0f, 0f, 90f);
                toSplit--;
                pea1.GetComponent<PeaProj>().SetPierce(toSplit);
                pea2.GetComponent<PeaProj>().SetPierce(toSplit);
            }
            basePierce -= 1;
            if (CalculatePierce() < 0)
                ProjectileManager.RemoveProjectile(gameObject);

            hitEnemyThisTile = true;
        }
        if (other.tag == "Wall")
        {
            ProjectileManager.RemoveProjectile(gameObject);
        }
    }
    void SetPierce(int p)
    {
        toSplit = p;
    }
}
