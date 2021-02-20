using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitActivatorTile : Tile
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (other.GetComponent<PlayerMovement>() != null) // is head
            {
                Debug.Log("Finishing level...");
            }
        }
    }
}
