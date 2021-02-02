using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    // public bool use realTime; // or use ticks
    public float secondsTillDestroyed;

    void Start()
    {
        Destroy(gameObject, secondsTillDestroyed);
    }
}
