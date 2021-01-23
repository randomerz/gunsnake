using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _body = new GameObject[4];
    public static GameObject[] body = new GameObject[4];

    [SerializeField]
    private PlayerHealth _playerHealth;
    public static PlayerHealth playerHealth;
    [SerializeField]
    private PlayerMovement _playerMovement ;
    public static PlayerMovement playerMovement;
    [SerializeField]
    private PlayerWeaponManager _playerWeaponManager;
    public static PlayerWeaponManager playerWeaponManager;

    private void Awake()
    {
        body = _body;
        playerHealth = _playerHealth;
        playerMovement = _playerMovement;
        playerWeaponManager = _playerWeaponManager;
    }
}
