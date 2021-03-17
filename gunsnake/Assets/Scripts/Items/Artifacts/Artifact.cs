using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artifact : MonoBehaviour
{
    private GameObject player;
    private int artifact_code;
    private int artifact_val;
    private string[] artifacts = { "health", "attack" };
    public Artifact(GameObject p, string name, int av)
    {
        player = p;
        artifact_code = System.Array.IndexOf(artifacts, name);
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
}
