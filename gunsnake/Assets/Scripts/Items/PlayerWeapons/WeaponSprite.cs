using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSprite : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite weapon;
    public Sprite weaponBent;
    public Sprite weaponHead;

    public void SetSprite(bool isBent, bool isHead)
    {
        if (isHead)
        {
            spriteRenderer.sprite = weaponHead;
        }
        else
        {
            if (isBent)
            {
                spriteRenderer.sprite = weaponBent;
            }
            else
            {
                spriteRenderer.sprite = weapon;
            }
        }
    }
}
