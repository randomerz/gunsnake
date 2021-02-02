using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : Enemy
{
    [Tooltip("1 = 4 game ticks")]
    public int attackSpeed;
    private int ticksTillAttack;

    public GameObject mushroomAttackEffect;

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
                        Attack();
                    }
                    break;
            }
        }
    }

    private void Attack()
    {
        Collider2D h = Physics2D.OverlapBox(transform.position, new Vector2(1.5f, 1.5f), 0, playerLayerMask);
        // or use hitbox.OverlapCollider();
        if (h != null && h.tag == "Player")
        {
            h.GetComponent<PlayerSegmentHealth>().TakeDamage(damage);
        }

        // boonk gang bingo bongo hard coder
        Instantiate(mushroomAttackEffect, transform.position + Vector3.up, Quaternion.identity);
        Instantiate(mushroomAttackEffect, transform.position + Vector3.right, Quaternion.identity);
        Instantiate(mushroomAttackEffect, transform.position + Vector3.down, Quaternion.identity);
        Instantiate(mushroomAttackEffect, transform.position + Vector3.left, Quaternion.identity);
        Instantiate(mushroomAttackEffect, transform.position + Vector3.up + Vector3.right, Quaternion.identity);
        Instantiate(mushroomAttackEffect, transform.position + Vector3.right + Vector3.down, Quaternion.identity);
        Instantiate(mushroomAttackEffect, transform.position + Vector3.down + Vector3.left, Quaternion.identity);
        Instantiate(mushroomAttackEffect, transform.position + Vector3.left + Vector3.up, Quaternion.identity);
    }
}
