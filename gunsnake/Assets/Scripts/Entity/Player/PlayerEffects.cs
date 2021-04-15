using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffects : MonoBehaviour
{
    public GameObject playerExplosion;
    public GameObject playerSmoke;

    private int entranceInvisInd = -1;
    private int exitInvisInd = -1;

    private PlayerSegmentSprite[] segSprites = new PlayerSegmentSprite[Player.body.Length];

    private bool didInit = false;
    
    void Start()
    {
        InitReferences();
    }

    public void InitReferences()
    {
        if (didInit)
            return;

        didInit = true;

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
            {
                entranceInvisInd = -1;
                PlayerMovement.canSpecialMove = true;
            }
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
                PlayerMovement.canSpecialMove = true;
            }
        }
    }

    public void SetPlayerEntering()
    {
        for (int i = 0; i < segSprites.Length; i++)
            segSprites[i].spriteRenderer.enabled = false;
        segSprites[0].spriteRenderer.enabled = true;
        entranceInvisInd = 1;
        PlayerMovement.canSpecialMove = false;
    }

    public void SetPlayerExiting()
    {
        exitInvisInd = 0;
        PlayerMovement.canSpecialMove = false;
    }

    public int GetExitingIndex()
    {
        return exitInvisInd;
    }

    public void StartPlayerDieEffect(float length)
    {
        StartCoroutine(PlayerDieEffect(length));
    }

    private IEnumerator PlayerDieEffect(float length)
    {
        List<GameObject> smoke = new List<GameObject>();
        for (int i = 0; i < segSprites.Length; i++)
        {
            smoke.Add(Instantiate(playerSmoke, segSprites[i].transform.position, Quaternion.identity, transform));

            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(length - (0.5f * (segSprites.Length + 1)));

        for (int i = 0; i < segSprites.Length; i++)
        {
            smoke.Add(Instantiate(playerExplosion, segSprites[i].transform.position, Quaternion.identity, transform));
        }

        yield return new WaitForSeconds(0.5f);

        foreach (GameObject g in smoke)
        {
            Destroy(g);
        }
    }
}
