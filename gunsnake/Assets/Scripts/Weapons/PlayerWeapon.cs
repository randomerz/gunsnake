using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerWeapon : MonoBehaviour
{
    [Header("Stats")]
    public int damage;
    public int fireRate;
    private int ticksTillReady;
    private bool isReady;

    [Header("Other")]
    public PlayerSegmentSprite mount;

    // runs once per game tick
    public abstract void WeaponTick();
    public abstract void Attack();
}
