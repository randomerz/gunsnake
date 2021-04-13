using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : Enemy // this should not inherit from Enemy, but some generic hitbox class
{
    [Header("Torch")]
    public float radius;
    public static Fog fogController;

    public bool isLit = false; // :pensive:
    public GameObject smokeParticle;

    protected override void Awake()
    {
        // Special

        health = maxHealth;

        if (isLit)
        {
            Light();
        }
    }

    public override void EnemyTick(int tick)
    {
        // do nothing
    }

    public override void TakeDamage(int damage, Vector3 hitDirection)
    {
        if (!isLit && damage > 0)
        {
            isLit = true;

            //StrobeWhite(1); can't set oldMat

            Light();
        }
    }

    private void Light()
    {
        fogController.AddTorch(transform, radius);

        AudioManager.Play("dungeon_torch_lit");

        animator.animator.SetBool("isLit", true);
        smokeParticle.SetActive(true);

        if (deathParticle != null)
        {
            Debug.Log("making fwoosh");
            double angle = 90;
            Quaternion rot = Quaternion.Euler(0, 0, (float)angle);
            GameObject particle = Instantiate(deathParticle, transform.position, rot, transform.parent);
            if (animator != null)
            {
                ParticleSystem.MainModule settings = particle.GetComponent<ParticleSystem>().main;
                settings.startColor = new ParticleSystem.MinMaxGradient(animator.deathParticlesColor);
            }
        }
    }
}
