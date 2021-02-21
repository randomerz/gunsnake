using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artifact : MonoBehaviour
{
    private GameObject player;
    private int artifact_code;
    private int artifact_val;
    public Artifact(GameObject p)
    {
        player = p;
    }
    public Artifact(GameObject p, int an, int av)
    {
        player = p;
        artifact_code = an;
        artifact_val = av;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter(Collider other)
    {
        if(other.GameObject == player)
        {
            
        }
    }
}
