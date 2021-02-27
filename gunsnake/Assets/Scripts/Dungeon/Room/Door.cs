using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isClosed;
    public bool isLocked;
    public bool isVertical;
    public bool isOptional;
    public bool isWall;


    [Header("References")]
    public SpriteRenderer spriteRenderer;
    public Collider2D horsBox;
    public Collider2D vertBox;

    // jank
    public Sprite[] doorSprites;
    private static Sprite spriteOpenedHors;
    private static Sprite spriteClosedHors;
    private static Sprite spriteOpenedVert;
    private static Sprite spriteClosedVert;
    private static Sprite spriteLockedOpenedHors;
    private static Sprite spriteLockedClosedHors;
    private static Sprite spriteLockedOpenedVert;
    private static Sprite spriteLockedClosedVert;
    private static bool didInit = false;

    void Awake()
    {
        Init();

        UpdateSpriteBoxes();
    }
    
    private void Init()
    {
        if (!didInit)
        {
            didInit = true;

            spriteOpenedHors = doorSprites[0];
            spriteClosedHors = doorSprites[1];
            spriteOpenedVert = doorSprites[2];
            spriteClosedVert = doorSprites[3];
            spriteLockedOpenedHors = doorSprites[4];
            spriteLockedClosedHors = doorSprites[5];
            spriteLockedOpenedVert = doorSprites[6];
            spriteLockedClosedVert = doorSprites[7];
        }
    }

    private float timer;
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > 1)
        {
            timer = 0;
            UpdateSpriteBoxes();
        }
    }

    public void SetIsClosed(bool shouldClose)
    {
        isClosed = shouldClose;
        UpdateSpriteBoxes();
    }

    public void UnlockDoor()
    {
        SetIsClosed(false);
    }

    public void SetIsWall(bool wall)
    {
        isWall = wall;
        Init();
        UpdateSpriteBoxes();
    }

    private void UpdateSpriteBoxes()
    {
        spriteRenderer.enabled = !isWall;
        if (isVertical)
        {
            horsBox.enabled = false;
            if (isClosed)
            {
                vertBox.enabled = true;
                if (isLocked)
                {
                    spriteRenderer.sprite = spriteLockedClosedVert;
                }
                else
                {
                    spriteRenderer.sprite = spriteClosedVert;
                }
            }
            else
            {
                vertBox.enabled = false;
                if (isLocked)
                {
                    spriteRenderer.sprite = spriteLockedOpenedVert;
                }
                else
                {
                    spriteRenderer.sprite = spriteOpenedVert;
                }
            }
        }

        else
        {
            vertBox.enabled = false;
            if (isClosed)
            {
                horsBox.enabled = true;
                if (isLocked)
                {
                    spriteRenderer.sprite = spriteLockedClosedHors;
                }
                else
                {
                    spriteRenderer.sprite = spriteClosedHors;
                }
            }
            else
            {
                horsBox.enabled = false;
                if (isLocked)
                {
                    spriteRenderer.sprite = spriteLockedOpenedHors;
                }
                else
                {
                    spriteRenderer.sprite = spriteOpenedHors;
                }
            }
        }
    }
}
