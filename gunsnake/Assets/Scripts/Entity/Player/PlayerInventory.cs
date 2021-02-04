using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerInventory
{
    public static int gold;
    public static int keys;

    // TODO: make Aritact:Item and ArtifactManager for equiping/dequiping arts
    //public static Artifact[] artifacts;

    public static void AddGold(int amount)
    {
        gold += amount;
    }
    public static void AddKey(int amount)
    {
        keys += amount;
    }



    public static PlayerWeapon SwapWeapon(PlayerWeapon newWeapon, int index)
    {
        return Player.playerWeaponManager.SwapWeapon(newWeapon, index);
    }

    // public static void AddArtifact(Artifact artifact)
}
