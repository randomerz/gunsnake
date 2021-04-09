using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : Projectile
{
    public Vector3 direction;


    private bool hitPlayerThisTile = false;

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
        CheckIfPlayerOnSquare();
        if (tick % 2 == 0)
        {
            hitPlayerThisTile = false;
            transform.position += direction;
        }
        CheckIfPlayerOnSquare();
    }

    public override void SetValues(Projectile other)
    {
        baseDamage = other.baseDamage;
        basePierce = other.basePierce;
    }

    private void CheckIfPlayerOnSquare()
    {
        if (hitPlayerThisTile)
            return;

        Collider2D[] walls = Physics2D.OverlapCircleAll(transform.position, 0.5f, Entity.fullCollidableMask);
        foreach (Collider2D col in walls)
        {
            OnTriggerEnter2D(col);

            if (hitPlayerThisTile)
                break;
        }
        Collider2D[] player = Physics2D.OverlapCircleAll(transform.position, 0.5f, Entity.playerLayerMask);
        foreach (Collider2D col in player)
        {
            OnTriggerEnter2D(col);

            if (hitPlayerThisTile)
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerSegmentHealth>().TakeDamage(CalculateDamage());
            hitPlayerThisTile = true;
        }
        if (other.tag == "Wall")
        {
            ProjectileManager.RemoveProjectile(gameObject);
        }
    }
}
