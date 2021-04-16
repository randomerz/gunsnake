using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private static Dictionary<Type, List<GameObject>> activeEnemies = 
        new Dictionary<Type, List<GameObject>>();
    private static Dictionary<Type, List<GameObject>> inactiveEnemies = 
        new Dictionary<Type, List<GameObject>>();
    private static List<Enemy> currentLevelEnemies = new List<Enemy>();
    public static int levelBonusHealth = 0;

    private static EnemyManager _instance;

    private static GameObject enemyContainer;

    private static int enemiesKilled = 0;
    
    // could be static
    public GameObject smallGoldDrop;
    public GameObject medGoldDrop;
    public GameObject healthDrop;
    public GameObject keyDrop;
    public float healthDropRate = 0.1f;

    public GameObject keyEffect;

    private void Awake()
    {
        //if (_instance == null)
        //{
        _instance = this;
        enemyContainer = new GameObject("EnemyContainer");
        //}
        TimeTickSystem.OnTick_Enemies += TimeTickSystem_OnTick;
    }

    private void Start()
    {

    }

    private void TimeTickSystem_OnTick(object sender, TimeTickSystem.OnTickEventArgs e)
    {
        foreach (List<GameObject> enemyList in activeEnemies.Values)
        {
            foreach (GameObject g in enemyList)
            {
                Enemy enemy = g.GetComponent<Enemy>();
                if (enemy != null && enemy.doTick && g.activeSelf)
                {
                    enemy.EnemyTick(e.tick);
                }
            }
        }
    }

    public static void InitializeEnemyDrops()
    {
        if (currentLevelEnemies.Count == 0)
        {
            Debug.LogError("Tring to initialize drops with 0 enemies in currentLevelEnemies!");
            return;
        }

        Debug.Log("Setting drops for " + currentLevelEnemies.Count + " enemies");

        bool didAddKey = false;
        int failCount = 0;
        int randInd = 0;

        while (!didAddKey && failCount < 10000)
        {
            randInd = UnityEngine.Random.Range(0, currentLevelEnemies.Count);
            if (currentLevelEnemies[randInd].doDrop)
            {
                didAddKey = true;
                currentLevelEnemies[randInd].itemDrop = _instance.keyDrop;
                currentLevelEnemies[randInd].AddEffect(_instance.keyEffect);
            }
            else
                failCount++;
        }

        if (!didAddKey)
        {
            Debug.LogWarning("Failed to add key to enemy!");
            randInd = 0;
            currentLevelEnemies[randInd].itemDrop = _instance.keyDrop;
            currentLevelEnemies[randInd].AddEffect(_instance.keyEffect);
        }

        for (int i = 0; i < currentLevelEnemies.Count; i++)
        {
            if (i != randInd) {
                if (UnityEngine.Random.Range(0f, 1f) < _instance.healthDropRate)
                    currentLevelEnemies[i].itemDrop = _instance.healthDrop;
                else
                {
                    if (currentLevelEnemies[i].dropsSmallGold)
                        currentLevelEnemies[i].itemDrop = _instance.smallGoldDrop;
                    else
                        currentLevelEnemies[i].itemDrop = _instance.medGoldDrop;
                }
            }
        }
    }



    public static GameObject CreateEnemy(GameObject enemyPrefab)
    {
        GameObject enemy;
        Type type = enemyPrefab.GetComponent<Enemy>().GetType();
        if (!inactiveEnemies.ContainsKey(type))
        {
            //Debug.Log("Creating new projectile container: " + type);
            inactiveEnemies.Add(type, new List<GameObject>());
            activeEnemies.Add(type, new List<GameObject>());
        }

        int len = inactiveEnemies[type].Count;
        if (len > 0)
        {
            enemy = inactiveEnemies[type][len - 1];
            inactiveEnemies[type].RemoveAt(len - 1);
            enemy.SetActive(true);
        }
        else
        {
            enemy = Instantiate(enemyPrefab, enemyContainer.transform);
        }

        enemy.GetComponent<Enemy>().GainBonusHealth(levelBonusHealth);

        activeEnemies[type].Add(enemy);
        return enemy;
    }

    public static void AddEnemy(Enemy enemy)
    {
        Type type = enemy.GetType();
        if (!inactiveEnemies.ContainsKey(type))
        {
            //Debug.Log("Creating new enemy container: " + type);
            inactiveEnemies.Add(type, new List<GameObject>());
            activeEnemies.Add(type, new List<GameObject>());
        }

        if (!activeEnemies[type].Contains(enemy.gameObject))
            activeEnemies[type].Add(enemy.gameObject);
    }

    public static void RemoveEnemy(GameObject enemy)
    {
        Type type = enemy.GetComponent<Enemy>().GetType();
        if (activeEnemies[type].Contains(enemy))
        {
            enemiesKilled++;
            if (enemiesKilled % 10 == 0)
                Player.playerHealth.Lifesteal(true);
            inactiveEnemies[type].Add(enemy);
            activeEnemies[type].Remove(enemy);
            enemy.SetActive(false);
            enemy.transform.parent = enemyContainer.transform;
        }
        else
        {
            Debug.LogWarning("Could not remove enemy! Something went wrong.");
            enemy.SetActive(false);
            enemy.transform.parent = enemyContainer.transform;
        }
    }

    public static void AddToCurrentLevelEnemies(Enemy enemy)
    {
        currentLevelEnemies.Add(enemy);
    }

    public static void ResetAllEnemies()
    {
        enemiesKilled = 0;
        activeEnemies.Clear();
        inactiveEnemies.Clear();
        currentLevelEnemies.Clear();
    }

    public static void ClearCurrentEnemies()
    {
        foreach (Type enemyType in activeEnemies.Keys)
        {
            for (int i = activeEnemies[enemyType].Count - 1; i >= 0; i--)
            {
                RemoveEnemy(activeEnemies[enemyType][i]);
            }
        }
    }
}
