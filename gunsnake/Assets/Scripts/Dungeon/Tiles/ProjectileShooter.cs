using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShooter : Tile
{
    public int fireRate; // in ticks
    public int ticksTillShot;

    public Direction direction;
    public GameObject projectile;

    public Transform bulletSpawn;
    public GameObject prepParticles;
    public GameObject attackParticles;
    //private Animator animator;

    private void Awake()
    {
        //animator = GetComponent<Animator>();

        TimeTickSystem.OnTick_Dungeon += TimeTickSystem_OnTick;
    }

    protected virtual void TimeTickSystem_OnTick(object sender, TimeTickSystem.OnTickEventArgs e)
    {
        if (e.tick % 4 == 0)
        {
            if (!isEnabled)
                return;

            ticksTillShot -= 1;

            if (ticksTillShot == 5)
            {
                //animator.SetTrigger("prep");
                Instantiate(prepParticles, bulletSpawn.position, Quaternion.identity, transform);
            }

            if (ticksTillShot <= 0)
            {
                // AudioManager.PlaySound("");
                Instantiate(attackParticles, bulletSpawn.position, Quaternion.identity, transform);

                Shoot();

                ticksTillShot = fireRate;
            }
        }
    }

    protected virtual void Shoot()
    {
        Vector3 dir = DirectionUtil.Convert(direction);

        GameObject go = ProjectileManager.CreateProjectile(projectile);
        BasicProjectile bp = go.GetComponent<BasicProjectile>();
        go.transform.position = transform.position; // + dir
        go.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        bp.direction = dir;
        bp.IgnoreCollision(GetComponent<Collider2D>());
    }
}
