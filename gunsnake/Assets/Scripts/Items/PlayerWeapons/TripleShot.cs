using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleShot : PlayerWeapon
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
        GameObject proj2 = ProjectileManager.CreateProjectile(bulletPrefab);
        GameObject proj3 = ProjectileManager.CreateProjectile(bulletPrefab);
        BasicProjectile bp1 = proj1.GetComponent<BasicProjectile>();
        BasicProjectile bp2 = proj2.GetComponent<BasicProjectile>();
        BasicProjectile bp3 = proj3.GetComponent<BasicProjectile>();

        if (mount.isHead)
        {
            Vector3 dir = -mount.prevSegDir;
            proj1.transform.position = mount.transform.position; // + dir
            proj2.transform.position = mount.transform.position + Vector3.Cross(Vector3.forward, dir);
            proj3.transform.position = mount.transform.position + Vector3.Cross(Vector3.back, dir); // + dir
            proj1.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
            proj2.transform.rotation = proj1.transform.rotation;
            proj3.transform.rotation = proj1.transform.rotation;
            bp1.direction = dir;
            bp2.direction = dir;
            bp3.direction = dir;
        }
        else
        {
            GameObject proj4 = ProjectileManager.CreateProjectile(bulletPrefab);
            GameObject proj5 = ProjectileManager.CreateProjectile(bulletPrefab);
            GameObject proj6 = ProjectileManager.CreateProjectile(bulletPrefab);
            BasicProjectile bp4 = proj4.GetComponent<BasicProjectile>();
            BasicProjectile bp5 = proj5.GetComponent<BasicProjectile>();
            BasicProjectile bp6 = proj6.GetComponent<BasicProjectile>();

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

            proj1.transform.position = mount.transform.position; // + dir1
            proj2.transform.position = mount.transform.position + Vector3.Cross(Vector3.forward, dir1);
            proj3.transform.position = mount.transform.position + Vector3.Cross(Vector3.back, dir1);
            proj1.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir1.y, dir1.x) * Mathf.Rad2Deg);
            proj2.transform.rotation = proj1.transform.rotation;
            proj3.transform.rotation = proj1.transform.rotation;
            bp1.direction = dir1;
            bp2.direction = dir1;
            bp3.direction = dir1;

            proj4.transform.position = mount.transform.position; // + dir2
            proj5.transform.position = mount.transform.position + Vector3.Cross(Vector3.forward, dir2);
            proj6.transform.position = mount.transform.position + Vector3.Cross(Vector3.back, dir2);
            proj4.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir2.y, dir2.x) * Mathf.Rad2Deg);
            proj5.transform.rotation = proj4.transform.rotation;
            proj6.transform.rotation = proj4.transform.rotation;
            bp4.direction = dir2;
            bp5.direction = dir2;
            bp6.direction = dir2;
        }
    }
}
