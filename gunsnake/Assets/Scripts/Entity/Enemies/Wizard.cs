using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : Enemy
{
    [Header("Wizard")]
    [Tooltip("1 = 4 game ticks")]
    public int attackSpeed;
    private int ticksTillAttack;
    public GameObject bulletPrefab;

    private bool justAttacked = false;

    protected override void Awake()
    {
        base.Awake();
        if (randomizeStartingVars)
            ticksTillAttack = Random.Range(1, attackSpeed);
        else
            ticksTillAttack = attackSpeed;

        myName = "wizard";
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
                case 2:
                case 1:
                    if (!justAttacked)
                    {
                        if (animator != null)
                            SetAnimatorBool("isPrep", true);
                    }
                    break;
                default:
                    if (ticksTillAttack <= 0)
                    {
                        ticksTillAttack = attackSpeed;

                        if (justAttacked)
                        {
                            justAttacked = false;
                            Move(GetDirectionToPlayer(false));
                        }
                        else
                        {
                            justAttacked = true;

                            // visuals
                            if (animator != null)
                                SetAnimatorBool("isAttack", true);

                            AudioManager.Play("wizard_attack");

                            Attack();
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

    private void Attack()
    {
        //       PlayerSegmentHealth h = seg.GetComponent<PlayerSegmentHealth>();
        //      if (h != null)
        //       {
        //            h.TakeDamage(damage);
        //       }

   
        Vector3[] directions = {new Vector3(1, 0, 0), new Vector3(-1, 0, 0), new Vector3(0, 1, 0), new Vector3 (0, -1, 0),
        new Vector3(1, 1, 0), new Vector3(-1, 1, 0), new Vector3(1, -1, 0), new Vector3(-1, -1, 0)};

        for (int i = 0; i < directions.Length; i++)
        {
            GameObject proj = ProjectileManager.CreateProjectile(bulletPrefab);
            BasicProjectile ep = proj.GetComponent<BasicProjectile>();
            proj.transform.position = transform.position;
            proj.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(directions[i].y, directions[i].x) * Mathf.Rad2Deg);
            ep.direction = directions[i]; 
        }
    }
}
