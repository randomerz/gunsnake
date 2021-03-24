using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WeaponSprite))]
public abstract class PlayerWeapon : MonoBehaviour
{
    [Header("Stats")]
    public int damage;
    public int fireRate;
    private int ticksTillReady;
    private bool isReady;

    [Header("Other")]
    public string attackSound;

    [HideInInspector]
    public PlayerSegmentSprite mount;
    protected WeaponSprite weaponSprite;


    private void Awake()
    {
        weaponSprite = GetComponent<WeaponSprite>();
    }

    // runs once per game tick
    public virtual void WeaponTick(int tick)
    {
        weaponSprite.SetSprite(mount.isBent, mount.isHead);
    }

    public virtual void Attack()
    {
        AudioManager.Play(attackSound);
    }
}
