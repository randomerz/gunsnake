using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Enemy
{
    [Tooltip("1 = 4 game ticks")]
    public int attackSpeed;
    private int ticksTillAttack;

    protected override void Awake()
    {
        base.Awake();
        if (randomizeStartingVars)
            ticksTillAttack = Random.Range(1, attackSpeed);
        else
            ticksTillAttack = attackSpeed;
    }

    public override void EnemyTick(int tick)
    {
        if (tick % 4 == 0)
        {
            ticksTillAttack -= 1;

            switch (ticksTillAttack)
            {
                case 2:
                case 1:
                    spriteRenderer.color = Color.red;
                    break;
                default:
                    if (ticksTillAttack <= 0)
                    {
                        spriteRenderer.color = Color.white;
                        ticksTillAttack = attackSpeed;

                        // attack if within range
                        GameObject closestSeg = GetClosestPlayerSegment();
                        if ((closestSeg.transform.position - transform.position).magnitude <= 1)
                        {
                            Attack(closestSeg);
                        }
                        // else move closer
                        else
                        {
                            Move(GetDirectionToPlayer(false));
                        }
                    }
                    break;
            }
        }
    }

    // TODO: fix movement, see card. 
    // May need new to create new method, GetDirectionsToPlayer(shouldDiag)
    private void Move(Vector3 dir)
    {
        if (CanMove(transform.position + dir))
            transform.position += dir;
    }

    private void Attack(GameObject seg)
    {
        PlayerSegmentHealth h = seg.GetComponent<PlayerSegmentHealth>();
        if (h != null)
        {
            h.TakeDamage(damage);
        }
    }
}
