using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public GameObject explosionFX;

    public LayerMask defaultTarget;
    public int defaultDamage;

    private bool didExplode;

    void Start()
    {
        Explode(defaultTarget, defaultDamage);
    }

    public void Explode(LayerMask target, int damage)
    {
        if (didExplode)
            return;

        didExplode = true;

        StartCoroutine(ExplodeHelper(target, damage));
    }

    private IEnumerator ExplodeHelper(LayerMask target, int damage)
    {
        AudioManager.Play("misc_explosion");

        CameraShake.Shake(.75f, 1.25f);
        explosionFX.SetActive(true);

        //spriteRenderer.enabled = false;
        //hitbox.enabled = false;



        //ContactFilter2D targetFilter = new ContactFilter2D();
        //targetFilter.layerMask = fullHeightEntitiesMask;
        //RaycastHit2D[] hits = new RaycastHit2D[99];
        //int numHit = Physics2D.Raycast(startPos, direction, targetFilter, hits, wallHit.distance);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 2f, target);

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

        //Die();
    }
}
