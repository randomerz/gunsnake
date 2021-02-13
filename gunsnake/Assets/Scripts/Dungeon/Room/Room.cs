using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Room : MonoBehaviour
{
    public RoomData roomData;

    //   data for other classes
    public bool didPlayerEnter;


    //   internal logic
    // private List<Door> externalDoors;
    private int numSegmentsIn; // could be buggy doing it this way

    private bool isInCombat;
    [SerializeField]
    private List<Enemy> activeEnemies;

    private const int MAX_WAVES = 5;
    private List<List<Enemy>> waves; // may want to have different wave spawn conditions
    private int currentWave;

    [SerializeField]
    private List<Door> unlockedDoors;

    void Awake()
    {
        // get object in children door => if door isn't locked add to thing

        switch (roomData.roomType)
        {
            case RoomData.RoomType.combat: 
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

                // Room_WxH_Doors
                // - Doors
                //   - Door
                //   - ..
                unlockedDoors = new List<Door>();
                GameObject doorContainer = transform.Find("Doors").gameObject;
                foreach (Door d in doorContainer.GetComponentsInChildren<Door>())
                {
                    if (!d.isLocked)
                        unlockedDoors.Add(d);
                }


                break;

            case RoomData.RoomType.entrance:
                didPlayerEnter = true;
                break;
        }
    }

    void Update()
    {
        switch (roomData.roomType)
        {
            case RoomData.RoomType.combat:
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
                        foreach (Door d in unlockedDoors)
                        {
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
                Debug.Log("Player entered room " + gameObject.name);

                switch (roomData.roomType)
                {
                    case RoomData.RoomType.combat:
                        isInCombat = true;
                        foreach (Door d in unlockedDoors)
                        {
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
