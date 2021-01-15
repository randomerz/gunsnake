using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodySegment : MonoBehaviour
{
    public PlayerHealth playerHealth;

    void Start()
    {
        
    }
    
    void Update()
    {
        
    }
    
    public void TakeDamage()
    {
        TakeDamage(1);
    }

    public void TakeDamage(int amount)
    {
        playerHealth.TakeDamage(amount);
    }
}
