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

    [Header("Main UI")]
    public UIAnimationController mainAnimator;

    [Header("Options")]
    public GameObject volumeSlider;
    public float volumeNumber;
    public TextMeshProUGUI volumeCounter;

    public GameObject sfxSlider;
    public float sfxNumber;
    public TextMeshProUGUI sfxCounter;

    [Header("Panels")]
    public GameObject optionPanel;
    public GameObject creditsPanel;
    public GameObject resumePanel;

    [Header("fade")]
    public float Duration = .04f;
    public Fade f;

    void Start()
    {
        creditsPanel.SetActive(false);
        optionPanel.SetActive(false);
        resumePanel.SetActive(false);
        mainAnimator.SetVisible(true); // do after fade into main screen

        volumeNumber = UIManager.volumeNumber;
        sfxNumber = UIManager.sfxNumber;

        volumeSlider.GetComponent<Slider>().value = volumeNumber;
        sfxSlider.GetComponent<Slider>().value = sfxNumber;

        volumeCounter.text = volumeNumber.ToString();
        sfxCounter.text = sfxNumber.ToString();


        Time.timeScale = 1;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePanels();
        }
    }

    public void ButtonClicked()
    {
        AudioManager.Play("ui_click");
    }

    public void StartGame()
    {
        f.FadeOut();
        StartCoroutine(StartingGame());
    }
 

    //quit button
    public void QuitGame()
    {
        ButtonClicked();

        Debug.Log("quit game");
        Application.Quit();
    }

    //setting and credits panel
    public void TurnOnCreditsPanel()
    {
        ButtonClicked();

        creditsPanel.SetActive(true);
        optionPanel.SetActive(false);
        resumePanel.SetActive(true);
    }

    public void TurnOnOptionPanel()
    {
        ButtonClicked();

        creditsPanel.SetActive(false);
        optionPanel.SetActive(true);
        resumePanel.SetActive(true);
    }

    public void ClosePanels()
    {
        ButtonClicked();

        creditsPanel.SetActive(false);
        optionPanel.SetActive(false);
        resumePanel.SetActive(false);
    }

    //volume stuff
    public void SetVolume(float volume)
    {
        ButtonClicked();

        volumeNumber = volume;
        UIManager.volumeNumber = volume;
        volumeCounter.text = volumeNumber.ToString();
        AudioManager.SetMusicVolume(volume / volumeSlider.GetComponent<Slider>().maxValue);
    }

    //sxf stuff
    public void SetSfx(float volume)
    {
        ButtonClicked();

        sfxNumber = volume;
        UIManager.sfxNumber = volume;
        sfxCounter.text = sfxNumber.ToString();
        AudioManager.SetSfxVolume(volume / volumeSlider.GetComponent<Slider>().maxValue);
    }

    //fading stuff
    public IEnumerator StartingGame()
    {
        yield return new WaitForSeconds(Duration);
        LevelHandler.SetToJungle();
        LevelHandler.RestartGame();
    }
}