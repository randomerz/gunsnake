using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactManager : MonoBehaviour
{
    public GameObject art_prefab;
    private GameObject[] myartifacts;
    private string[] artifacts = { "health", "attack" };
    private string[] curr_artifacts = { };
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void createArtifact(string name)
    {
        Instantiate(art_prefab, new Vector3(0, 0, 0), Quaternion.identity);
        art_prefab.getComponent<
    }
    void artifactCollect(GameObject a)
    {
        a.getComponent<
    }
}
