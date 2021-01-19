using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicShot : PlayerWeapon
{
    public GameObject bulletPrefab;

    public override void WeaponTick()
    {

    }

    public override void Attack()
    {
        Debug.Log("bang!");
    }
}
