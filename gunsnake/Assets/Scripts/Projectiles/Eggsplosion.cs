using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eggsplosion : MonoBehaviour
{
    public GameObject explodePrefab;
    private int explosions;
    private Vector3 pos;
    private int delay = 8;
    // Start is called before the first frame update
    
    public void Init(int i)
    {
        pos = transform.position;
        explosions = i;
        TimeTickSystem.OnTick_Projectiles += TimeTickSystem_OnTick;
    }

    // Update is called once per frame
    private void TimeTickSystem_OnTick(object sender, TimeTickSystem.OnTickEventArgs e)
    {
        if (e.tick % delay == 0)
        {
            if (explosions > 0)
            {
                GameObject expGO = Instantiate(explodePrefab, pos, Quaternion.identity, transform.parent);
                expGO.GetComponent<Explosion>().defaultTarget = Entity.fullHeightEntitiesMask;
                pos = transform.position + new Vector3(Random.Range(-1, 2), Random.Range(-1, 2), 0);
                explosions--;
            }
            else
            {
                Die();
            }
        }
    }
    private void Die()
    {
        Destroy(this.gameObject);
        TimeTickSystem.OnTick_Projectiles -= TimeTickSystem_OnTick;
    }
}
