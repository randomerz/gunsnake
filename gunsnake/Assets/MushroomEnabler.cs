using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomEnabler : MonoBehaviour
{
    public Tile[] turrets;

    public void SetEnable(bool value)
    {
        foreach (Tile t in turrets)
        {
            t.isTileEnabled = value;
        }
    }
}
