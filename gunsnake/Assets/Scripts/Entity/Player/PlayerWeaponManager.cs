using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    public int shotCooldown = 1;
    private int ticksTillCooldown = 0;

    [SerializeField]
    private Item[] defaultItems = new Item[Player.body.Length];
    private static Item[] weaponItems = new Item[Player.body.Length];
    private static PlayerWeapon[] weapons = new PlayerWeapon[Player.body.Length];
    private static GameObject[] weaponObjs = new GameObject[Player.body.Length];
    private static int currMountIndex;

    // can fire
    public bool isSprinting;

    private bool didInit;

    private void Awake()
    {
        TimeTickSystem.OnTick_PlayerWeapons += TimeTickSystem_OnTick;
    }

    void Start()
    {
        if (!didInit)
            InitializeWeapons();
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


    public Item SetWeapon(Item newWeaponItem, int index)
    {
        Item oldWeaponItem = weaponItems[index];
        weaponItems[index] = newWeaponItem;
        Destroy(weaponObjs[index]);

        if (weaponItems[index] != null)
        {
            GameObject weaponObj = Instantiate(weaponItems[index].prefab, Player.sprites[index].transform);
            weapons[index] = weaponObj.GetComponent<PlayerWeapon>();
            weapons[index].mount = Player.body[index].GetComponent<PlayerSegmentSprite>();
            weaponObjs[index] = weaponObj;
        }

        return oldWeaponItem;
    }

    public Item GetWeapon(int index)
    {
        return weaponItems[index];
    }

    public void InitializeWeapons()
    {
        didInit = true;

        for (int i = 0; i < weapons.Length; i++)
        {
            if (weaponItems[i] != null)
            {
                GameObject weaponObj = Instantiate(weaponItems[i].prefab, Player.sprites[i].transform);
                weapons[i] = weaponObj.GetComponent<PlayerWeapon>();
                weapons[i].mount = Player.body[i].GetComponent<PlayerSegmentSprite>();
                weaponObjs[i] = weaponObj;
            }
        }

        ticksTillCooldown = shotCooldown;
    }

    public void ResetWeaponsToDefault()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            SetWeapon(null, i);
        }
        defaultItems.CopyTo(weaponItems, 0);
        InitializeWeapons();
    }
}
