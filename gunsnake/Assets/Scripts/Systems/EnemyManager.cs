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

    private static EnemyManager _instance;

    private static GameObject enemyContainer;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            enemyContainer = new GameObject("EnemyContainer");
        }
    }

    private void Start()
    {
        TimeTickSystem.OnTick_Enemies += TimeTickSystem_OnTick;
    }

    private void TimeTickSystem_OnTick(object sender, TimeTickSystem.OnTickEventArgs e)
    {
        if (e.tick % 4 == 0)
        {
            foreach (List<GameObject> enemyList in activeEnemies.Values)
            {
                foreach (GameObject g in enemyList)
                {
                    if (g.GetComponent<Enemy>() != null && g.activeSelf)
                    {
                        g.GetComponent<Enemy>().EnemyTick(e.tick);
                    }
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

        activeEnemies[type].Add(enemy);
        return enemy;
    }

    public static void AddEnemy(Enemy enemy)
    {
        Type type = enemy.GetType();
        if (!inactiveEnemies.ContainsKey(type))
        {
            //Debug.Log("Creating new projectile container: " + type);
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
}
