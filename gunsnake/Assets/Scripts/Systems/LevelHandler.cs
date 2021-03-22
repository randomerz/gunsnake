using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Does level stuff
// Once level finishes, 
// Loads next floor in same scene if in same area (Jung 1-1 -> Jung 1-2)
// Loads new scene if different area (Jung 1-2 -> Dung 1-1)


public class LevelHandler : MonoBehaviour
{
    public GameObject playerPrefab;

    public static string currentArea; // "Jungle", "Dungeon", "Temple"
    public static int currentFloor; // 0, 1
    public const int numFloors = 2;

    public static string jungleSceneName = "Dungeon";
    public static string dungeonSceneName = "Dungeon";
    public static string templeSceneName = "Dungeon";

    // Set by DungeonRoomPlacer
    public static GameObject startObject;
    public static Direction startDirection;
    public static GameObject endObject;

    [Header("References")]
    public DungeonGenerator dungeonGen;


    private static LevelHandler instance;

    private void Awake()
    {
        if (currentArea == null)
        {
            currentArea = "Jungle";
            currentFloor = 0;
        }

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


    }

    public void EndLevel()
    {
        Debug.Log("Level handler called to end level!");
        Player.playerEffects.SetPlayerExiting();
        // add a between level UI for short cut quests, etc
        
        // temp
        StartNextLevel();
    }

    public static void StartNextLevel()
    {
        currentFloor += 1;
        if (currentFloor < numFloors)
        {
            LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            currentFloor = 0;
            if (currentArea == "Jungle")
            {
                currentArea = "Dungeon";
                LoadScene(dungeonSceneName);
            }
            else if (currentArea == "Dungeon")
            {
                currentArea = "Temple";
                LoadScene(templeSceneName);
            }
            else
            {
                WinGame();
            }
        }
    }

    public static void RestartGame()
    {
        currentArea = "Jungle";
        currentFloor = 0;
        LoadScene(jungleSceneName);
    }

    private static void LoadScene(string sceneName)
    {
        Debug.Log("Now loading " + currentArea + "-" + (currentFloor + 1));
        TimeTickSystem.ClearDelegates();
        ProjectileManager.ResetAllProjectiles();
        EnemyManager.ResetAllEnemies();
        SceneManager.LoadScene(sceneName);
    }

    private static void WinGame()
    {
        Debug.Log("You won the game!");
    }
}
