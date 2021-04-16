using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelHandler : MonoBehaviour
{
    public GameObject playerPrefab;

    public static string currentArea; // "Jungle", "Dungeon", "Temple"
    public static int currentFloor; // 0, 1
    public const int numFloors = 2;

    public static string jungleSceneName = "Jungle";
    public static string dungeonSceneName = "Dungeon";
    public static string templeSceneName = "Temple";
    public static string templeBossSceneName = "Temple Boss";

    // Set by DungeonRoomPlacer
    public static GameObject startObject;
    public static Direction startDirection;
    public static GameObject endObject;

    [Header("References")]
    public DungeonGenerator dungeonGen;
    public Fog fog;


    private static LevelHandler instance;
    private static GameObject playerObj;

    private bool didInit = false;
    private static bool shouldResetPlayer = true;

    [Header("Fade")]
    public Fade f;
    public GameObject darkness;
    public TextMeshProUGUI levelNameText;
    public float levelTime = .04f;
    public void Awake()
    {
        if (currentArea == null)
        {
            SetToJungle();
        }

        if (!didInit)
        {
            Initialize();
        }

        if (shouldResetPlayer)
        {
            shouldResetPlayer = false;
            Player.ResetSnakeToDefault();
        }
    }

    void Start()
    {
        StartLevel();
    }

    public static LevelHandler GetInstance()
    {
        return instance;
    }

    public void Initialize()
    {
        if (didInit)
            return;

        didInit = true;

        instance = this;

        // spawn player
        playerObj = GameObject.Find(instance.playerPrefab.name);
        if (playerObj == null)
            playerObj = Instantiate(instance.playerPrefab);
        playerObj.GetComponent<Player>().InitReferences();
    }

    public void StartLevel()
    {
        // audio
        if (currentArea == "Jungle")
        {
            AudioManager.PlayMusic("music_jungle");
        }
        else if (currentArea == "Dungeon")
        {
            AudioManager.PlayMusic("music_dungeon");
        }
        else if (currentArea == "Temple" || currentArea == "Temple Boss")
        {
            AudioManager.PlayMusic("music_temple");
        }

        // dungeon stuff
        dungeonGen.CreateDungeon();
        if (fog != null)
            fog.Init();
        EnemyManager.InitializeEnemyDrops();
        TempleCurseSystem.Reset();

        //fade shows the title, pauses time as well
        StartCoroutine(ShowingLevelTitle());

        //player.transform.position;
        Player.playerMovement.SetSnakeSpawn(startObject.transform.position, startDirection);
        startObject.GetComponent<TeleporterTile>().SetTeleporterAnimation(false, 2);
        Player.playerEffects.SetPlayerEntering();
    }

    public void EndLevel()
    {
        Debug.Log("Level handler called to end level!");

        Player.playerEffects.SetPlayerExiting();
        // add a between level UI for short cut quests, etc

        StartCoroutine(FadeOut());
        // temp
        //StartNextLevel();
    }

    public static void StartNextLevel()
    {
        if (currentArea == "Temple Boss")
        {
            WinGame();
            return;
        }

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
            else if (currentArea == "Temple")
            {
                currentArea = "Temple Boss";
                LoadScene(templeBossSceneName);
            }
            else
            {
                Debug.LogWarning("Tried loading an unknown area! Current area: " + currentArea + ", current floor: " + currentFloor);
            }
        }
    }

    public static void SetToJungle()
    {
        currentArea = "Jungle";
        currentFloor = 0;
    }

    public static void RestartGame()
    {
        SetToJungle();
        shouldResetPlayer = true;
        TempleCurseSystem.isEnabled = false;
        LoadScene(jungleSceneName);
    }

    public static void ClearEnemiesAndProjectiles()
    {
        EnemyManager.ClearCurrentEnemies();
        ProjectileManager.ClearAllProjectiles();
    }

    public static string GetCurrentLevelAsString()
    {
        if (currentArea == "Temple Boss")
            return "Temple - X";
        return currentArea + " - " + (currentFloor + 1);
    }

    private static void LoadScene(string sceneName)
    {
        Debug.Log("Now loading " + GetCurrentLevelAsString());
        TimeTickSystem.ClearDelegates();
        ProjectileManager.ResetAllProjectiles();
        EnemyManager.ResetAllEnemies();
        UpdateEnemyBonusHealth();
        SceneManager.LoadScene(sceneName);
    }

    private static void UpdateEnemyBonusHealth()
    {
        switch (currentArea)
        {
            case "Jungle":
                break;
            case "Dungeon":
                EnemyManager.levelBonusHealth = 1;
                break;
            case "Temple":
            case "Temple Boss":
                EnemyManager.levelBonusHealth = 2;
                break;
        }
    }

    private static void WinGame()
    {
        //AudioManager.PlayMusic("music_win");

        instance.darkness.SetActive(false);
        Player.EndGame(true);
    }

    public static void LoseGame()
    {
        //AudioManager.PlayMusic("music_lose");
    }

    //fade stuff
    public IEnumerator ShowingLevelTitle()
    {
        darkness.SetActive(true);
        UIManager.canOpen = false;
        Time.timeScale = 0;
        levelNameText.text = GetCurrentLevelAsString();

        yield return new WaitForSecondsRealtime(levelTime);

        UIManager.canOpen = true;
        Time.timeScale = 1;
        StartCoroutine(HidingTitle());
    }

    public IEnumerator HidingTitle()
    {
        f.FadeOut();
        yield return new WaitForSeconds(f.Duration);
        darkness.SetActive(false);
    }

    public IEnumerator FadeOut()
    {
        f.FadeOut();
        darkness.SetActive(true);
        UIManager.canOpen = false;
        levelNameText.text = "";

        yield return new WaitForSeconds(f.Duration);

        UIManager.canOpen = true;
        StartNextLevel();
    }
}
