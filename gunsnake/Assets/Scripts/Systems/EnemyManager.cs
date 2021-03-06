﻿using System;
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

    private static EnemyManager _instance;

    private static GameObject enemyContainer;

    // could be static
    public GameObject goldDrop;
    public GameObject healthDrop;
    public GameObject keyDrop;
    public float healthDropRate = 0.1f;

    public GameObject keyEffect;

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

        int randInd = UnityEngine.Random.Range(0, currentLevelEnemies.Count);
        currentLevelEnemies[randInd].itemDrop = _instance.keyDrop;
        currentLevelEnemies[randInd].AddEffect(_instance.keyEffect);

        for (int i = 0; i < currentLevelEnemies.Count; i++)
        {
            if (i != randInd) {
                if (UnityEngine.Random.Range(0f, 1f) < _instance.healthDropRate)
                    currentLevelEnemies[i].itemDrop = _instance.healthDrop;
                else
                    currentLevelEnemies[i].itemDrop = _instance.goldDrop;
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

    public static void ResetCurrentLevelEnemies()
    {
        currentLevelEnemies.Clear();
    }
}
