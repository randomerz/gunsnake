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
    [HideInInspector]
    public string codename;
}

public class ArtifactManager : MonoBehaviour
{
    public int numArtifacts;
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

    // Start is called before the first frame update
    void Start()
    {
        artifactList = new Artifact[numArtifacts];
        artifactList[(int)ArtIndex.attack] = attack;
        artifactList[(int)ArtIndex.health] = health;
        artifactList[(int)ArtIndex.pierce] = pierce;
        for (int i = 0; i < artifactList.Length; i++)
        {
            artifactList[i].codename = System.Enum.GetName(ArtIndex, i);
            freqsum += artifactList[i].freq;
        }
    }

    public Artifact GenArtifact()
    {
        float random = Random.Range(0, freqsum);
        for (int i = 0; i < artifactList.Length; i++)
        {
            if (random < artifactList[i].freq)
            {
                return artifactList[i];
            }
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
            tempfreq[i] = retArts[i].freq;
            retArts[i].freq = 0;
        }
        for(int i = 0; i < num; i++)
        {
            retArts[i].freq = tempfreq[i];
        }
        return retArts;
    }
    public void AddArtifact(Artifact a)
    {
        a.count++;
        //if(count == maxcount)
        //{
        // a.freq = 0;
        //}
    }
    //update get methods as daniel needs?
    public Artifact[] GetArtifacts()
    {
        return artifactList;
    }
}
