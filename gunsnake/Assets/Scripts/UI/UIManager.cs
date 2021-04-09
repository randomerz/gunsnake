using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    public static bool stopPlayerInput;

    private bool isOpen = false;
    private bool isClosing = false; // for ui animation
    private bool canClose = true;
    public bool devEnabled;

    [Header("PlayerInfo")]

    public Image[] snakeWeapons = new Image[4];
    public Image[] storageWeapons = new Image[2];
    public ArtifactDisplay[] artifactDisplays = new ArtifactDisplay[22];

    public Image[] snakeWeaponsSelected = new Image[4];
    public Image[] storageWeaponsSelected = new Image[2];

    public TextMeshProUGUI healthText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI keysText;

    public TextMeshProUGUI descNameText;
    public Image descIcon;
    public TextMeshProUGUI descDescriptionText;

    public Image swapImage;
    public Image trashImage;

    [Header("Options")]
    public GameObject volumeSlider;
    public static float volumeNumber = 9;
    public TextMeshProUGUI volumeCounter;

    public GameObject sfxSlider;
    public static float sfxNumber = 9;
    public TextMeshProUGUI sxfCounter;

    [Header("Quit")]
    public string mainMenuSceneName = "Main Menu";
    public GameObject yesButton;


    [Header("Menu")]
    public TextMeshProUGUI areaText;
    public GameObject infoButton;
    public GameObject quitButton;
    public GameObject optionButton;

    [Header("Shop")]
    public GameObject[] shopButtons;
    private Image[] shopIcons;
    public TextMeshProUGUI shopCostText;

    [Header("Loot")]
    public GameObject[] lootButtons;
    private Image[] lootIcons;

    [Header("Win Lose")]
    public TextMeshProUGUI winText;
    public TextMeshProUGUI lossText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI scoreText;

    [Header("Panels")]
    public GameObject leftSidePanel;
    public GameObject rightSidePanel;
    public GameObject PlayerInfoPanel;
    public GameObject OptionPanel;
    public GameObject QuitPanel;
    public GameObject MenuPanel;
    public GameObject ShopPanel;
    public GameObject LootPanel;
    public GameObject DevPanel;
    public GameObject winLosePanel;

    [Header("Other References")]
    public ItemLootTable lootTable;
    private const int WEAPON_TABLE_ID = 0;
    private const int ARTIFACT_TABLE_ID = 1;

    public Sprite defaultIcon;
    public Sprite selectedIcon;
    public Sprite defaultSquare;
    public Sprite selectedSquare;
    public Sprite defaultSwap;
    public Sprite selectedSwap;
    public Sprite defaultTrash;
    public Sprite selectedTrash;

    public UIAnimationController animationController;


    // private
    private static Item[] shopItems = new Item[6];
    private static int[] shopPrices = new int[6];
    private static Item[] lootItems = new Item[3];
    private static Item currSelectedItem;

    // -1 means not selected
    private static int currShopSelected = -1;   // 0 - 5
    private static int currLootSelected = -1;   // 0 - 2
    private static int currInvSelected = -1;    // 0 - 3 are snake, 4, 5 are storage

    private static int snakeLength = 4;
    private static bool canSwapAndTrash;
    private static bool doSwap;
    private static bool doTrash;

    private static UIManager instance;

    private void Awake()
    {
        instance = this;

        shopIcons = new Image[shopButtons.Length];
        for (int i = 0; i < shopButtons.Length; i++)
        {
            shopIcons[i] = shopButtons[i].GetComponentsInChildren<Image>()[1]; // this is some hot garbo code
        }
        lootIcons = new Image[lootButtons.Length];
        for (int i = 0; i < lootButtons.Length; i++)
        {
            lootIcons[i] = lootButtons[i].GetComponentsInChildren<Image>()[1];
        }

        leftSidePanel.SetActive(true);
        rightSidePanel.SetActive(true);
    }

    void Start()
    {
        volumeSlider.GetComponent<Slider>().value = volumeNumber;
        sfxSlider.GetComponent<Slider>().value = sfxNumber;
        volumeCounter.text = volumeNumber.ToString();
        sxfCounter.text = sfxNumber.ToString();

        areaText.text = LevelHandler.currentArea.ToLower() + " - " + (LevelHandler.currentFloor + 1);

        UpdateDescription();

        SetShopLootItems();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isOpen)
            {
                if (!isClosing)
                    CloseUI();
                else
                    CloseUIFunctions();
            }
            else
                OpenUI();
        }
        CheckDevPanel();

        if (isOpen)
        {
            // PlayerUI stuff
            healthText.text = "" + Player.playerHealth.GetHealth() + " / " + Player.playerHealth.GetMaxHealth(); ;
            goldText.text = "" + PlayerInventory.gold;
            keysText.text = "" + PlayerInventory.keys;

            for (int i = 0; i < snakeWeapons.Length; i++)
            {
                if (PlayerInventory.GetWeapon(i) == null)
                    snakeWeapons[i].sprite = defaultIcon;
                else
                    snakeWeapons[i].sprite = PlayerInventory.GetWeapon(i).icon;
            }

            for (int i = 0; i < storageWeapons.Length; i++)
            {
                if (PlayerInventory.weaponStorage[i] == null)
                    storageWeapons[i].sprite = defaultIcon;
                else
                    storageWeapons[i].sprite = PlayerInventory.weaponStorage[i].icon;
            }

            UpdateArtifacts();
        }
    }



    private void ButtonClicked()
    {
        AudioManager.Play("ui_click");
    }

    private void DeselectAll()
    {
        currShopSelected = -1;
        currLootSelected = -1;
        currInvSelected = -1;
        currSelectedItem = null;

        RemoveSelectedSprites();
        swapImage.sprite = defaultSwap;
        trashImage.sprite = defaultTrash;

        UpdateDescription();
    }

    private void RemoveSelectedSprites()
    {
        for (int i = 0; i < snakeWeaponsSelected.Length; i++)
        {
            snakeWeaponsSelected[i].sprite = defaultIcon;
        }
        for (int i = 0; i < storageWeaponsSelected.Length; i++)
        {
            storageWeaponsSelected[i].sprite = defaultIcon;
        }
        for (int i = 0; i < artifactDisplays.Length; i++)
        {
            artifactDisplays[i].buttonImage.sprite = defaultIcon;
        }
        for (int i = 0; i < shopButtons.Length; i++)
        {
            shopButtons[i].GetComponent<Button>().image.sprite = defaultSquare;
        }
        for (int i = 0; i < lootButtons.Length; i++)
        {
            lootButtons[i].GetComponent<Button>().image.sprite = defaultSquare;
        }
    }

    public void UpdateDescription()
    {
        if (currSelectedItem == null)
        {
            descNameText.text = "-";
            descIcon.sprite = defaultIcon;
            descDescriptionText.text = "";
        }
        else
        {
            descNameText.text = currSelectedItem.name;
            descIcon.sprite = currSelectedItem.icon;
            descDescriptionText.text = currSelectedItem.description;
        }
    }

    public void OpenUI()
    {
        Time.timeScale = 0f;

        //leftSidePanel.SetActive(true);
        //rightSidePanel.SetActive(true);
        MenuPanel.SetActive(true);
        PlayerInfoPanel.SetActive(true);
        
        isOpen = true;

        animationController.SetVisible(true);
    }

    public void CloseUI()
    {
        if (!canClose)
            return;

        animationController.SetVisible(false);

        isClosing = true;
        //CloseUIFunctions();
    }

    public void CloseUIFunctions()
    {
        Time.timeScale = 1f;

        //leftSidePanel.SetActive(false);
        //rightSidePanel.SetActive(false);
        MenuPanel.SetActive(false);
        ShopPanel.SetActive(false);
        LootPanel.SetActive(false);

        PlayerInfoPanel.SetActive(false);
        OptionPanel.SetActive(false);
        QuitPanel.SetActive(false);

        isOpen = false;
        isClosing = false;
        canSwapAndTrash = false;
    }


    #region Player Info

    public void SelectFromInventory(int index)
    {
        if (currInvSelected == index)
        {
            ButtonClicked();

            DeselectAll();
            return;
        }
        if (doSwap)
        {
            if (DoSwap(index)) // succesfully swapped
            {
                ButtonClicked();

                DeselectAll();
            }
            else
            {
                UpdateInventorySelected();
            }
            return;
        }
        if (doTrash)
        {
            currInvSelected = index;
            if (DoTrash()) // succesfully swapped
            {
                ButtonClicked();

                DeselectAll();
            }
            else
            {
                UpdateInventorySelected();
            }
            return;
        }

        ButtonClicked();

        currInvSelected = index;
        UpdateInventorySelected();

        UpdateDescription();
    }

    private void UpdateInventorySelected()
    {
        if (currInvSelected == -1)
        {
            RemoveSelectedSprites();
            currSelectedItem = null;
        }
        else
        {
            RemoveSelectedSprites();

            if (currInvSelected < snakeLength)
            {
                currSelectedItem = PlayerInventory.GetWeapon(currInvSelected);
                snakeWeaponsSelected[currInvSelected].sprite = selectedIcon;
            }
            else
            {
                currSelectedItem = PlayerInventory.weaponStorage[currInvSelected - snakeLength];
                storageWeaponsSelected[currInvSelected - snakeLength].sprite = selectedIcon;
            }
        }
    }

    public void SetSwapStatus()
    {
        if (canSwapAndTrash)
        {
            ButtonClicked();

            doSwap = !doSwap;
            if (doSwap)
            {
                swapImage.sprite = selectedSwap;
            }
            else
            {
                swapImage.sprite = defaultSwap;
            }
            doTrash = false;
            trashImage.sprite = defaultTrash;
        }
    }

    public bool DoSwap(int swapTo)
    {
        doTrash = false;
        if (currInvSelected == -1 || swapTo == -1)
        {
            ButtonClicked();

            doSwap = true;
            currInvSelected = swapTo;
            return false;
        }
        else
        {
            // make sure currInvSelected < swapTo
            if (currInvSelected > swapTo)
            {
                int temp = currInvSelected;
                currInvSelected = swapTo;
                swapTo = temp;
            }
            if (currInvSelected < snakeLength && swapTo < snakeLength)
            {
                Item temp = PlayerInventory.GetWeapon(currInvSelected);
                PlayerInventory.SetWeapon(PlayerInventory.GetWeapon(swapTo), currInvSelected);
                PlayerInventory.SetWeapon(temp, swapTo);
            }
            else if (currInvSelected < snakeLength && snakeLength <= swapTo)
            {
                Item temp = PlayerInventory.GetWeapon(currInvSelected);
                PlayerInventory.SetWeapon(PlayerInventory.weaponStorage[swapTo - snakeLength], currInvSelected);
                PlayerInventory.weaponStorage[swapTo - snakeLength] = temp;
            }
            else
            { // snakeLength < currInvSelected && snakeLength < swapTo
                Item temp = PlayerInventory.weaponStorage[currInvSelected - snakeLength];
                PlayerInventory.weaponStorage[currInvSelected - snakeLength] = PlayerInventory.weaponStorage[swapTo - snakeLength];
                PlayerInventory.weaponStorage[swapTo - snakeLength] = temp;
            }

            doSwap = false;
            DeselectAll();
            swapImage.sprite = defaultSwap;
            // automatically updates inventory

            return true;
        }
    }

    public void SetTrashStatus()
    {
        if (canSwapAndTrash)
        {
            ButtonClicked();

            doTrash = !doTrash;
            if (doTrash)
            {
                trashImage.sprite = selectedTrash;
                DoTrash();
            }
            else
            {
                trashImage.sprite = defaultTrash;
            }
            doSwap = false;
            swapImage.sprite = defaultSwap;
        }
    }

    public bool DoTrash()
    {
        doSwap = false;

        if (currInvSelected == -1)
        {
            doTrash = true;
            return false;
        }
        else
        {
            if (currInvSelected < snakeLength)
            {
                PlayerInventory.SetWeapon(null, currInvSelected);
            }
            else
            {
                PlayerInventory.weaponStorage[currInvSelected - snakeLength] = null;
            }

            doTrash = false;
            DeselectAll();
            trashImage.sprite = defaultTrash;
            // automatically updates inventory
            return true;
        }
    }


    public void SelectArtifact(int i)
    {
        Item[] artifactList = PlayerInventory.GetArtifactList();
        if (i < artifactList.Length && artifactList[i] != null)
        {
            if (artifactList[i].count == 0)
                return;
            ButtonClicked();

            currSelectedItem = artifactList[i];

            RemoveSelectedSprites();
            artifactDisplays[i].buttonImage.sprite = selectedIcon;

            UpdateDescription();
        }
    }


    public void UpdateArtifacts()
    {
        Item[] artifactList = PlayerInventory.GetArtifactList();

        if (artifactList.Length > artifactDisplays.Length)
            Debug.LogError("More artifact in inventory than room to display!");
        for (int i = 0; i < artifactList.Length && i < artifactDisplays.Length; i++)
        {
            if (artifactList[i] == null)
            {
                artifactDisplays[i].UpdateDisplay(null, 0);
            }
            else
            {
                artifactDisplays[i].UpdateDisplay(artifactList[i].icon, artifactList[i].count);
            }
        }
    }

    #endregion


    #region Shop + Loot

    public static void OpenShop()
    {
        if (instance != null)
            instance.OpenShopHelper();
        else 
        {
            Debug.Log("Tried opening shop, but no UI Manager in current scene.");
        }
    }

    private void OpenShopHelper()
    {
        OpenUI();

        MenuPanel.SetActive(false);
        ShopPanel.SetActive(true);
        canSwapAndTrash = true;

        UpdateShopLootIcons();
        DeselectAll();
    }

    public void SelectFromShop(int index)
    {
        if (shopItems[index] == null)
        {
            return;
        }

        if (currShopSelected == index)
        {
            ButtonClicked();

            DeselectAll();
            shopCostText.text = "-";
            return;
        }

        ButtonClicked();

        currShopSelected = index;
        currSelectedItem = shopItems[index];

        RemoveSelectedSprites();
        shopButtons[index].GetComponent<Button>().image.sprite = selectedSquare;

        shopCostText.text = shopPrices[index].ToString();

        UpdateDescription();
    }

    public void BuySelectedFromShop()
    {
        if (currShopSelected == -1 || currSelectedItem == null)
        {
            return;
        }

        if (PlayerInventory.gold < shopPrices[currShopSelected])
        {
            return;
        }

        if (currSelectedItem.itemType == Item.ItemType.weapon)
        {
            if (PlayerInventory.IsStorageFull())
            {
                return;
            }
            PlayerInventory.AddWeapon(currSelectedItem);
        }
        else
        {
            PlayerInventory.AddArtifact(currSelectedItem);
            //Debug.Log("Bought " + currSelectedItem.name + ", but didn't add.");
        }

        AudioManager.Play("ui_buy_item");

        PlayerInventory.AddGold(-shopPrices[currShopSelected]);
        shopItems[currShopSelected] = null;
        DeselectAll();
        UpdateShopLootIcons();
    }

    private void SetShopLootItems()
    {
        // possible duplicates
        shopItems[0] = lootTable.GetEntry(WEAPON_TABLE_ID);
        shopItems[1] = lootTable.GetEntry(WEAPON_TABLE_ID);
        shopItems[2] = lootTable.GetEntry(WEAPON_TABLE_ID);
        shopItems[3] = lootTable.GetEntry(ARTIFACT_TABLE_ID);
        shopItems[4] = lootTable.GetEntry(ARTIFACT_TABLE_ID);
        shopItems[5] = lootTable.GetEntry(ARTIFACT_TABLE_ID);

        for (int i = 0; i < shopItems.Length; i++)
        {
            shopPrices[i] = CalculatePrice(shopItems[i]);
        }

        for (int i = 0; i < lootItems.Length; i++)
        {
            if (Random.Range(0, 2) == 0)
            {
                lootItems[i] = lootTable.GetEntry(WEAPON_TABLE_ID);
            }
            else
            {
                lootItems[i] = lootTable.GetEntry(ARTIFACT_TABLE_ID);
            }
        }
    }

    private int CalculatePrice(Item item)
    {
        int price = item.baseCost;
        float rand = Random.Range(0.9f, 1.1f);
        price = (int)(price * rand);
        price = Mathf.Max(price, 1);
        return price;
    }

    private void UpdateShopLootIcons()
    {
        for (int i = 0; i < shopItems.Length; i++)
        {
            if (shopItems[i] == null)
            {
                //shopItems[i] = emptyIcon;
                shopIcons[i].sprite = defaultIcon;
            }
            else
                shopIcons[i].sprite = shopItems[i].icon;
        }

        for (int i = 0; i < lootItems.Length; i++)
        {
            if (lootItems[i] == null)
            {
                //lootItems[i] = emptyIcon;
                lootIcons[i].sprite = defaultIcon;
            }
            else
                lootIcons[i].sprite = lootItems[i].icon;
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



    public static void OpenLoot()
    {
        if (instance != null)
            instance.OpenLootHelper();
        else
        {
            Debug.Log("Tried opening loot, but no UI Manager in current scene.");
        }
    }

    private void OpenLootHelper()
    {
        OpenUI();

        MenuPanel.SetActive(false);
        LootPanel.SetActive(true);
        canSwapAndTrash = true;

        AudioManager.Play("ui_open_loot");

        UpdateShopLootIcons();
        DeselectAll();
    }

    public void SelectFromLoot(int index)
    {
        if (lootItems[index] == null)
        {
            return;
        }

        if (currLootSelected == index)
        {
            ButtonClicked();

            DeselectAll();
            return;
        }

        ButtonClicked();

        currLootSelected = index;
        currSelectedItem = lootItems[index];

        RemoveSelectedSprites();
        lootButtons[index].GetComponent<Button>().image.sprite = selectedSquare;

        UpdateDescription();
    }

    public void ChooseSelectedFromLoot()
    {
        if (currLootSelected == -1 || currSelectedItem == null)
        {
            return;
        }

        if (currSelectedItem.itemType == Item.ItemType.weapon)
        {
            if (PlayerInventory.IsStorageFull())
            {
                return;
            }
            PlayerInventory.AddWeapon(currSelectedItem);
        }
        else
        {
            PlayerInventory.AddArtifact(currSelectedItem);
            //Debug.Log("Chose " + currSelectedItem.name + ", but didn't add.");
        }

        for (int i = 0; i < lootItems.Length; i++)
        {
            lootItems[i] = null;
        }
        DeselectAll();
        UpdateShopLootIcons();
    }



    #endregion


    #region Menu

    //switching between menus
    public void TurnOnQuitPanel()
    {
        PlayerInfoPanel.SetActive(false);
        OptionPanel.SetActive(false);
        QuitPanel.SetActive(true);
        winLosePanel.SetActive(false);
    }
    public void TurnOnInfoPanel()
    {
        PlayerInfoPanel.SetActive(true);
        OptionPanel.SetActive(false);
        QuitPanel.SetActive(false);
        winLosePanel.SetActive(false);
    }
    public void TurnOnOptionPanel()
    {
        PlayerInfoPanel.SetActive(false);
        OptionPanel.SetActive(true);
        QuitPanel.SetActive(false);
        winLosePanel.SetActive(false);
    }

    #endregion


    #region Quit

    public void QuitToMainMenu()
    {
        LevelHandler.SetToJungle();

        canClose = true;
        CloseUI();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    //quit button
    public void QuitGame()
    {
        ButtonClicked();

        Debug.Log("quit game");
        Application.Quit();
    }

    #endregion


    #region Options

    //volume stuff
    public void SetVolume(float volume)
    {
        ButtonClicked();

        volumeNumber = volume;
        volumeCounter.text = volumeNumber.ToString();
        AudioManager.SetMusicVolume(volume / volumeSlider.GetComponent<Slider>().maxValue);
    }

    //sxf stuff
    public void SetSxf(float volume)
    {
        ButtonClicked();

        sfxNumber = volume;
        sxfCounter.text = sfxNumber.ToString();
        AudioManager.SetSfxVolume(volume / volumeSlider.GetComponent<Slider>().maxValue);
    }

    #endregion


    #region Win Lose

    public static void EndGame(bool didWin, int time, int score)
    {
        instance?.EndGameHelper(didWin, time, score);
    }

    private void EndGameHelper(bool didWin, int time, int score)
    {
        OpenUI();

        Time.timeScale = 0f;

        PlayerInfoPanel.SetActive(false);
        winLosePanel.SetActive(true);

        canClose = false;

        winText.gameObject.SetActive(didWin);
        lossText.gameObject.SetActive(!didWin);

        int mins = time / 60;
        int secs = time % 60;
        timeText.text = mins + ":" + secs.ToString("00");

        scoreText.text = score.ToString();
    }

    #endregion


    #region Dev Tools

    [Header("Dev tools")]
    public Toggle invulnToggle;
    public Toggle damageToggle;

    private static bool devIsInvuln;
    private static bool devIsDamage;

    private void CheckDevPanel()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote) && devEnabled) // ~ key
        {
            if (DevPanel.activeSelf)
            {
                Time.timeScale = 1f;
                DevPanel.SetActive(false);

                invulnToggle.isOn = devIsInvuln;
                damageToggle.isOn = devIsDamage;
            }
            else
            {
                Time.timeScale = 0f;
                DevPanel.SetActive(true);
            }
        }
    }

    public void AddGold(int amount)
    {
        ButtonClicked();

        PlayerInventory.AddGold(amount);
    }

    public void AddKeys(int amount)
    {
        ButtonClicked();

        PlayerInventory.AddKey(amount);
    }

    public void SetInvuln(bool value)
    {
        ButtonClicked();

        devIsInvuln = value;

        Player.playerHealth.isInvulnerable = value;
    }

    public void SetInfDamage(bool value)
    {
        ButtonClicked();

        devIsDamage = value;

        if (value)
        {
            Projectile.bonusDamage += 999999;
        }
        else
        {
            Projectile.bonusDamage -= 999999;
        }
    }

    #endregion
}

