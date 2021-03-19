using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicShot : PlayerWeapon
{
    public bool doDiagonal;
    public GameObject bulletPrefab;

    public override void WeaponTick(int tick)
    {
        base.WeaponTick(tick);
    }

    public override void Attack()
    {
        GameObject proj1 = ProjectileManager.CreateProjectile(bulletPrefab);
        BasicProjectile bp1 = proj1.GetComponent<BasicProjectile>();

        if (mount.isHead)
        {
            Vector3 dir = -mount.prevSegDir;
            proj1.transform.position = mount.transform.position; // + dir
            proj1.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
            bp1.direction = dir;
        }
        else
        {
            GameObject proj2 = ProjectileManager.CreateProjectile(bulletPrefab);
            BasicProjectile bp2 = proj2.GetComponent<BasicProjectile>();

            Vector3 dir1;
            Vector3 dir2;
            if (mount.isBent && !mount.isTail)
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

            proj1.transform.position = mount.transform.position; // + dir1;
            proj2.transform.position = mount.transform.position; // + dir2;
            proj1.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir1.y, dir1.x) * Mathf.Rad2Deg);
            proj2.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir2.y, dir2.x) * Mathf.Rad2Deg);
            bp1.direction = dir1;
            bp2.direction = dir2;
        }
    }
}
