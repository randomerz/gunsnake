using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LandingScreen : MonoBehaviour
{
    public string mainMenu;

    void Update()
    {
        if (Input.anyKey)
        {
            SceneManager.LoadScene(mainMenu);
        }
    }
}
