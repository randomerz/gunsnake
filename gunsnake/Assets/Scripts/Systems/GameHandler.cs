using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public Material enemyWhiteFlashMat;

    private void Awake()
    {

    }

    void Start()
    {
        TimeTickSystem.Create();

        Enemy.whiteFlashMat = enemyWhiteFlashMat;
    }
    
    void Update()
    {
        
    }
}
