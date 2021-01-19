using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _body = new GameObject[4];
    public static GameObject[] body = new GameObject[4];

    private void Awake()
    {
        body = _body;
    }
}
