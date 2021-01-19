using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSegmentSprite : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public bool isBent;
    public Vector3 nextSegDir;
    public Vector3 prevSegDir; 

    private float rotationAmount;

    public void SetSprite(Sprite sprite, bool bent, float rotAmount, Vector3 next, Vector3 prev)
    {
        spriteRenderer.sprite = sprite;
        isBent = bent;
        rotationAmount = rotAmount;
        nextSegDir = next;
        prevSegDir = prev;
        spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, rotAmount);
    }

    public void SetSprite(PlayerSegmentSprite newSeg)
    {
        SetSprite(newSeg.spriteRenderer.sprite, newSeg.isBent, newSeg.rotationAmount, newSeg.nextSegDir, newSeg.prevSegDir);
    }
}
