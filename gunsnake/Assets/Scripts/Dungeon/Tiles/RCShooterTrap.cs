using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RCShooterTrap : ProjectileShooter
{
    public GameObject prefireLaser;

    protected override void TimeTickSystem_OnTick(object sender, TimeTickSystem.OnTickEventArgs e)
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

            if (ticksTillShot == 4)
            {
                Prefire();
            }

            if (ticksTillShot <= 0)
            {
                Instantiate(attackParticles, bulletSpawn.position, Quaternion.identity, transform);

                AudioManager.Play("dungeon_turret_laser");

                Shoot();

                ticksTillShot = fireRate;
            }
        }
    }

    protected override void Shoot()
    {
        Vector3 dir = DirectionUtil.Convert(direction);

        GameObject go = ProjectileManager.CreateProjectile(projectile);
        RayCastProj rc = go.GetComponent<RayCastProj>();
        go.transform.position = bulletSpawn.position; // + dir
        rc.startPos = bulletSpawn.position;
        rc.direction = dir;
        rc.Cast();
    }

    private void Prefire()
    {
        Vector3 dir = DirectionUtil.Convert(direction);

        GameObject go = ProjectileManager.CreateProjectile(prefireLaser);
        RayCastProj rc = go.GetComponent<RayCastProj>();
        go.transform.position = bulletSpawn.position; // + dir
        rc.startPos = bulletSpawn.position;
        rc.direction = dir;
        rc.IgnoreCollision(GetComponent<Collider2D>());
        rc.Cast();
    }
}
