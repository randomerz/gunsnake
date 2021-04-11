using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggProj : BasicProjectile
{
    public GameObject explosionPrefab;
    public static int explode = 0;
    private int toExplode;

    new void Awake()
    {
        SetExplode(explode);
    }
    void Update()
    {
        
    }
    public void SetExplode(int e)
    {
        toExplode = e;
    }
}
