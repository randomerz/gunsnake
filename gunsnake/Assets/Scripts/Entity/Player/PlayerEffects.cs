using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffects : MonoBehaviour
{
    private int entranceInvisInd = -1;
    private int exitInvisInd = -1;

    private PlayerSegmentSprite[] segSprites = new PlayerSegmentSprite[Player.body.Length];


    void Start()
    {
        for (int i = 0; i < Player.body.Length; i++)
        {
            segSprites[i] = Player.body[i].GetComponent<PlayerSegmentSprite>();
        }
    }

    public void UpdateMovementEffects()
    {
        if (entranceInvisInd != -1)
        {
            segSprites[entranceInvisInd].spriteRenderer.enabled = true;
            entranceInvisInd++;
            if (entranceInvisInd >= segSprites.Length)
                entranceInvisInd = -1;
        }
        if (exitInvisInd != -1)
        {
            segSprites[exitInvisInd].spriteRenderer.enabled = false;
            exitInvisInd++;
            if (exitInvisInd >= segSprites.Length)
            {
                for (int i = 0; i < segSprites.Length; i++)
                    segSprites[i].spriteRenderer.enabled = true;
                exitInvisInd = -1;
            }
        }
    }

    public void SetPlayerEntering()
    {
        for (int i = 0; i < segSprites.Length; i++)
            segSprites[i].spriteRenderer.enabled = false;
        segSprites[0].spriteRenderer.enabled = true;
        entranceInvisInd = 1;
    }

    public void SetPlayerExiting()
    {
        exitInvisInd = 0;
    }

    public int GetExitingIndex()
    {
        return exitInvisInd;
    }
}
