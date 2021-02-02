using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : Projectile
{
    public Vector3 direction;

    public override void ProjectileTick(int tick)
    {
        if (tick % 2 == 0)
            transform.position += direction;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            other.gameObject.GetComponent<Enemy>().TakeDamage(damage);
            ProjectileManager.RemoveProjectile(gameObject);
        }
        if (other.tag == "Wall")
        {
            ProjectileManager.RemoveProjectile(gameObject);
        }
    }
}
