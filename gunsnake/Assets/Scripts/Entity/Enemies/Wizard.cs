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
    private int attackCount;
    private int summonThresh = 2;

    // === Boss Stuff ===
    private int curPhase;

    private GameObject curEnemySpawn;
    public GameObject slime0Prefab;
    public GameObject slime1Prefab;
    public GameObject slime2Prefab;
    public GameObject explosiveBarrelPrefab;

    public GameObject shieldSprite;
    private bool isShielded;
    private int shieldTicks;

    // Phase 0

    public GameObject[] spawnLocs;

    // Phase 1
    public int phase1Cutoff;
    public GameObject spikeContainer;

    // Phase 2
    public int phase2Cutoff;
    public GameObject turretContainer;
    public GameObject[] turrets;

    protected override void Awake()
    {
        base.Awake();
        if (randomizeStartingVars)
            ticksTillAttack = Random.Range(1, attackSpeed);
        else
            ticksTillAttack = attackSpeed;

        myName = "wizard";

        SetPhase(0);
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
                            break;
                        }
                        else
                        {
                            justAttacked = true;

                            // visuals
                            if (animator != null)
                                SetAnimatorBool("isAttack", true);

                            AudioManager.Play("wizard_attack");


                            // Attacking code
                            if (attackCount >= summonThresh)
                            {
                                // Summon
                                attackCount = 0;

                                Summon();
                            }
                            else
                            {
                                // AoE attack
                                attackCount += 1;

                                Attack();
                            }
                        }
                    }
                    break;
            }

            // other nonstandard

            if (isShielded)
            {
                shieldTicks -= 1;
                if (shieldTicks < 0)
                {
                    SetShieldStatus(false);
                }
            }

        }

        // other updates not %4 tick
        animator.UpdatePosition();
    }

    public override void TakeDamage(int damage)
    {
        doTick = true;
        if (!isShielded)
        {
            health -= damage;

            if (curPhase == 2 && health <= 0)
            {
                Die();
            }
            else
            {
                StrobeWhite(1);

                AudioManager.Play(myName + "_damage");
            }
        }

        // check phase stuff + timing

        CheckPhase();

    }

    public override void Die()
    {
        base.Die();

        Player.playerHealth.SetInvulnerable(8);

        ScreenFlash.Flash(0.5f);
        CameraShake.Shake(3f, 1f);

        LevelHandler.ClearEnemiesAndProjectiles();
        spikeContainer.SetActive(false);
        foreach (GameObject g in turrets)
            g.GetComponent<Tile>().isTileEnabled = false;
        turretContainer.SetActive(false);
    }

    private void SetShieldStatus(bool value)
    {
        shieldSprite.SetActive(value);
        isShielded = value;
        if (value)
            shieldTicks = 16;
    }

    private void CheckPhase()
    {
        switch (curPhase)
        {
            case 0:
                if (health < phase1Cutoff)
                    SetPhase(1);
                break;
            case 1:
                if (health < phase2Cutoff)
                    SetPhase(2);
                break;
        }
    }

    private void SetPhase(int phase)
    {
        curPhase = phase;
        switch (curPhase)
        {
            case 0:
                curEnemySpawn = slime0Prefab;
                break;
            case 1:
                health = phase1Cutoff;
                SetShieldStatus(true);

                ScreenFlash.Flash(0.5f);

                curEnemySpawn = slime1Prefab;
                spikeContainer.SetActive(true);

                break;
            case 2:
                health = phase2Cutoff;
                SetShieldStatus(true);

                ScreenFlash.Flash(0.5f);

                curEnemySpawn = slime2Prefab;
                turretContainer.SetActive(true);
                foreach (GameObject g in turrets)
                {
                    g.GetComponent<Tile>().isTileEnabled = true;
                }

                break;
        }
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

    private void Summon()
    {
        foreach (GameObject g in spawnLocs)
        {
            //GameObject e = EnemyManager.CreateEnemy(curEnemySpawn);
            //e.transform.position = g.transform.position;
            Instantiate(curEnemySpawn, g.transform.position, Quaternion.identity, transform.parent);
        }
    }
}
