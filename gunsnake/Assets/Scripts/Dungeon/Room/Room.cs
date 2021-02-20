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
    public enum RoomDrop { health, gold, key }

    public RoomData roomData;
    public RoomDrop roomDrop = RoomDrop.health;


    //  === Data for other classes ===
    public List<Door> doors;

    public bool didPlayerEnter;


    //  === Internal logic ===
    
    // for counting when player enters
    private int numSegmentsIn; // could be buggy doing it this way

    private bool isInCombat;
    //[SerializeField]
    private List<Enemy> activeEnemies;

    private const int MAX_WAVES = 5;
    private List<List<Enemy>> waves; // may want to have different wave spawn conditions
    private int currentWave;

    void Awake()
    {
        // get object in children door => if door isn't locked add to thing

        switch (roomData.roomType)
        {
            case RoomType.normal: 
                activeEnemies = new List<Enemy>();
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
                        List<Enemy> wave = new List<Enemy>();
                        foreach (Enemy e in enemyContainer.GetComponentsInChildren<Enemy>())
                        {
                            e.gameObject.SetActive(false);
                            wave.Add(e);
                        }
                        waves.Add(wave);
                    }
                }

                // Room_WxH_Doors
                // - Enemies
                //   - Slime
                //   - ..
                if (waves.Count == 0)
                {
                    List<Enemy> wave = new List<Enemy>();
                    foreach (Enemy e in waveContainer.GetComponentsInChildren<Enemy>())
                    {
                        e.gameObject.SetActive(false);
                        wave.Add(e);
                    }
                    waves.Add(wave);
                }

                SetDoorRefs();

                break;

            case RoomType.entrance:
                didPlayerEnter = true;
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
                        foreach (Door d in doors)
                        {
                            if (!d.isLocked)
                                d.SetIsClosed(false);
                        }
                        Debug.Log("Room complete!");
                    }
                    else
                    {
                        SpawnNextWave();
                    }
                }
                break;
        }
    }


    public void SetDoorRefs()
    {
        // Room_WxH_Doors
        // - Doors
        //   - Door
        //   - ..
        doors = new List<Door>();
        GameObject doorContainer = transform.Find("Doors").gameObject;
        foreach (Door d in doorContainer.GetComponentsInChildren<Door>())
        {
            doors.Add(d);
        }
    }

    #region Combat

    private void SpawnNextWave()
    {
        activeEnemies = waves[currentWave];
        currentWave++;

        foreach (Enemy e in activeEnemies)
        {
            e.gameObject.SetActive(true);
        }
    }

    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            numSegmentsIn += 1;
            if (numSegmentsIn == Player.body.Length && !didPlayerEnter)
            {
                didPlayerEnter = true;
                //Debug.Log("Player entered room " + gameObject.name);

                switch (roomData.roomType)
                {
                    case RoomType.normal:
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
