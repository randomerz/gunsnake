using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    [SerializeField]
    private Material enemyWhiteFlashMat;

    private void Awake()
    {
        Enemy.whiteFlashMat = enemyWhiteFlashMat;
        TimeTickSystem.Create();
    }

    void Start()
    {

    }
    
    void Update()
    {
        
    }
}
