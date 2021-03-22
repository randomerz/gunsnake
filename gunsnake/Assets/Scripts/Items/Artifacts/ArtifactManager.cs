using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Artifact
{
    public float freq;
    //public int maxcount;
    [HideInInspector]
    public int count = 0;
    public string codename;
}

public class ArtifactManager : MonoBehaviour
{
    public Artifact attack;
    public Artifact health;
    public Artifact pierce;

    private Artifact[] artifactList;
    private float freqsum = 0;

    public static ArtifactManager _instance;

    public enum ArtIndex
    {
        attack,
        health,
        pierce,
    }

    void Awake()
    {
        if (_instance == null)
            _instance = this;
        artifactList = new Artifact[System.Enum.GetNames(typeof(ArtIndex)).Length];
        artifactList[(int)ArtIndex.attack] = attack;
        artifactList[(int)ArtIndex.health] = health;
        artifactList[(int)ArtIndex.pierce] = pierce;
        for (int i = 0; i < artifactList.Length; i++)
        {
            artifactList[i].codename = System.Enum.GetName(typeof(ArtIndex), i);
            freqsum += artifactList[i].freq;
        }
    }
    private float ResetFreq(Artifact a)
    {
        float retFreq = a.freq;
        a.freq = 0;
        freqsum -= retFreq;
        return retFreq;
    }
    private Artifact GenArtifact()
    {
        float random = Random.Range(0, freqsum);
        for (int i = 0; i < artifactList.Length; i++)
        {
            if (random < artifactList[i].freq)
                return artifactList[i];
            random -= artifactList[i].freq;
        }
        return null;
    }
    public Artifact[] UniqueGenArtifacts(int num)
    {
        float[] tempfreq = new float[num];
        Artifact[] retArts = new Artifact[num];
        for(int i = 0; i < num; i++)
        {
            retArts[i] = GenArtifact();
            tempfreq[i] = ResetFreq(retArts[i]);
        }
        for (int i = 0; i < num; i++)
        {
            retArts[i].freq = tempfreq[i];
            freqsum += tempfreq[i];
        }
        return retArts;
    }
    public void AddArtifact(Artifact a)
    {
        a.count++;
        //if(count == maxcount)
        // ResetFreq(a);
    }
    //update get methods as daniel needs?
    public Artifact[] GetArtifacts()
    {
        return artifactList;
    }
}
