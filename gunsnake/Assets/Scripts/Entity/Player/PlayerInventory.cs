using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerInventory
{
    public static int gold;
    public static int keys;

    public static Item[] weaponStorage = new Item[2];

    private static int goldBonus = 0;

    public static void ResetValues()
    {
        gold = 0;
        keys = 0;
        weaponStorage = new Item[2];
    }

    public static void AddGold(int amount)
    {
        gold += (int)(amount * (1 + (0.2*goldBonus)));

        Player.AddScore(amount);
    }
    public static void AddKey(int amount)
    {
        keys += amount;
    }
    public static bool HasKeys()
    {
        return keys > 0;
    }

    public static int GetGold()
    {
        return gold;
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

    public static void AddArtifact(Item art)
    {
        ArtifactManager._instance.AddArtifact(art);
        switch (art.name)
        {
            case "Protein Shake":
                Projectile.bonusDamage++;
                break;
            case "Hearty Apple":
                Player.playerHealth.ChangeMaxHealth();
                break;
            case "Piercing Grapes":
                Projectile.bonusPierce++;
                break;
            case "Ghost Pepper":
                Player.playerHealth.UpdateDodge(art.count);
                break;
            case "Gold Berry":
                goldBonus++;
                break;
            case "Thirsty Succulent":
                Player.playerHealth.Lifesteal(false);
                break;
            case "Scrambled Eggs":
                break;
            case "Canned Peas":
                PeaProj.split++;
                break;
        }
    }

    public static Item[] GetArtifactList()
    {
        return ArtifactManager._instance.GetArtifacts();
    }
}
