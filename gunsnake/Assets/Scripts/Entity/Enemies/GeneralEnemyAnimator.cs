using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralEnemyAnimator : MonoBehaviour
{
    public Color deathParticlesColor;
    public Animator animator;
    public GameObject spriteObj;

    //private SpriteRenderer spriteRenderer;
    private Vector3 orig;
    private int positionAnimationIndex = 0;

    public int positionIndexMax = 3; // temp, either set to 3 or 4

    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetFacing(bool isFacingLeft)
    {
        spriteObj.GetComponent<SpriteRenderer>().flipX = isFacingLeft;
    }

    public void SetOrigPos(Vector3 orig)
    {
        this.orig = orig;
        positionAnimationIndex = positionIndexMax;
    }

    // each game frame
    public void UpdatePosition()
    {
        positionAnimationIndex -= 1;
        switch (positionAnimationIndex)
        {
            case 3:
                spriteObj.transform.position = 0.5f * orig + 0.5f * transform.position;
                spriteObj.transform.position += 0.1875f * Vector3.up;
                break;
            case 2:
                spriteObj.transform.position = 0.25f * orig + 0.75f * transform.position;
                spriteObj.transform.position += 0.125f * Vector3.up;
                break;
            case 1:
                spriteObj.transform.position = 0.125f * orig + 0.875f * transform.position;
                spriteObj.transform.position += 0.0625f * Vector3.up;
                break;
            case 0:
            default:
                spriteObj.transform.position = transform.position;
                break;
        }
    }
}
