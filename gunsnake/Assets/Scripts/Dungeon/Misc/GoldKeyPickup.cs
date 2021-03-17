using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldKeyPickup : MonoBehaviour
{
    public bool isKey;
    public int goldAmount;

    public bool randomizeGold;
    public int minGold;
    public int maxGold;


    private void Awake()
    {
        if (randomizeGold)
            goldAmount = Random.Range(minGold, maxGold);
    }

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
