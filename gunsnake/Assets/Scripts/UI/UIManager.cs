using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    public static bool stopPlayerInput;

    
    public TextMeshProUGUI[] snakeWeapons = new TextMeshProUGUI[4];
    public TextMeshProUGUI[] storageWeapons = new TextMeshProUGUI[2];

    public TextMeshProUGUI healthText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI keysText;
    public bool isOpen = false;

    [Header("Options")]

    public GameObject OptionPanel;
    public GameObject QuitPanel;
    public GameObject PlayerInfoPanel;
    public GameObject MenuPanel;
    

    [Header("Menu")]
    public GameObject infoButton;
    public GameObject quitButton;
    public GameObject optionButton;

    [Header("Volume + Sxf")]
    public GameObject volumeSlider;
    public float volumeNumber;
    public TextMeshProUGUI volumeCounter;

    public GameObject sxfSlider;
    public float sxfNumber;
    public TextMeshProUGUI sxfCounter;

    [Header("quit")]
    public GameObject yesButton;



    void Start()
    {
        PlayerInventory.AddGold(100);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isOpen)
                CloseUI();
            else
                OpenUI();
        }

        if (isOpen)
        {
            healthText.text = "" + Player.playerHealth.GetHealth() + " / " + Player.playerHealth.GetmaxHealth(); ;
            goldText.text = "" + PlayerInventory.gold;
            keysText.text = "" + PlayerInventory.keys;
            volumeCounter.text = volumeNumber.ToString();
            sxfCounter.text = sxfNumber.ToString();
            for (int i = 0; i < snakeWeapons.Length; i++)
            {
                if (Player.playerWeaponManager.GetWeapon(i) == null)
                    snakeWeapons[i].text = "-";
                else
                    snakeWeapons[i].text = Player.playerWeaponManager.GetWeapon(i).name;
            }
            for (int i = 0; i < storageWeapons.Length; i++)
            {
                if (PlayerInventory.weaponStorage[i] == null)
                    storageWeapons[i].text = "-";
                else
                    storageWeapons[i].text = PlayerInventory.weaponStorage[i].name;
            }
        }
    }


    public void Heal()
    {
        if (PlayerInventory.gold >= 10)
        {
            PlayerInventory.AddGold(-10);
            Player.playerHealth.GainHealth(1);
        }
    }


    public void MoveEquippedToStorage(int equipIndex, int storageIndex)
    {
        PlayerInventory.MoveEquippedToStorage(equipIndex, storageIndex);
    }

    public void MoveStorageToEquipped(int storageIndex, int equipIndex)
    {
        PlayerInventory.MoveStorageToEquipped(storageIndex, equipIndex);
    }

    public void MoveEquippedToOpenStorage(int equipIndex)
    {
        if (PlayerInventory.IsStorageFull())
            return;
        for (int i = 0; i < PlayerInventory.weaponStorage.Length; i++)
        {
            if (PlayerInventory.weaponStorage[i] == null)
            {
                MoveEquippedToStorage(equipIndex, i);
            }
        }
    }

    public void MoveStorageToOpenEquipped(int storageIndex)
    {
        for (int i = 0; i < snakeWeapons.Length; i++)
        {
            if (Player.playerWeaponManager.GetWeapon(i) == null)
            {
                MoveStorageToEquipped(storageIndex, i);
            }
        }
    }



    public void OpenUI()
    {
        Time.timeScale = 0f;
        MenuPanel.SetActive(true);
        PlayerInfoPanel.SetActive(true);
        isOpen = true;
    }

    public void CloseUI()
    {
        Time.timeScale = 1f;
        MenuPanel.SetActive(false);
        PlayerInfoPanel.SetActive(false);
        OptionPanel.SetActive(false);
        QuitPanel.SetActive(false);
        isOpen = false;
    }

    //switching between menus
    public void TurnOnQuitPanel()
    {
        PlayerInfoPanel.SetActive(false);
        OptionPanel.SetActive(false);
        QuitPanel.SetActive(true);
    }
    public void TurnOnInfoPanel()
    {
        PlayerInfoPanel.SetActive(true);
        OptionPanel.SetActive(false);
        QuitPanel.SetActive(false);
    }
    public void TurnOnOptionPanel()
    {
        PlayerInfoPanel.SetActive(false);
        OptionPanel.SetActive(true);
        QuitPanel.SetActive(false);
    }

    //quit button
    public void QuitGame()
    {
        Debug.Log("quit game");
        Application.Quit();
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

