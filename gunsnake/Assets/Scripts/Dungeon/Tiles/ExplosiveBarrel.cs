using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : Enemy
{
    [SerializeField]
    private bool isPrimed;
    private int primedTick = -1;
    private bool didExplode;


    public Sprite primedSprite;
    public GameObject explosionFX;


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
                    StartCoroutine(Explode());
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

    private IEnumerator Explode()
    {
        didExplode = true;

        // AudioManager.PlaySound("explode");

        CameraShake.Shake(.75f, 0.5f);
        explosionFX.SetActive(true);

        spriteRenderer.enabled = false;
        hitbox.enabled = false;


        
        //ContactFilter2D targetFilter = new ContactFilter2D();
        //targetFilter.layerMask = fullHeightEntitiesMask;
        //RaycastHit2D[] hits = new RaycastHit2D[99];
        //int numHit = Physics2D.Raycast(startPos, direction, targetFilter, hits, wallHit.distance);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 2f, fullHeightEntitiesMask | playerLayerMask);

        for (int i = 0; i < hits.Length; i++)
        {
            //Debug.Log("Explosion hit " + hits[i].name + " at " + hits[i].transform.position);

            Enemy e = hits[i].transform.GetComponent<Enemy>();
            if (e != null)
            {
                e.TakeDamage(damage, (e.transform.position - transform.position).normalized);
            }
            // create effect
            PlayerSegmentHealth p = hits[i].transform.GetComponent<PlayerSegmentHealth>();
            if (p != null)
            {
                p.TakeDamage(damage);
            }
        }

        yield return new WaitForSeconds(2);

        Die();
    }

    public override void Die()
    {
        EnemyManager.RemoveEnemy(gameObject);
    }
}
