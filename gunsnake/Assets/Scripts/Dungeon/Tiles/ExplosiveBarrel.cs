using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : Enemy
{
    [SerializeField]
    private bool isPrimed;
    private int primedTick = -1;
    private bool didExplode;

    //public Explosion explosion;
    public GameObject explosionPrefab;

    public Sprite primedSprite;
    //public GameObject explosionFX;


    private Collider2D hitbox;

    protected override void Awake()
    {
        base.Awake();

        hitbox = GetComponent<Collider2D>();

        TimeTickSystem.OnTick_Dungeon += TimeTickSystem_OnTick; // lmao this is so nasty and crusty
    }

    private void TimeTickSystem_OnTick(object sender, TimeTickSystem.OnTickEventArgs e)
    {
        if (e.tick % 4 == 0)
        {
            if (isPrimed && !didExplode)
            {
                if (primedTick != e.tick)
                {
                    Explode();
                }
            }
        }
    }

    public override void EnemyTick(int tick)
    {
        // do nothing
    }

    public override void TakeDamage(int damage, Vector3 hitDirection)
    {
        if (!isPrimed)
        {
            Prime();
        }
    }

    private void Prime()
    {
        isPrimed = true;
        primedTick = TimeTickSystem.GetTick();

        spriteRenderer.sprite = primedSprite;

        Debug.Log("Primed tick: " + primedTick);
    }

    private void Explode()
    {
        didExplode = true;

        spriteRenderer.enabled = false;
        hitbox.enabled = false;

        //explosion.Explode(fullHeightEntitiesMask | playerLayerMask, damage);
        GameObject expGO = Instantiate(explosionPrefab, transform.position, Quaternion.identity, transform.parent);

        //yield return new WaitForSeconds(2);

        Die();
    }

    public override void Die()
    {
        EnemyManager.RemoveEnemy(gameObject);
    }
}
