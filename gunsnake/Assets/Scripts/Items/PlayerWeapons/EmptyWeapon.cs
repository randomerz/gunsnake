using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyWeapon : PlayerWeapon
{
    public override void WeaponTick(int tick)
    {
        base.WeaponTick(tick);
    }

    public override void Attack()
    {
        base.Attack();
    }
}
