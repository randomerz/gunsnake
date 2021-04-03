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

                        Debug.Log("Wizard atkin?");
                        Attack();
                        GameObject closestSeg = GetClosestPlayerSegment();
                        Move(GetDirectionToPlayer(false));
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
        else {
            int randomDir = Random.Range(0, 3);
            switch (randomDir)
            {
                case 0:
                    if (CanMove(transform.position + new Vector3(1, 0, 0)))
                        transform.position += new Vector3(1, 0, 0);
                    break;
                case 1:
                    if (CanMove(transform.position + new Vector3(-1, 0, 0)))
                        transform.position += new Vector3(-1, 0, 0);
                    break;
                case 2:
                    if (CanMove(transform.position + new Vector3(0, 1, 0)))
                        transform.position += new Vector3(0, 1, 0);
                    break;
                case 3:
                    if (CanMove(transform.position + new Vector3(0, -1, 0)))
                        transform.position += new Vector3(0, -1, 0);
                    break;
                default:
                    break;
            }
        }
    }

    private void Attack()
    {
        //       PlayerSegmentHealth h = seg.GetComponent<PlayerSegmentHealth>();
        //      if (h != null)
        //       {
        //            h.TakeDamage(damage);
        //       }

        Debug.Log("Wizard atking");
        Vector3[] directions = {new Vector3(1, 0, 0), new Vector3(-1, 0, 0), new Vector3(0, 1, 0), new Vector3 (0, -1, 0),
        new Vector3(1, 1, 0), new Vector3(-1, 1, 0), new Vector3(1, -1, 0), new Vector3(-1, -1, 0)};

        for (int i = 0; i < directions.Length; i++)
        {
            GameObject proj = ProjectileManager.CreateProjectile(bulletPrefab);
            EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
            proj.transform.position = transform.position;
            proj.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(directions[i].y, directions[i].x) * Mathf.Rad2Deg);
            ep.direction = directions[i]; 
        }
    }
}
