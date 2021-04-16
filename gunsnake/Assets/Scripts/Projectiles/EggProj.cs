using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggProj : BasicProjectile
{
    public GameObject eggsplosionPrefab;
    public static int explode = 0;

    new void Awake()
    {
        base.Awake();
    }
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (ignoredColliders.Contains(other))
        {
            return;
        }
        // check if it is not in targets
        if ((targets & (1 << other.gameObject.layer)) == 0)
        {
            return;
        }

        if (other.tag == "Enemy" && !hitEnemyThisTile)
        {
            Enemy e = other.gameObject.GetComponent<Enemy>();
            e.TakeDamage(CalculateDamage(), direction);
            basePierce -= 1;
            if (explode > 0)
            {
                GameObject egg = Instantiate(eggsplosionPrefab, transform.position, Quaternion.identity, transform.parent);
                egg.GetComponent<Eggsplosion>().Init(explode, CalculateDamage());
            }
            if (CalculatePierce() < 0)
                ProjectileManager.RemoveProjectile(gameObject);

            hitEnemyThisTile = true;
        }

        if (other.tag == "Player" && !hitEnemyThisTile)
        {
            PlayerSegmentHealth p = other.gameObject.GetComponent<PlayerSegmentHealth>();
            p.TakeDamage(CalculateDamage());
            basePierce -= 1;
            if (CalculatePierce() < 0)
                ProjectileManager.RemoveProjectile(gameObject);

            hitEnemyThisTile = true;
        }

        if (other.tag == "Wall")
        {
            /*if (explode > 0)
            {
                GameObject egg = Instantiate(eggsplosionPrefab, transform.position, Quaternion.identity, transform.parent);
                egg.GetComponent<Eggsplosion>().Init(explode);
            }*/
            ProjectileManager.RemoveProjectile(gameObject);
        }
    }
}
