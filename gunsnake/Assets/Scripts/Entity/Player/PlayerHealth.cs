using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    private int health;
    private int maxHealth = 3;

    public bool canTakeDamage;

    public TextMeshProUGUI healthText;

    void Start()
    {
        health = maxHealth;
    }


    private void UpdateHUD()
    {
        healthText.text = health.ToString();
    }

    public void TakeDamage()
    {
        TakeDamage(1);
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        UpdateHUD();
        CameraShake.Shake(0.25f, 0.25f);
    }
}
