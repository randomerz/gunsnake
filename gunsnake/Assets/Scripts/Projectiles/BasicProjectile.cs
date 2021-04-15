using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : Projectile
{
    public Vector3 direction;
    public int moveRate = 2;

    private bool hitEnemyThisTile = false;
    private Vector3 origPosition;
    private Vector3 realPosition;

    private Coroutine animationCo;

    //  For making gifs
    //public void Awake()
    //{
        //base.Awake();

        //origPosition = transform.position;

        //TimeTickSystem.OnTick_Projectiles += TimeTickSystem_OnTick;
    //}

    //private void TimeTickSystem_OnTick(object sender, TimeTickSystem.OnTickEventArgs e)
    //{
    //    if (e.tick % 4 == 0)
    //        ProjectileTick(e.tick);
    //}

    public override void ProjectileTick(int tick)
    {
        CheckIfEnemyOnSquare();
        if (!isActiveAndEnabled)
            return;

        if (tick % moveRate == 0)
        {
            hitEnemyThisTile = false;

            //if (animationCo != null)
            //{
            //    transform.position = realPosition; // in case coroutine doesnt finish
            //}
            origPosition = transform.position;
            realPosition = transform.position + direction;
            animationCo = StartCoroutine(BulletAnimation(2));
            //transform.position = realPosition;
        }
        CheckIfEnemyOnSquare();
    }

    private IEnumerator BulletAnimation(int positionAnimationIndex)
    {
        while (positionAnimationIndex > 0)
        {
            positionAnimationIndex -= 1;
            switch (positionAnimationIndex)
            {
                case 1:
                    transform.position = 0.125f * origPosition + 0.875f * realPosition;
                    break;
                case 0:
                    transform.position = 0.0625f * origPosition + 0.9375f * realPosition;
                    break;
                default:
                    transform.position = realPosition;
                    break;
            }

            yield return null;
        }

        transform.position = realPosition;
        animationCo = null;
    }

    public override void SetValues(Projectile other)
    {
        baseDamage = other.baseDamage;
        basePierce = other.basePierce;
        targets = other.targets;
        moveRate = ((BasicProjectile)other).moveRate;
    }

    private void CheckIfEnemyOnSquare()
    {
        if (hitEnemyThisTile)
            return;

        Collider2D[] enemies = Physics2D.OverlapPointAll(transform.position, targets);
        foreach (Collider2D col in enemies)
        {
            OnTriggerEnter2D(col);

            if (hitEnemyThisTile)
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (ignoredColliders.Contains(other))
        {
            return;
        }
        // check if it is not in targets
        if ((targets & (1 << other.gameObject.layer)) == 0) {
            return;
        }

        if (other.tag == "Enemy" && !hitEnemyThisTile)
        {
            Enemy e = other.gameObject.GetComponent<Enemy>();
            e.TakeDamage(CalculateDamage(), direction);
            basePierce -= 1;
            if (CalculatePierce() < 0)
                ProjectileManager.RemoveProjectile(gameObject);

            hitEnemyThisTile = true;
        }

        if (other.tag == "Player" && !hitEnemyThisTile)
        {
            PlayerSegmentHealth p = other.gameObject.GetComponent<PlayerSegmentHealth>();
            p.TakeDamage(baseDamage);// CalculateDamage());
            basePierce -= 1;
            if (basePierce < 0) //CalculatePierce() < 0)
                ProjectileManager.RemoveProjectile(gameObject);

            hitEnemyThisTile = true;
        }

        if (other.tag == "Wall")
        {
            ProjectileManager.RemoveProjectile(gameObject);
        }
    }
}
