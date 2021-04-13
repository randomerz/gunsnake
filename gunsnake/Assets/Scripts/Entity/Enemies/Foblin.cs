using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foblin : Enemy
{
    [Header("Foblin")]
    [Tooltip("1 = 4 game ticks")]
    public int attackSpeed;
    private int baseAttackSpeed;
    private int ticksTillAttack;

    public int chargeLength;
    public float chargeChance;
    private bool preCharge;
    private int chargeCount;
    private Vector3 chargeDir;

    protected override void Awake()
    {
        base.Awake();

        baseAttackSpeed = attackSpeed;

        if (randomizeStartingVars)
            ticksTillAttack = Random.Range(1, attackSpeed);
        else
            ticksTillAttack = attackSpeed;

        myName = "foblin";
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
                case 4:
                case 3:
                    if (animator != null && preCharge)
                        SetAnimatorBool("isPrep", true);
                    break;
                case 2:
                case 1:
                    if (animator != null)
                        SetAnimatorBool("isPrep", true);
                    break;
                default:
                    if (ticksTillAttack <= 0)
                    {
                        ticksTillAttack = attackSpeed;

                        if (!preCharge && chargeCount == 0)
                        {
                            // attack if within range
                            GameObject closestSeg = GetClosestPlayerSegment();
                            if ((closestSeg.transform.position - transform.position).magnitude <= 1)
                            {
                                // visuals
                                if (animator != null)
                                    SetAnimatorBool("isAttack", true);

                                AudioManager.Play("foblin_attack");

                                Attack(closestSeg);
                            }
                            // else move closer
                            else
                            {
                                Move(GetDirectionToPlayer(false));

                                if (Random.Range(0f, 1f) < chargeChance)
                                {
                                    if ((closestSeg.transform.position - transform.position).magnitude > 4)
                                    {
                                        AudioManager.Play("foblin_charge");

                                        preCharge = true;
                                        ticksTillAttack = 4;
                                        chargeCount = chargeLength;
                                        chargeDir = GetDirectionToPlayer(false);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (preCharge)
                            {
                                preCharge = false;
                            }

                            chargeCount -= 1;
                            ticksTillAttack = 1;
                            Move(chargeDir);

                            if (animator != null)
                                SetAnimatorBool("isAttack", true);

                            // if near player stop charge
                            GameObject closestSeg = GetClosestPlayerSegment();
                            if ((closestSeg.transform.position - transform.position).magnitude <= 1)
                            {
                                chargeLength = 0;

                                // visuals
                                if (animator != null)
                                    SetAnimatorBool("isAttack", true);

                                AudioManager.Play("foblin_attack");

                                Attack(closestSeg);
                            }
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


        if (CanMoveForEnemy(transform.position, currDir))
        {
            MoveDir(dir);
        }
        else
        {
            Direction randomDir = (Direction)Random.Range(0, 4);
            if (CanMoveForEnemy(transform.position, randomDir))
                MoveDir(DirectionUtil.Convert(randomDir));

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
