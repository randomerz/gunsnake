using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    [SerializeField]
    private Material enemyWhiteFlashMat;
    [SerializeField]
    private GameObject enemyDeathParticle;

    private void Awake()
    {
        Enemy.whiteFlashMat = enemyWhiteFlashMat;
        Enemy.deathParticle = enemyDeathParticle;
        TimeTickSystem.Create();
    }

    void Start()
    {

    }
    
    void Update()
    {
        
    }
}
