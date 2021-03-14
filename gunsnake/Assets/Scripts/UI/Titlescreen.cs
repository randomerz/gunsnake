using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Titlescreen : MonoBehaviour
{
    public static bool stopPlayerInput;

    public string newGameScene;

    [Header("Menu")]
    public GameObject creditsButton;
    public GameObject playButton;
    public GameObject quitButton;
    public GameObject optionButton;

    [Header("Volume + Sxf")]
    public GameObject volumeSlider;
    public float volumeNumber;
    public TextMeshProUGUI volumeCounter;

    public GameObject sxfSlider;
    public float sxfNumber;
    public TextMeshProUGUI sxfCounter;

    [Header("Options")]
    public GameObject optionPanel;
    public GameObject creditsPanel;

    void Start()
    {
        creditsPanel.SetActive(false);
        optionPanel.SetActive(false);
    }

    void Update()
    {
        volumeCounter.text = volumeNumber.ToString();
        sxfCounter.text = sxfNumber.ToString();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(newGameScene);
    }
 

    //quit button
    public void QuitGame()
    {
        Debug.Log("quit game");
        Application.Quit();
    }

    //setting and credits panel
    public void TurnOnCreditsPanel()
    {
        creditsPanel.SetActive(true);
        optionPanel.SetActive(false);
    }
    public void TurnOnOptionPanel()
    {
        creditsPanel.SetActive(false);
        optionPanel.SetActive(true);
    }
    //volume stuff
    public void SetVolume(float volume)
    {
        volumeNumber = volume;

    }

    //sxf stuff
    public void SetSxf(float sxf)
    {
        sxfNumber = sxf;

    }
}