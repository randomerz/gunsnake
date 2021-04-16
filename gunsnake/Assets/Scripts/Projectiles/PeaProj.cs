using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeaProj : BasicProjectile
{
    public GameObject thisPrefab;
    public static int split = 1;
    public int toSplit;
    // Start is called before the first frame update

    // Update is called once per frame
    new void Awake()
    {
        SetSplit(split);
        base.Awake();
    }
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (ignoredColliders.Contains(other))
        {
            return;
        }

        if (other.tag == "Enemy" && !hitEnemyThisTile)
        {
            Enemy e = other.gameObject.GetComponent<Enemy>();
            e.TakeDamage(CalculateDamage(), direction);
            Split(other);
            basePierce -= 1;
            if (CalculatePierce() < 0)
                ProjectileManager.RemoveProjectile(gameObject);

            hitEnemyThisTile = true;
        }
        if (other.tag == "Wall")
        {
            ProjectileManager.RemoveProjectile(gameObject);
        }
    }
    private void Split(Collider2D c)
    {
        if (toSplit > 0)
        {
            Debug.Log("penis");
            GameObject pea1 = ProjectileManager.CreateProjectile(thisPrefab);
            GameObject pea2 = ProjectileManager.CreateProjectile(thisPrefab);
            pea1.transform.position = transform.position;
            pea2.transform.position = transform.position;
            pea1.transform.rotation = transform.rotation;
            pea2.transform.rotation = transform.rotation;
            pea1.GetComponent<PeaProj>().direction = Vector3.Cross(Vector3.forward, direction);
            pea2.GetComponent<PeaProj>().direction = Vector3.Cross(Vector3.back, direction);
            pea1.GetComponent<PeaProj>().IgnoreCollision(c);
            pea2.GetComponent<PeaProj>().IgnoreCollision(c);
            pea1.transform.Rotate(0f, 0f, 90f);
            pea2.transform.Rotate(0f, 0f, -90f);
            toSplit--;
            pea1.GetComponent<PeaProj>().SetSplit(toSplit);
            pea2.GetComponent<PeaProj>().SetSplit(toSplit);
        }
    }
    private void SetSplit(int p)
    {
        toSplit = p;
    }
}
