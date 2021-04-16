using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// "Projectile"
public class RayCastProj : Projectile
{
    public bool canHit = true;

    public GameObject thisPrefab;
    public Vector3 startPos;
    public Vector3 direction;
    public LineRenderer lineRenderer;
    
    public int ticksAlive;
    private int tickCount;
    public float alphaRate = 1;
    public float lineShrinkRate = 1;

    //private static int stop = 0;
    //public static int chain = 10;
    //private const int radius = 10;
    //protected bool chained = false;

    public override void ProjectileTick(int tick)
    {
        Color sc = lineRenderer.startColor;
        Color ec = lineRenderer.endColor;
        float a = sc.a;

        tickCount -= 1;
        if (tickCount <= 0)
        {
            ProjectileManager.RemoveProjectile(gameObject);
        }
        else
        {
            a *= alphaRate;
            lineRenderer.startColor = new Color(sc.r, sc.g, sc.b, a);
            lineRenderer.endColor = new Color(ec.r, ec.g, ec.b, a);
            float w = lineRenderer.startWidth * lineShrinkRate;
            lineRenderer.startWidth = w;
            lineRenderer.endWidth = w;
        }
    }

    public override void SetValues(Projectile other)
    {
        RayCastProj rcOther = (RayCastProj)other;
        baseDamage = rcOther.baseDamage;
        basePierce = rcOther.basePierce;
        ticksAlive = rcOther.ticksAlive;
        alphaRate = rcOther.alphaRate;
        //chained = false;

        canHit = rcOther.canHit;
        targets = rcOther.targets;

        lineRenderer.startColor = rcOther.lineRenderer.startColor;
        lineRenderer.endColor = rcOther.lineRenderer.endColor;
        lineRenderer.startWidth = rcOther.lineRenderer.startWidth;
        lineRenderer.endWidth = rcOther.lineRenderer.endWidth;
    }

    public void Cast()
    {
        int targetLayerMask = targets;
        int wallLayerMask = Entity.wallLayerMask;
        tickCount = ticksAlive;

        if (CalculatePierce() <= 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(startPos, direction, Mathf.Infinity, targetLayerMask | wallLayerMask);
            if (hit && !ignoredColliders.Contains(hit.collider) && canHit)
            {
                Enemy e = hit.transform.GetComponent<Enemy>();
                if (e != null)
                {
                    e.TakeDamage(CalculateDamage(), direction);
                }
                PlayerSegmentHealth p = hit.transform.GetComponent<PlayerSegmentHealth>();
                if (p != null)
                {
                    p.TakeDamage(CalculateDamage());
                }

                // create effect
                lineRenderer.SetPosition(0, startPos);
                lineRenderer.SetPosition(1, hit.point);
            }
        }
        else
        {
            RaycastHit2D wallHit = Physics2D.Raycast(startPos, direction, Mathf.Infinity, wallLayerMask);

            ContactFilter2D targetFilter = new ContactFilter2D();
            targetFilter.layerMask = targetLayerMask;
            targetFilter.useLayerMask = true;
            RaycastHit2D[] hits = new RaycastHit2D[CalculatePierce()];

            int numHit = Physics2D.Raycast(startPos, direction, targetFilter, hits, wallHit.distance);

            for (int i = 0; i < numHit; i++)
            {
                if (hits[i] && !ignoredColliders.Contains(hits[i].collider) && canHit)
                {
                    Enemy e = hits[i].transform.GetComponent<Enemy>();
                    if (e != null)
                    {
                        e.TakeDamage(CalculateDamage(), direction);
                        //Lightning(e);
                    }
                    // create effect
                    PlayerSegmentHealth p = hits[i].transform.GetComponent<PlayerSegmentHealth>();
                    if (p != null)
                    {
                        p.TakeDamage(CalculateDamage());
                    }
                }
            }
            if (wallHit.point == Vector2.zero)
            {
                Debug.Log("dick");
            }
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, wallHit.point);
        }
    }
    /*private void Lightning(Enemy e)
    {
        if (chained)
            return;
        int enemynum = 0;
        Collider2D[] enemies = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), radius, targets);
        for( int i = chain; i > 0; i--)
        {
            if (enemynum < enemies.Length && enemies[enemynum].transform.position - e.GetComponent<Collider2D>().transform.position == Vector3.zero)
                enemynum++;
            if(enemynum < enemies.Length)
            {
                GameObject c = ProjectileManager.CreateProjectile(thisPrefab);
                RayCastProj rc = c.GetComponent<RayCastProj>();
                c.transform.position = e.transform.position;
                rc.chained = true;
                rc.startPos = e.transform.position;
                rc.direction = enemies[enemynum].transform.position - e.transform.position;
                if(stop == 3)
                    Debug.Log(rc.direction);
                rc.Cast();
                enemynum++;
            }
        }
        stop++;
    }*/
}
