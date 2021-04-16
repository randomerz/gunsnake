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

    [Header("Play")]
    public Toggle hardcoreToggle;
    public Toggle devToggle;
    public TMP_InputField seedInputField;
    public TextMeshProUGUI seedInput;

    [Header("Options")]
    public GameObject volumeSlider;
    public float volumeNumber;
    public TextMeshProUGUI volumeCounter;

    public GameObject sfxSlider;
    public float sfxNumber;
    public TextMeshProUGUI sfxCounter;

    [Header("Panels")]
    public GameObject playPanel;
    public GameObject optionPanel;
    public GameObject creditsPanel;
    public GameObject resumePanel;

    [Header("Fade")]
    public float Duration = .04f;
    public Fade f;

    void Start()
    {
        f.FadeIn();
        StartCoroutine(OpenMenu());

        AudioManager.PlayMusic("music_main_menu");

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
        if (seedInput.text != null)
        {
            DungeonGenerator.doSeed = true;
            DungeonGenerator.seedToSet = seedInput.text.GetHashCode();
            Debug.Log("Seed: " + seedInput.text.GetHashCode());
        }
        else
        {
            DungeonGenerator.doSeed = false;
        }

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
    public void TurnOnPlayPanel()
    {
        ButtonClicked();

        hardcoreToggle.isOn = PlayerHealth.isHardcore;
        devToggle.isOn = UIManager.devEnabled;
        seedInputField.text = "";

        playPanel.SetActive(true);
        creditsPanel.SetActive(false);
        optionPanel.SetActive(false);
        resumePanel.SetActive(true);
    }

    public void TurnOnCreditsPanel()
    {
        ButtonClicked();

        playPanel.SetActive(false);
        creditsPanel.SetActive(true);
        optionPanel.SetActive(false);
        resumePanel.SetActive(true);
    }

    public void TurnOnOptionPanel()
    {
        ButtonClicked();

        playPanel.SetActive(false);
        creditsPanel.SetActive(false);
        optionPanel.SetActive(true);
        resumePanel.SetActive(true);
    }

    public void ClosePanels()
    {
        ButtonClicked();

        playPanel.SetActive(false);
        creditsPanel.SetActive(false);
        optionPanel.SetActive(false);
        resumePanel.SetActive(false);
    }

    // play stuff

    public void SetHardcore(bool value)
    {
        PlayerHealth.isHardcore = value;
    }

    public void SetCheats(bool value)
    {
        UIManager.devEnabled = value;
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
    public IEnumerator OpenMenu()
    {
        yield return new WaitForSeconds(Duration);
        creditsPanel.SetActive(false);
        optionPanel.SetActive(false);
        resumePanel.SetActive(false);
        mainAnimator.SetVisible(true); // do after fade into main screen
    }

    //fading stuff
    public IEnumerator StartingGame()
    {
        yield return new WaitForSeconds(Duration);
        LevelHandler.SetToJungle();
        LevelHandler.RestartGame();
    }
}