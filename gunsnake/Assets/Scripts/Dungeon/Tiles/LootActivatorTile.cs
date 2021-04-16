using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootActivatorTile : Tile
{
    public Sprite openChestSprite;
    public bool isArtifact;

    private Collider2D myCollider;

    private void Awake()
    {
        myCollider = GetComponent<Collider2D>();
        myCollider.enabled = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;

        StartCoroutine(TrySpawning());
        //Instantiate(preSpawnParticle, transform.position, Quaternion.identity, transform);

    }

    private IEnumerator TrySpawning()
    {
        while (!CanSpawn(transform.position))
        {
            yield return new WaitForSeconds(0.25f);
        }

        myCollider.enabled = true;
        spriteRenderer.enabled = true;
    }

    private bool CanSpawn(Vector2 pos)
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(pos);
        foreach (Collider2D hit in hits)
        {
            if (hit && hit != myCollider && hit.tag != "Room")
            {
                return false;
            }
        }
        return true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isEnabled && other.tag == "Player")
        {
            if (other.GetComponent<PlayerMovement>() != null) // is head
            {
                OpenLoot();
            }
        }
    }

    public void OpenLoot()
    {
        spriteRenderer.sprite = openChestSprite;

        UIManager.OpenLoot(isArtifact);
    }
}
