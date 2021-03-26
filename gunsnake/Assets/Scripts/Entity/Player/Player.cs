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

    private bool didInit = false;

    private void Awake()
    {
        InitReferences();
        TimeTickSystem.OnTick_PlayerMove += TimeTickSystem_OnTick;
    }

    private void Start()
    {

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

        playerEffects.InitReferences();

        //ResetSnakeToDefault();
    }

    public static void ResetSnakeToDefault()
    {
        playerHealth.ResetValuesToDefault();
        playerWeaponManager.ResetWeaponsToDefault();
    }

    // my favorite method
    public static GameObject GetHead()
    {
        return body[0];
    }
}
