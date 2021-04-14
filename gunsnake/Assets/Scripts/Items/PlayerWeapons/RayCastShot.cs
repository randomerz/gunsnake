using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastShot : PlayerWeapon
{
    public bool doDiagonal;
    public GameObject bulletPrefab;

    public override void WeaponTick(int tick)
    {
        base.WeaponTick(tick);
    }

    public override void Attack()
    {
        base.Attack();

        GameObject proj1 = ProjectileManager.CreateProjectile(bulletPrefab);
        RayCastProj rc1 = proj1.GetComponent<RayCastProj>();

        if (mount.isHead)
        {
            Vector3 dir = -mount.prevSegDir;
            proj1.transform.position = mount.transform.position + (0.5f * dir);
            rc1.startPos = proj1.transform.position;
            rc1.direction = dir;
            rc1.Cast();
        }
        else
        {
            GameObject proj2 = ProjectileManager.CreateProjectile(bulletPrefab);
            RayCastProj rc2 = proj2.GetComponent<RayCastProj>();

            Vector3 dir1;
            Vector3 dir2;
            if (mount.isBent)
            {
                if (doDiagonal)
                {
                    dir1 = mount.nextSegDir + mount.prevSegDir;
                    dir2 = -dir1;
                }
                else
                {
                    dir1 = -mount.nextSegDir;
                    dir2 = -mount.prevSegDir;
                }
            }
            else
            {
                dir1 = Vector3.Cross(Vector3.forward, mount.nextSegDir);
                dir2 = Vector3.Cross(Vector3.back, mount.nextSegDir);
            }

            proj1.transform.position = mount.transform.position + (0.5f * dir1);
            proj2.transform.position = mount.transform.position + (0.5f * dir2);
            rc1.startPos = proj1.transform.position;
            rc2.startPos = proj2.transform.position;
            rc1.direction = dir1;
            rc2.direction = dir2;

            rc1.Cast();
            rc2.Cast();
        }
    }
}
