using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Entity
{
    // stats
    public int maxHealth = -1;
    protected int health;

    public int damage = 1;
    public bool randomizeStartingVars;
    public bool hasSpawnEffect = false;
    public bool dropsSmallGold = true;
    public bool reducesCurseOnKill = false;

    protected string myName = "";

    // game stuff
    public bool doTick = true;
    public bool doDrop = true;
    [HideInInspector()]
    public GameObject itemDrop;

    // movement
    private Vector3 oldPos;

    private List<GameObject> effects = new List<GameObject>();
    protected Vector3 lastHitDir = Vector3.zero; // i have no idea how to properly do htis
    private bool strobing;
    private Material oldMat;

    [Header("References")]
    public SpriteRenderer spriteRenderer;
    public GeneralEnemyAnimator animator;

    public static Material whiteFlashMat; // set in GameHandler.cs
    public static GameObject deathParticle; // set in GameHandler.cs
    public static GameObject preSpawnParticle; // set in GameHandler.cs
    public static GameObject spawnParticle; // set in GameHandler.cs

    protected override void Awake()
    {
        base.Awake();
        health = maxHealth;

        if (spriteRenderer == null)
            Debug.LogError("Enemy doesn't have SpriteRenderer reference!");
        oldMat = spriteRenderer.material;
        if (animator != null && animator.animator != null)
            animator.animator.SetBool("isDead", false);

        myCollider = GetComponent<Collider2D>();

        if (hasSpawnEffect)
            StartCoroutine(SpawnAfterDelay());
        else
            Spawn();
    }

    public abstract void EnemyTick(int tick);


    private void Spawn()
    {
        CheckSpawn();

        EnemyManager.AddEnemy(this);
    }

    private IEnumerator SpawnAfterDelay()
    {
        bool origDoTick = doTick;
        doTick = false;
        myCollider.enabled = false;
        spriteRenderer.enabled = false;

        Instantiate(preSpawnParticle, transform.position, Quaternion.identity, transform);
        yield return new WaitForSeconds(1.4f);
        Instantiate(spawnParticle, transform.position, Quaternion.identity, transform);
        yield return new WaitForSeconds(.1f);
        Spawn();

        doTick = origDoTick;
        myCollider.enabled = true;
        spriteRenderer.enabled = true;
    }

    private void CheckSpawn()
    {
        // normal
        if (CanSpawn(transform.position))
            return;

        // try something around it
        Vector3[] difPos = { new Vector3(1, 0), new Vector3(0, 1), new Vector3(-1, 0), new Vector3(0, -1),
                             new Vector3(1, 1), new Vector3(-1, 1), new Vector3(-1, -1), new Vector3(1, -1) };
        foreach (Vector3 dif in difPos)
        {
            if (CanSpawn(transform.position + dif))
            {
                transform.position += dif;
                return;
            }
        }
    }

    private bool CanSpawn(Vector2 pos)
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(pos);
        foreach (Collider2D hit in hits)
        {
            if (hit && hit != myCollider && hit.tag != "Room")
            {
                //Debug.Log("Couldn't place! Conflicting with " + hit.name);
                return false;
            }
        }
        return true;
    }


    #region Health

    public virtual void TakeDamage(int damage, Vector3 hitDirection)
    {
        lastHitDir = hitDirection.normalized;
        TakeDamage(damage);
    }

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        doTick = true;
        if (health <= 0)
        {
            Die();
        }
        else
        {
            StrobeWhite(1);

            AudioManager.Play(myName + "_damage");
        }
    }
    
    public virtual void Die()
    {
        if (animator != null && animator.animator != null)
            animator.animator.SetBool("isDead", true);

        if (myName != "")
        {
            AudioManager.Play(myName + "_die");
        }

        foreach (GameObject e in effects)
        {
            Destroy(e);
        }

        if (itemDrop != null)
            Instantiate(itemDrop, transform.position, Quaternion.identity, transform.parent);

        if (TempleCurseSystem.isEnabled && reducesCurseOnKill)
        {
            TempleCurseSystem.GetKill();
        }

        if (deathParticle != null && lastHitDir != Vector3.zero)
        {
            double angle = Mathf.Atan2(lastHitDir.y, lastHitDir.x) * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.Euler(0, 0, (float)angle);
            GameObject particle = Instantiate(deathParticle, transform.position, rot, transform.parent);
            if (animator != null)
            {
                ParticleSystem.MainModule settings = particle.GetComponent<ParticleSystem>().main;
                settings.startColor = new ParticleSystem.MinMaxGradient(animator.deathParticlesColor);
            }
        }
        EnemyManager.RemoveEnemy(gameObject);
    }

    #endregion


    #region Movement

    // manhattan distance
    public GameObject GetClosestPlayerSegment()
    {
        GameObject closest = Player.body[0];
        float dist = Manhattan(transform.position, closest.transform.position);
        for (int i = 1; i < Player.body.Length; i++)
        {
            if (Manhattan(transform.position, Player.body[i].transform.position) < dist)
            {
                closest = Player.body[i];
                dist = Manhattan(transform.position, closest.transform.position);
            }
        }
        return closest;
    }

    public Vector3 GetDirectionToPlayer(bool shouldDiagonal)
    {
        Vector3 dir = Vector3.zero;
        Vector3 vecToPlayer = Player.body[0].transform.position - transform.position;
        // tiebreaker
        Vector3 vecToPlayerTwo = Player.body[1].transform.position - transform.position;

        float angle = Mathf.Atan2(vecToPlayer.y, vecToPlayer.x) * Mathf.Rad2Deg;
        float angle2 = Mathf.Atan2(vecToPlayerTwo.y, vecToPlayerTwo.x) * Mathf.Rad2Deg;

        if (angle < 0)  angle += 360;
        if (angle2 < 0) angle2 += 360;

        if ((!shouldDiagonal && angle % 90 == 45) || (shouldDiagonal && angle % 45 == 22.5))
            dir = DirectionFromAngle(shouldDiagonal, angle2);
        else
            dir = DirectionFromAngle(shouldDiagonal, angle);

        return dir;
    }

    private Vector3 DirectionFromAngle(bool shouldDiagonal, float angle)
    {
        Vector3 dir = Vector3.zero;

        if (!shouldDiagonal)
        {
            if (angle > 315)
                dir = Vector3.right;
            else if (angle > 225)
                dir = Vector3.down;
            else if (angle > 135)
                dir = Vector3.left;
            else if (angle > 45)
                dir = Vector3.up;
            else
                dir = Vector3.right;
        }
        else
        {
            if (angle > 337.5)
                dir = Vector3.right;
            else if (angle > 292.5)
                dir = new Vector3(1, -1);
            else if (angle > 247.5)
                dir = Vector3.down;
            else if (angle > 202.5)
                dir = new Vector3(-1, -1);
            else if (angle > 157.5)
                dir = Vector3.left;
            else if (angle > 112.5)
                dir = new Vector3(-1, 1);
            else if (angle > 67.5)
                dir = Vector3.up;
            else if (angle > 22.5)
                dir = new Vector3(1, 1);
            else
                dir = Vector3.right;
        }

        return dir;
    }

    protected void MoveDir(Vector3 dir)
    {
        if (myName != "")
        {
            AudioManager.Play(myName + "_move");
        }

        animator.SetOrigPos(transform.position);
        if (dir.x > 0)
            animator.SetFacing(false);
        else if (dir.x < 0)
            animator.SetFacing(true);

        Vector3 oldPos = transform.position;

        transform.position += dir;

        StartCoroutine(CheckToRevertOnNextFixedUpdate(oldPos));
    }

    IEnumerator CheckToRevertOnNextFixedUpdate(Vector3 oldPos)
    {
        yield return new WaitForFixedUpdate();

        Collider2D[] hits = new Collider2D[2];
        int numHit = Physics2D.OverlapPointNonAlloc(transform.position, hits, fullCollidableMask | playerLayerMask);
        if (numHit > 1)
        {
            transform.position = oldPos;
        }
    }

    #endregion


    protected void SetAnimatorBool(string name, bool value)
    {
        if (animator != null || animator.animator != null)
        {
            animator.animator.SetBool("isIdle", false);
            animator.animator.SetBool("isPrep", false);
            animator.animator.SetBool("isAttack", false);
            animator.animator.SetBool(name, value);
        }
    }

    public void AddEffect(GameObject effectPrefab)
    {
        effects.Add(Instantiate(effectPrefab, transform));
    }

    #region Strobe Color

    protected void StrobeColor(int _strobeCount, Color _toStrobe)
    {
        if (strobing)
            return;

        strobing = true;

        StartCoroutine(StrobeColorHelper(0, (_strobeCount * 2) - 1));
    }

    protected void StrobeWhite(int _strobeCount)
    {
        StrobeColor(_strobeCount, Color.white);
    }

    private IEnumerator StrobeColorHelper(int _i, int _stopAt)
    {
        if (_i <= _stopAt)
        {
            if (_i % 2 == 0)
                spriteRenderer.material = whiteFlashMat;
            else
                spriteRenderer.material = oldMat;

            yield return new WaitForSeconds(0.125f);
            StartCoroutine(StrobeColorHelper((_i + 1), _stopAt));
        }
        else
        {
            strobing = false;
        }
    }

    #endregion
}
