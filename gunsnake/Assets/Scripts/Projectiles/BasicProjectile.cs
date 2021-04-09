using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : Projectile
{
    public Vector3 direction;
    public int moveRate = 2;

    private bool hitEnemyThisTile = false;

    //  For making gifs
    //private void Awake()
    //{
    //    TimeTickSystem.OnTick_Projectiles += TimeTickSystem_OnTick;
    //}

    //private void TimeTickSystem_OnTick(object sender, TimeTickSystem.OnTickEventArgs e)
    //{
    //    if (e.tick % 4 == 0)
    //        ProjectileTick(e.tick);
    //}

    public override void ProjectileTick(int tick)
    {
        CheckIfEnemyOnSquare();
        if (tick % moveRate == 0)
        {
            hitEnemyThisTile = false;
            transform.position += direction;
        }
        CheckIfEnemyOnSquare();
    }

    public override void SetValues(Projectile other)
    {
        baseDamage = other.baseDamage;
        basePierce = other.basePierce;
        moveRate = ((BasicProjectile)other).moveRate;
    }

    private void CheckIfEnemyOnSquare()
    {
        if (hitEnemyThisTile)
            return;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 0.5f, Entity.fullHeightEntitiesMask);
        foreach (Collider2D col in enemies)
        {
            OnTriggerEnter2D(col);

            if (hitEnemyThisTile)
                break;
        }
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
}
