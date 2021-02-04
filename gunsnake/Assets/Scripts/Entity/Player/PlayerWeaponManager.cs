using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    public int shotCooldown = 2;
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
            w.WeaponTick(e.tick);

        if (e.tick % 4 == 0)
        {
            ticksTillCooldown--;
            if (ticksTillCooldown <= 0)
            {
                weapons[currMountIndex].Attack(); // test CanAttack() => Attack() maybe
                currMountIndex = (currMountIndex + 1) % weapons.Length;

                ticksTillCooldown = shotCooldown;
            }
        }
    }

    public bool CanFire()
    {
        return !isSprinting;
    }


    public PlayerWeapon SwapWeapon(PlayerWeapon newWeapon, int index)
    {
        PlayerWeapon oldWeapon = weapons[index];
        weapons[index] = newWeapon;
        return oldWeapon;
    }
}
