using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerInventory
{
    public static int gold;
    public static ArtifactManager _artmanager;
    [SerializeField]
    public static int keys = 1;

    public static PlayerWeapon[] weaponStorage = new PlayerWeapon[2];

    // TODO: make Aritact:Item and ArtifactManager for equiping/dequiping arts
    private static Artifact[] artifacts;

    public static void AddGold(int amount)
    {
        gold += amount;
    }
    public static void AddKey(int amount)
    {
        keys += amount;
    }
    public static bool HasKeys()
    {
        return keys > 0;
    }


    public static PlayerWeapon SetWeapon(PlayerWeapon newWeapon, int index)
    {
        return Player.playerWeaponManager.SetWeapon(newWeapon, index);
    }

    public static void MoveEquippedToStorage(int equipIndex, int storageIndex)
    {
        if (weaponStorage[storageIndex] != null || Player.playerWeaponManager.GetWeapon(equipIndex) == null)
            return;
        weaponStorage[storageIndex] = SetWeapon(null, equipIndex);
    }

    public static void MoveStorageToEquipped(int storageIndex, int equipIndex)
    {
        if (weaponStorage[storageIndex] == null || Player.playerWeaponManager.GetWeapon(equipIndex) != null)
            return;
        weaponStorage[storageIndex] = SetWeapon(weaponStorage[storageIndex], equipIndex);
    }

    public static bool IsStorageFull()
    {
        foreach (PlayerWeapon w in weaponStorage)
            if (w == null)
                return false;
        return true;
    }

    public static void AddArtifact(Artifact artifact)
    {
        switch(artifact.codename)
        {
            case "attack":
                break;
            case "health":
                Player.playerHealth.ChangemaxHealth();
                break;
            case "pierce":
                break;
        }
    }
}
