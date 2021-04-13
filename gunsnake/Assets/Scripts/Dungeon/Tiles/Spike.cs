using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Spike : Tile
{
    public bool breakAfterUse;

    public bool isOnTimer;
    public int numTicksOn;
    public int numTicksOff;

    public int ticksTillChange;
    public bool isActive;

    private Collider2D spikeCollider;
    private Animator animator;

    private void Awake()
    {
        spikeCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        SetActive(isActive);

        if (isOnTimer)
        {
            TimeTickSystem.OnTick_Dungeon += TimeTickSystem_OnTick;
        }
    }

    private void TimeTickSystem_OnTick(object sender, TimeTickSystem.OnTickEventArgs e)
    {
        if (e.tick % 4 == 0)
        {
            ticksTillChange -= 1;

            if (ticksTillChange == 4 && !isActive)
            {
                animator.SetTrigger("prep");
            }

            if (ticksTillChange <= 0)
            {
                ToggleActive();

                ticksTillChange = isActive ? numTicksOn : numTicksOff;
            }
        }
    }

    public void ToggleActive()
    {
        SetActive(!isActive);
    }

    private void SetActive(bool value)
    {
        isActive = value;
        spikeCollider.enabled = value;

        animator.SetBool("isActive", value);

        if (isActive)
        {
            AudioManager.Play("dungeon_spike_raise");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerSegmentHealth>().TakeDamage(1);

            if (breakAfterUse)
            {
                SetActive(false);

                if (isOnTimer)
                    TimeTickSystem.OnTick_Dungeon -= TimeTickSystem_OnTick;
                // broken
            }
        }
    }
}
