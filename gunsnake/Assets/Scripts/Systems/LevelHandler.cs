using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Does level stuff
// Once level finishes, 
// Loads next floor in same scene if in same area (Jung 1-1 -> Jung 1-2)
// Loads new scene if different area (Jung 1-2 -> Dung 1-1)


public class LevelHandler : MonoBehaviour
{
    public GameObject playerPrefab;

    public static GameObject startObject;
    public static GameObject endObject;

    void Start()
    {
        
    }

    public void StartLevel()
    {
        // spawn player
        // fade screen in

        
        // pick an enemy (not in challenge room) and give it a key
    }
}
