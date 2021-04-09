﻿using System.Collections;
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


        if (dir.x > 0)
        {
            currDir = Direction.right;
        } else if (dir.x < 0)
        {
            currDir = Direction.left;
        } else if (dir.y > 0)
        {
            currDir = Direction.up;
        } else
        {
            currDir = Direction.down;
        }


        if (CanMove(transform.position + dir, currDir))
            transform.position += dir;
        else
        {
            int randomDir = Random.Range(0, 3);
            switch (randomDir)
            {
                case 0:
                    if (CanMove(transform.position + new Vector3(1, 0, 0), Direction.right))
                        transform.position += new Vector3(1, 0, 0);
                    break;
                case 1:
                    if (CanMove(transform.position + new Vector3(-1, 0, 0), Direction.left))
                        transform.position += new Vector3(-1, 0, 0);
                    break;
                case 2:
                    if (CanMove(transform.position + new Vector3(0, 1, 0), Direction.up))
                        transform.position += new Vector3(0, 1, 0);
                    break;
                case 3:
                    if (CanMove(transform.position + new Vector3(0, -1, 0), Direction.down))
                        transform.position += new Vector3(0, -1, 0);
                    break;
                default:
                    break;
            }
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
