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

    // Set by DungeonRoomPlacer
    public static GameObject startObject;
    public static Direction startDirection;
    public static GameObject endObject;

    [Header("References")]
    public DungeonGenerator dungeonGen;


    private static LevelHandler instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        StartLevel();
    }

    public static LevelHandler GetInstance()
    {
        return instance;
    }

    public void StartLevel()
    {
        // dungeon stuff
        dungeonGen.CreateDungeon();
        EnemyManager.InitializeEnemyDrops();

        // spawn player
        GameObject player = GameObject.Find(playerPrefab.name);
        if (player == null)
            player = GameObject.Instantiate(playerPrefab);
        //player.transform.position;
        Player.playerMovement.SetSnakeSpawn(startObject.transform.position, startDirection);
        Player.playerEffects.SetPlayerEntering();
        // fade screen in


        // pick an enemy (not in challenge room) and give it a key
    }

    public void EndLevel()
    {
        Debug.Log("Level handler called to end level!");
        Player.playerEffects.SetPlayerExiting();
    }
}
