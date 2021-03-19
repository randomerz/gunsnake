using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    public int damage;
    public int pierce; // a bullet with 1 pierce goes through 1 enemy

    public abstract void ProjectileTick(int tick);
    public abstract void SetValues(Projectile other);
}
