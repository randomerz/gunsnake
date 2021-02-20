using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootActivatorTile : Tile
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isEnabled && other.tag == "Player")
        {
            if (other.GetComponent<PlayerMovement>() != null) // is head
            {
                Debug.Log("Opening loot...");
                isEnabled = false;
            }
        }
    }
}
