using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    [SerializeField]
    private Material enemyWhiteFlashMat;
    [SerializeField]
    private GameObject enemyDeathParticle;
    [SerializeField]
    private GameObject enemyPreSpawnParticle;
    [SerializeField]
    private GameObject enemySpawnParticle;

    public Player player;
    public LevelHandler levelHandler;

    private void Awake()
    {
        Enemy.whiteFlashMat = enemyWhiteFlashMat;
        Enemy.deathParticle = enemyDeathParticle;
        Enemy.spawnParticle = enemySpawnParticle;
        Enemy.preSpawnParticle = enemyPreSpawnParticle;
        TimeTickSystem.Create();

        if (player != null)
            player.InitReferences();

        if (levelHandler != null)
            levelHandler.Initialize();
    }

    void Start()
    {

    }
    
    void Update()
    {
        
    }
}
