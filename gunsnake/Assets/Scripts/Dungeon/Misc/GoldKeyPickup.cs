using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldKeyPickup : MonoBehaviour
{
    public bool isKey;
    public int goldAmount;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (isKey)
                PlayerInventory.AddKey(1);
            else
                PlayerInventory.AddGold(goldAmount);
            
            Destroy(gameObject);
        }
    }
}
