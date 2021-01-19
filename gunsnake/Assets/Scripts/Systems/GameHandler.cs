using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    //[SerializeField]
    //private LayerMask _wallLayerMask;
    //public static LayerMask wallLayerMask;

    private void Awake()
    {
        //wallLayerMask = _wallLayerMask;
    }

    void Start()
    {
        TimeTickSystem.Create();

        //TimeTickSystem.OnTick += delegate (object sender, TimeTickSystem.OnTickEventArgs e)
        //{
        //    Debug.Log("tick: " + TimeTickSystem.GetTick());
        //};
        //TimeTickSystem.OnTick_4 += delegate (object sender, TimeTickSystem.OnTickEventArgs e)
        //{
        //    Debug.Log("QUAD");
        //};
    }
    
    void Update()
    {
        
    }
}
