using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : Enemy
{
    [Header("Box Loot Table")]
    public TableEntry[] table;
    private static float tableSum = -1;

    public GameObject extraParticles;

    [System.Serializable]
    public class TableEntry
    {
        public string name;
        public float freq = 1; // default 1, 0.5 = half as often, 2 = twice as often
        public GameObject value;
    }

    protected override void Awake()
    {
        base.Awake();

        UpdateSums();

        itemDrop = GetEntry();
    }

    public override void EnemyTick(int tick)
    {
        // do nothing
    }

    private void UpdateSums()
    {
        tableSum = 0;
        for (int j = 0; j < table.Length; j++)
        {
            tableSum += table[j].freq;
        }
    }

    public GameObject GetEntry() // make this better lol
    {
        if (tableSum == -1)
            UpdateSums();
        float random = Random.Range(0, tableSum);
        GameObject ret = null;

        for (int i = 0; i < table.Length; i++)
        {
            if (random < table[i].freq)
            {
                ret = table[i].value;
                break;
            }
            random -= table[i].freq;
        }

        return ret;
    }

    public override void Die()
    {
        base.Die();

        // extra particles
        if (extraParticles != null && lastHitDir != Vector3.zero)
        {
            double angle = Mathf.Atan2(lastHitDir.y, lastHitDir.x) * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.Euler(0, 0, (float)angle);
            GameObject particle = Instantiate(extraParticles, transform.position, rot, transform.parent);
        }
    }
}
