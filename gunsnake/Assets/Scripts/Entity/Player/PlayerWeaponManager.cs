using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    public int shotCooldown = 1;
    private int ticksTillCooldown = 0;

    [SerializeField]
    private PlayerWeapon[] weapons = new PlayerWeapon[Player.body.Length];
    private int currMountIndex;

    // can fire
    public bool isSprinting;

    void Start()
    {
        TimeTickSystem.OnTick_PlayerWeapons += TimeTickSystem_OnTick;
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].mount = Player.body[i].GetComponent<PlayerSegmentSprite>();
        }

        ticksTillCooldown = shotCooldown;
    }

    private void TimeTickSystem_OnTick(object sender, TimeTickSystem.OnTickEventArgs e)
    {
        if (!CanFire())
            return;

        foreach (PlayerWeapon w in weapons)
            if (w != null)
                w.WeaponTick(e.tick);

        if (e.tick % 4 == 0)
        {
            ticksTillCooldown--;
            if (ticksTillCooldown <= 0)
            {
                if (weapons[currMountIndex] != null)
                    weapons[currMountIndex].Attack(); // test CanAttack() => Attack() maybe
                if (weapons[(currMountIndex + 2) % 4] != null)
                    weapons[(currMountIndex + 2) % 4].Attack(); // test CanAttack() => Attack() maybe
                currMountIndex = (currMountIndex + 1) % weapons.Length;

                ticksTillCooldown = shotCooldown;
            }
        }
    }

    public bool CanFire()
    {
        return !isSprinting;
    }


    public PlayerWeapon SetWeapon(PlayerWeapon newWeapon, int index)
    {
        PlayerWeapon oldWeapon = weapons[index];
        weapons[index] = newWeapon;
        return oldWeapon;
    }

    public PlayerWeapon GetWeapon(int index)
    {
        return weapons[index];
    }
}
