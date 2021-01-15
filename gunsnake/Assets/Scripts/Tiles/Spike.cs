using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : Tile
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerBodySegment>().TakeDamage();
        }
    }
}
