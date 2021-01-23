using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscoDamageTile : Tile
{
    public Sprite redTile;
    public Sprite greenTile;
    private Sprite originalTile;

    public Collider2D collider2d;

    private int t = 0;
    private bool shouldDamage = false;
    private bool didDamage = false;

    void Start()
    {
        originalTile = spriteRenderer.sprite;
        TimeTickSystem.OnTick_Dungeon += TimeTickSystem_OnTick;
    }

    private void TimeTickSystem_OnTick(object sender, TimeTickSystem.OnTickEventArgs e)
    {
        switch (t)
        {
            case 32:
            case 40:
            case 48:
                spriteRenderer.sprite = shouldDamage ? redTile : greenTile;
                break;
            case 36:
            case 44:
            case 52:
                spriteRenderer.sprite = originalTile;
                break;
            case 56:
                spriteRenderer.sprite = shouldDamage ? redTile : greenTile;
                collider2d.enabled = shouldDamage;
                break;
            case 63:
                collider2d.enabled = false;
                break;
            case 64:
                spriteRenderer.sprite = originalTile;
                t = 0;
                shouldDamage = Random.Range(0, 2) == 0;
                didDamage = false;
                break;
        }

        t++;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && !didDamage)
        {
            didDamage = true;
            other.GetComponent<PlayerSegmentHealth>().TakeDamage();
        }
    }
}
