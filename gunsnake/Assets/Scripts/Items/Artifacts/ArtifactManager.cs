using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactManager : MonoBehaviour
{

    public Item[] artifactList;
    private bool didInit = false;

    public static ArtifactManager _instance;

    void Awake()
    {
        Initialize();
    }
    public void ResetArtifacts()
    {
        if (!didInit)
            Initialize();
        for (int i = 0; i < artifactList.Length; i++)
        {
            artifactList[i].count = 0;
        }
        EggProj.explode = 0;
        PeaProj.split = 0;
        RayCastProj.chain = 0;
    }
    private void Initialize()
    {
        if (_instance == null)
            _instance = this;
        didInit = true;
    }
    public void AddArtifact(Item a)
    {
        a.count++;
    }

    public Item[] GetArtifacts()
    {
        return artifactList;
    }
}
