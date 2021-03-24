using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foblin : Enemy
{
    [Header("Foblin")]
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

            // visuals
            if (animator != null)
                SetAnimatorBool("isIdle", true);

            switch (ticksTillAttack)
            {
                case 2:
                case 1:
                    if (animator != null)
                        SetAnimatorBool("isPrep", true);
                    break;
                default:
                    if (ticksTillAttack <= 0)
                    {
                        ticksTillAttack = attackSpeed;

                        // visuals
                        if (animator != null)
                            SetAnimatorBool("isAttack", true);


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

        // other updates not %4 tick
        animator.UpdatePosition();
    }

    // TODO: fix movement, see card. 
    // May need new to create new method, GetDirectionsToPlayer(shouldDiag)
    private void Move(Vector3 dir)
    {
        animator.SetOrigPos(transform.position);
        if (dir.x > 0)
            animator.SetFacing(false);
        else if (dir.x < 0)
            animator.SetFacing(true);

        if (CanMove(transform.position + dir))
            transform.position += dir;
        else if (CanMove(transform.position + new Vector3(0, 1, 0)))
        {
            Vector3 newdir = new Vector3(0, 1, 0);
            transform.position += newdir;
        }
        else if (CanMove(transform.position + new Vector3(0, -1, 0)))
        {
            Vector3 newdir = new Vector3(0, -1, 0);
            transform.position += newdir;
        }
        else if (CanMove(transform.position + new Vector3(1, 0, 0)))
        {
            Vector3 newdir = new Vector3(1, 0, 0);
            transform.position += newdir;
        }
        else
        {
            Vector3 newdir = new Vector3(-1, 0, 0);
            transform.position += newdir;
        }
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
