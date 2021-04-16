using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _body = new GameObject[4];
    public static GameObject[] body = new GameObject[4];
    [SerializeField]
    private SpriteRenderer[] _sprites = new SpriteRenderer[4];
    public static SpriteRenderer[] sprites = new SpriteRenderer[4];

    [SerializeField]
    private PlayerEffects _playerEffects;
    public static PlayerEffects playerEffects;
    [SerializeField]
    private PlayerHealth _playerHealth;
    public static PlayerHealth playerHealth;
    [SerializeField]
    private PlayerMovement _playerMovement ;
    public static PlayerMovement playerMovement;
    [SerializeField]
    private PlayerWeaponManager _playerWeaponManager;
    public static PlayerWeaponManager playerWeaponManager;
    [SerializeField]
    private ArtifactManager _ArtifactManager;
    public static ArtifactManager artifactManager;

    private bool didInit = false;
    [SerializeField]
    private bool debugResetPlayer;

    private static float timeTaken;
    private static float score; // gold count

    private void Awake()
    {
        InitReferences();
        TimeTickSystem.OnTick_PlayerMove += TimeTickSystem_OnTick;

        if (debugResetPlayer)
            ResetSnakeToDefault();
    }

    private void Start()
    {

    }

    private void Update()
    {
        // do in update or OnTick?
        timeTaken += Time.deltaTime;
    }


    private void TimeTickSystem_OnTick(object sender, TimeTickSystem.OnTickEventArgs e)
    {
        playerHealth.OnTick(e.tick);
        playerMovement.OnTick(e.tick);
    }

    public void InitReferences()
    {
        if (didInit)
            return;

        didInit = true;

        body = _body;
        sprites = _sprites;
        playerEffects = _playerEffects;
        playerHealth = _playerHealth;
        playerMovement = _playerMovement;
        playerWeaponManager = _playerWeaponManager;
        artifactManager = _ArtifactManager;

        playerEffects.InitReferences();

        //ResetSnakeToDefault();
    }

    public static void ResetSnakeToDefault()
    {
        timeTaken = 0;
        score = 0;

        playerHealth.ResetValuesToDefault();
        PlayerInventory.ResetValues();
        playerWeaponManager.ResetWeaponsToDefault();
        artifactManager.ResetArtifacts();
    }

    public static void EndGame(bool didWin)
    {
        UIManager.EndGame(didWin, (int)timeTaken, (int)score);
        if (didWin)
        {
            // This is called by LevelHandler.WinGame();
        }
        else
        {
            LevelHandler.LoseGame();
        }
    }

    public static void AddScore(float amount)
    {
        score += amount;
    }

    // my favorite method
    public static GameObject GetHead()
    {
        return body[0];
    }
}
