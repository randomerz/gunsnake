using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// "Projectile"
public class RayCastProj : Projectile
{
    public Vector3 startPos;
    public Vector3 direction;
    public LineRenderer lineRenderer;
    
    public int ticksAlive;
    private int tickCount;
    public float alphaRate;

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
        }
    }

    public override void SetValues(Projectile other)
    {
        RayCastProj rcOther = (RayCastProj)other;
        baseDamage = rcOther.baseDamage;
        basePierce = rcOther.basePierce;
        ticksAlive = rcOther.ticksAlive;
        alphaRate = rcOther.alphaRate;

        lineRenderer.startColor = rcOther.lineRenderer.startColor;
        lineRenderer.endColor = rcOther.lineRenderer.endColor;
        lineRenderer.startWidth = rcOther.lineRenderer.startWidth;
        lineRenderer.endWidth = rcOther.lineRenderer.endWidth;
    }

    public void Cast()
    {
        int targetLayerMask = Entity.fullHeightEntitiesMask;
        int wallLayerMask = Entity.wallLayerMask;
        tickCount = ticksAlive;

        if (CalculatePierce() <= 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(startPos, direction, Mathf.Infinity, targetLayerMask | wallLayerMask);
            if (hit)
            {
                Enemy e = hit.transform.GetComponent<Enemy>();
                if (e != null)
                {
                    e.TakeDamage(CalculateDamage(), direction);
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
            RaycastHit2D[] hits = new RaycastHit2D[CalculatePierce()];

            int numHit = Physics2D.Raycast(startPos, direction, targetFilter, hits, wallHit.distance);

            for (int i = 0; i < numHit; i++)
            {
                if (hits[i])
                {
                    Enemy e = hits[i].transform.GetComponent<Enemy>();
                    if (e != null)
                    {
                        e.TakeDamage(CalculateDamage(), direction);
                    }
                    // create effect
                }
            }

            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, wallHit.point);
        }
    }
}
