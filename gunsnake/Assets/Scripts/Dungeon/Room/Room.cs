using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{
    normal,
    challenge,
    shop,
    loot,
    entrance,
    exit,
}

[RequireComponent(typeof(Collider2D))]
public class Room : MonoBehaviour
{
    //public enum RoomDrop { health, gold, key }
    //public RoomDrop roomDrop = RoomDrop.health;

    public RoomData roomData;


    //  === Data for other classes ===
    public List<Door> doors;

    public bool didPlayerEnter;


    //  === Internal logic ===
    
    // for counting when player enters
    private int numSegmentsIn; // could be buggy doing it this way


    // combat
    private bool isInCombat;
    //[SerializeField]
    private List<Enemy> activeEnemies;

    private const int MAX_WAVES = 5;
    private List<List<Enemy>> waves; // may want to have different wave spawn conditions
    private int currentWave;

    // entrance/exit
    public static GameObject entrancePrefab;
    public static GameObject exitPrefab;

    void Awake()
    {
        // get object in children door => if door isn't locked add to thing

        switch (roomData.roomType)
        {
            case RoomType.normal:
                activeEnemies = new List<Enemy>();

                InitWaves();
                foreach (List<Enemy> wave in waves)
                    foreach (Enemy e in wave)
                        e.doTick = false;
                

                break;

            case RoomType.challenge:
                activeEnemies = new List<Enemy>();
                
                InitWaves();
                foreach (List<Enemy> wave in waves)
                    foreach (Enemy e in wave)
                        e.gameObject.SetActive(false);

                break;

            case RoomType.entrance:
                didPlayerEnter = true;
                break;

            case RoomType.exit:
                break;
        }
    }

    void Update()
    {
        switch (roomData.roomType)
        {
            case RoomType.normal:
                if (!isInCombat)
                    break;

                for (int i = activeEnemies.Count - 1; i >= 0; i--)
                {
                    if (!activeEnemies[i].gameObject.activeSelf)
                    {
                        activeEnemies.RemoveAt(i);
                    }
                }

                if (activeEnemies.Count == 0)
                {
                    if (currentWave == waves.Count)
                    {
                        isInCombat = false;
                        Debug.Log("Room complete!");
                    }
                    else
                    {
                        SetNextWaveDoTick(true);
                    }
                }
                break;

            case RoomType.challenge:
                if (!isInCombat)
                    break;

                for (int i = activeEnemies.Count - 1; i >= 0; i--)
                {
                    if (!activeEnemies[i].gameObject.activeSelf)
                    {
                        activeEnemies.RemoveAt(i);
                    }
                }

                if (activeEnemies.Count == 0)
                {
                    if (currentWave == waves.Count)
                    {
                        isInCombat = false;
                        foreach (Door d in doors)
                        {
                            if (!d.isLocked)
                                d.SetIsClosed(false);
                        }
                        Debug.Log("Room complete!");
                    }
                    else
                    {
                        SetNextWaveActive(true);
                    }
                }
                break;
        }
    }

    #region Combat

    private void InitWaves()
    {
        waves = new List<List<Enemy>>();

        // Room_WxH_Doors
        // - Enemies
        //   - Wave1
        //     - Slime
        //     - ..
        //   - Wave2
        //     - Slime
        //     - ..
        GameObject waveContainer = transform.Find("Enemies").gameObject;
        for (int i = 0; i < MAX_WAVES; i++)
        {
            Transform enemyContainer = waveContainer.transform.Find("Wave" + i);
            if (enemyContainer != null)
            {
                waves.Add(GetWave(enemyContainer.gameObject));
            }
        }

        // Room_WxH_Doors
        // - Enemies
        //   - Slime
        //   - ..
        if (waves.Count == 0)
        {
            waves.Add(GetWave(waveContainer));
        }
    }

    private List<Enemy> GetWave(GameObject container)
    {
        List<Enemy> wave = new List<Enemy>();
        foreach (Enemy e in container.GetComponentsInChildren<Enemy>())
        {
            EnemyManager.AddToCurrentLevelEnemies(e);
            wave.Add(e);
        }
        return wave;
    }

    private void SetNextWaveActive(bool value)
    {
        activeEnemies = waves[currentWave];
        currentWave++;

        foreach (Enemy e in activeEnemies)
        {
            e.gameObject.SetActive(value);
        }
    }

    private void SetNextWaveDoTick(bool value)
    {
        activeEnemies = waves[currentWave];
        currentWave++;

        foreach (Enemy e in activeEnemies)
        {
            e.doTick = value;
        }
    }

    #endregion

    #region Entrance/Exit

    public void SpawnEnterance(Direction side)
    {
        GameObject gameObj = SpawnGate(entrancePrefab, side);
        LevelHandler.startObject = gameObj;
        LevelHandler.startDirection = (Direction)(((int)side + 2) % 4);
    }

    public void SpawnExit(Direction side)
    {
        GameObject gameObj = SpawnGate(exitPrefab, side);
        LevelHandler.endObject = gameObj;
    }

    private GameObject SpawnGate(GameObject gatePrefab, Direction side)
    {
        Vector3 spawnPos = Vector3.zero;

        switch (side)
        {
            case Direction.right:
                spawnPos = new Vector3(roomData.width - 2, roomData.height / 2);
                break;
            case Direction.up:
                spawnPos = new Vector3(roomData.width / 2, roomData.height - 2);
                break;
            case Direction.left:
                spawnPos = new Vector3(1, roomData.height / 2);
                break;
            case Direction.down:
                spawnPos = new Vector3(roomData.width / 2, 1);
                break;
        }
        GameObject gameObj = Instantiate(gatePrefab, transform.position + spawnPos, Quaternion.identity, transform);

        return gameObj;
    }

    #endregion



    public void SetDoorRefs()
    {
        // Room_WxH_Doors
        // - Doors
        //   - Door
        //   - ..
        doors = new List<Door>();
        GameObject doorContainer = transform.Find("Doors").gameObject;
        List<RCConnection> connections = new List<RCConnection>();
        foreach (Door d in doorContainer.GetComponentsInChildren<Door>())
        {
            doors.Add(d);
            int x = (int)d.transform.position.x;
            int y = (int)d.transform.position.y;
            Direction dir;
            if (x == 0)
                dir = Direction.left;
            if (y == 0)
                dir = Direction.down;
            else if (x == roomData.width - 1)
                dir = Direction.right;
            else if (y == roomData.height - 1)
                dir = Direction.up;
            else
            {
                // manhattan distance to edges
                int dl = x + Mathf.Abs(y - roomData.height / 2);
                int dd = y + Mathf.Abs(x - roomData.width / 2);
                int dr = (roomData.width - x) + Mathf.Abs(y - roomData.height / 2);
                int du = (roomData.height - y) + Mathf.Abs(x - roomData.width / 2);

                int min = Mathf.Min(dl, dd, dr, du);
                if (dl == min)
                    dir = Direction.left;
                else if (dd == min)
                    dir = Direction.down;
                else if (dr == min)
                    dir = Direction.right;
                else
                    dir = Direction.up;
            }
            connections.Add(new RCConnection(new Vector3Int(x, y, 0), dir));
        }
        roomData.SetDefaultConnections(connections);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            numSegmentsIn += 1;
            if (numSegmentsIn == Player.body.Length && !didPlayerEnter)
            {
                didPlayerEnter = true;
                Debug.Log("Player entered room type " + roomData.roomType);

                switch (roomData.roomType)
                {
                    case RoomType.normal:
                        isInCombat = true;
                        break;
                    case RoomType.challenge:
                        isInCombat = true;
                        foreach (Door d in doors)
                        {
                            if (!d.isLocked)
                                d.SetIsClosed(true);
                        }
                        break;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            numSegmentsIn -= 1;
        }
    }
}
