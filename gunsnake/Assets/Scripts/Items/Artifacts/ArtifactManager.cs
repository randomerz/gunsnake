using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// delete this file
//[System.Serializable]
//public class Artifact
//{
//    public float freq;
//    //public int maxcount;
//    [HideInInspector]
//    public int count = 0;
//    public string codename;
//}

public class ArtifactManager : MonoBehaviour
{
    public Item attack;
    public Item health;
    public Item pierce;
    public Item dodger;

    private Item[] artifactList;
    private bool didInit = false;
    //private float freqsum = 0;

    public static ArtifactManager _instance;

    public enum ArtIndex
    {
        attack,
        health,
        pierce,
        dodger,
    }

    void Awake()
    {
        Initialize();
        //for (int i = 0; i < artifactList.Length; i++)
        //{
        //    artifactList[i].codename = System.Enum.GetName(typeof(ArtIndex), i);
        //    freqsum += artifactList[i].freq;
        //}
    }
    public void ResetArtifacts()
    {
        if (!didInit)
            Initialize();
        for (int i = 0; i < artifactList.Length; i++)
        {
            artifactList[i].count = 0;
        }
    }
    private void Initialize()
    {
        if (_instance == null)
            _instance = this;
        artifactList = new Item[System.Enum.GetNames(typeof(ArtIndex)).Length];
        artifactList[(int)ArtIndex.attack] = attack;
        artifactList[(int)ArtIndex.health] = health;
        artifactList[(int)ArtIndex.pierce] = pierce;
        artifactList[(int)ArtIndex.dodger] = dodger;
        didInit = true;
    }
    //    private float ResetFreq(Artifact a)
    //    {
    //        float retFreq = a.freq;
    //        a.freq = 0;
    //        freqsum -= retFreq;
    //        return retFreq;
    //    }
    //    private Artifact GenArtifact()
    //    {
    //        float random = Random.Range(0, freqsum);
    //        for (int i = 0; i < artifactList.Length; i++)
    //        {
    //            if (random < artifactList[i].freq)
    //                return artifactList[i];
    //            random -= artifactList[i].freq;
    //        }
    //        return null;
    //    }
    //    public Artifact[] UniqueGenArtifacts(int num)
    //    {
    //        float[] tempfreq = new float[num];
    //        Artifact[] retArts = new Artifact[num];
    //        for(int i = 0; i < num; i++)
    //        {
    //            retArts[i] = GenArtifact();
    //            tempfreq[i] = ResetFreq(retArts[i]);
    //        }
    //        for (int i = 0; i < num; i++)
    //        {
    //            retArts[i].freq = tempfreq[i];
    //            freqsum += tempfreq[i];
    //        }
    //        return retArts;
    //    }
    public void AddArtifact(Item a)
    {
        a.count++;
        //if(count == maxcount)
        // ResetFreq(a);
    }

    public Item[] GetArtifacts()
    {
        return artifactList;
    }
}
