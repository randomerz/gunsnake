using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    private static Dictionary<Type, List<GameObject>> activeProjectiles = 
        new Dictionary<Type, List<GameObject>>();
    private static Dictionary<Type, List<GameObject>> inactiveProjectiles = 
        new Dictionary<Type, List<GameObject>>();

    private static ProjectileManager _instance;

    private static GameObject projectileContainer;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            projectileContainer = new GameObject("ProjectileContainer");
        }
    }

    private void Start()
    {
        TimeTickSystem.OnTick_Projectiles += TimeTickSystem_OnTick;
    }

    private void TimeTickSystem_OnTick(object sender, TimeTickSystem.OnTickEventArgs e)
    {
        //if (e.tick % 4 == 0)
        //{
        foreach (List<GameObject> projList in activeProjectiles.Values)
        {
            foreach (GameObject g in projList)
            {
                if (g.GetComponent<Projectile>() != null)
                {
                    g.GetComponent<Projectile>().ProjectileTick(e.tick);
                }
            }
        }
        //}
    }

    public static GameObject CreateProjectile(GameObject projectilePrefab)
    {
        GameObject proj;
        Type type = projectilePrefab.GetComponent<Projectile>().GetType();
        if (!inactiveProjectiles.ContainsKey(type))
        {
            //Debug.Log("Creating new projectile container: " + type);
            inactiveProjectiles.Add(type, new List<GameObject>());
            activeProjectiles.Add(type, new List<GameObject>());
        }

        int len = inactiveProjectiles[type].Count;
        if (len > 0)
        {
            proj = inactiveProjectiles[type][len - 1];
            inactiveProjectiles[type].RemoveAt(len - 1);
            proj.SetActive(true);
        }
        else
        {
            proj = Instantiate(projectilePrefab, projectileContainer.transform);
        }

        activeProjectiles[type].Add(proj);
        return proj;
    }

    public static void RemoveProjectile(GameObject proj)
    {
        Type type = proj.GetComponent<Projectile>().GetType();
        //Debug.Log("Destroying projectile of container: " + type);
        if (activeProjectiles[type].Contains(proj))
        {
            inactiveProjectiles[type].Add(proj);
            activeProjectiles[type].Remove(proj);
            proj.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Could not remove projectile: " + type + "! Something went wrong.");
            proj.SetActive(false);
        }
    }
}
