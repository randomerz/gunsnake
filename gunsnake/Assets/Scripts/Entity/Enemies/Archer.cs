using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Enemy
{
    [Header("Archer")]
    [Tooltip("1 = 4 game ticks")]
    public int attackSpeed;
    private int ticksTillAttack;
    public int moveSpeed;
    private int ticksTillMove;
    public int distToPlayerRange;
    public GameObject bulletPrefab;

    public GameObject attackIndicator;
    private Vector3 attackDir;

    protected override void Awake()
    {
        base.Awake();
        if (randomizeStartingVars)
            ticksTillAttack = attackSpeed + Random.Range(1, attackSpeed);
        else
            ticksTillAttack = attackSpeed;

        myName = "archer";

        attackIndicator.SetActive(false);
    }

    public override void EnemyTick(int tick)
    {
        if (tick % 4 == 0)
        {
            ticksTillAttack -= 1;
            ticksTillMove -= 1;

            // visuals
            if (animator != null)
                SetAnimatorBool("isIdle", true);

            switch (ticksTillAttack)
            {
                case 4:
                case 3:
                    if (animator != null)
                        SetAnimatorBool("isPrep", true);
                    break;
                case 2:
                case 1:
                    attackDir = GetDirectionToPlayer(true);
                    if (animator != null)
                        SetAnimatorBool("isPrep", true);

                    attackIndicator.transform.position = transform.position + attackDir * 0.5f;
                    attackIndicator.SetActive(true);

                    ticksTillMove += 2;
                    break;
                default:
                    if (ticksTillAttack <= 0)
                    {
                        ticksTillAttack = attackSpeed;

                        // visuals
                        if (animator != null)
                            SetAnimatorBool("isAttack", true);

                        AudioManager.Play("archer_attack");

                        Attack(attackDir);

                        attackIndicator.SetActive(false);
                    }
                    break;
            }

            switch (ticksTillMove)
            {
                default:
                    if (ticksTillMove <= 0)
                    {
                        ticksTillMove = moveSpeed;

                        // move to within range
                        GameObject closestSeg = GetClosestPlayerSegment();
                        if ((closestSeg.transform.position - transform.position).magnitude > 5)
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


        if (dir.x > 0)
        {
            currDir = Direction.right;
        }
        else if (dir.x < 0)
        {
            currDir = Direction.left;
        }
        else if (dir.y > 0)
        {
            currDir = Direction.up;
        }
        else
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

    private void Attack(Vector3 dir)
    {
        //  PlayerSegmentHealth h = seg.GetComponent<PlayerSegmentHealth>();
        //  if (h != null)
        //  {
        //      h.TakeDamage(damage);
        //  }
        GameObject proj = ProjectileManager.CreateProjectile(bulletPrefab);
        BasicProjectile ep = proj.GetComponent<BasicProjectile>();
        proj.transform.position = transform.position;
        proj.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        ep.direction = dir;
    }
}
