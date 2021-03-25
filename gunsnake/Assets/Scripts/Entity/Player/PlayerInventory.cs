using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerInventory
{
    public static int gold;
    public static int keys;

    public static Item[] weaponStorage = new Item[2];

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


    public static Item GetWeapon(int index)
    {
        return Player.playerWeaponManager.GetWeapon(index);
    }

    public static Item SetWeapon(Item newWeaponItem, int index)
    {
        return Player.playerWeaponManager.SetWeapon(newWeaponItem, index);
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
        foreach (Item w in weaponStorage)
            if (w == null)
                return false;
        return true;
    }

    public static void AddWeapon(Item weapon)
    {
        if (IsStorageFull())
        {
            Debug.LogWarning("Tried adding weapon to Player's weapon storage, but is full.");
            return;
        }

        for (int i = 0; i < weaponStorage.Length; i++)
        {
            if (weaponStorage[i] == null)
            {
                weaponStorage[i] = weapon;
                return;
            }
        }
    }

    public static void AddArtifact(Artifact artifact)
    {
        switch(artifact.codename)
        {
            case "attack":
                Projectile.bonusDamage++;
                break;
            case "health":
                Player.playerHealth.ChangeMaxHealth();
                break;
            case "pierce":
                Projectile.bonusPierce++;
                break;
        }
        ArtifactManager._instance.AddArtifact(artifact);
    }
}
