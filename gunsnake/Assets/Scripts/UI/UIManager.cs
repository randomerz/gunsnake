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
    public bool devEnabled;

    [Header("PlayerInfo")]

    public TextMeshProUGUI[] snakeWeapons = new TextMeshProUGUI[4];
    public TextMeshProUGUI[] storageWeapons = new TextMeshProUGUI[2];
    public ArtifactDisplay[] artifactDisplays = new ArtifactDisplay[24];

    public TextMeshProUGUI healthText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI keysText;

    public TextMeshProUGUI descNameText;
    public Image descIcon;
    public TextMeshProUGUI descDescriptionText;

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

    [Header("Panels")]
    public GameObject PlayerInfoPanel;
    public GameObject OptionPanel;
    public GameObject QuitPanel;
    public GameObject MenuPanel;
    public GameObject ShopPanel;
    public GameObject LootPanel;
    public GameObject DevPanel;

    [Header("Other References")]
    public ItemLootTable lootTable;
    private const int WEAPON_TABLE_ID = 0;
    private const int ARTIFACT_TABLE_ID = 1;

    public Sprite defaultIcon;

    // private
    private static Item[] shopItems = new Item[6];
    private static int[] shopPrices = new int[6];
    private static Item[] lootItems = new Item[3];
    private static Item currSelectedItem;

    // -1 means not selected
    private static int currShopSelected = -1;   // 0 - 5
    private static int currLootSelected = -1;   // 0 - 2
    [SerializeField]
    private int currInvSelected = -1;    // 0 - 3 are snake, 4, 5 are storage

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
                CloseUI();
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
                    snakeWeapons[i].text = "-";
                else
                    snakeWeapons[i].text = PlayerInventory.GetWeapon(i).name;
            }

            for (int i = 0; i < storageWeapons.Length; i++)
            {
                if (PlayerInventory.weaponStorage[i] == null)
                    storageWeapons[i].text = "-";
                else
                    storageWeapons[i].text = PlayerInventory.weaponStorage[i].name;
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
        UpdateDescription();
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
        MenuPanel.SetActive(true);
        PlayerInfoPanel.SetActive(true);
        isOpen = true;
    }

    public void CloseUI()
    {
        Time.timeScale = 1f;

        MenuPanel.SetActive(false);
        ShopPanel.SetActive(false);
        LootPanel.SetActive(false);

        PlayerInfoPanel.SetActive(false);
        OptionPanel.SetActive(false);
        QuitPanel.SetActive(false);

        isOpen = false;
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
            currSelectedItem = null;
        else
        {
            if (currInvSelected < snakeLength)
            {
                currSelectedItem = PlayerInventory.GetWeapon(currInvSelected);
            }
            else
            {
                currSelectedItem = PlayerInventory.weaponStorage[currInvSelected - snakeLength];
            }
        }
    }

    public void SetSwapStatus()
    {
        if (canSwapAndTrash)
        {
            ButtonClicked();

            doSwap = !doSwap;
            doTrash = false;
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
            DoTrash();
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
            // automatically updates inventory
            return true;
        }
    }


    public void SelectArtifact(int i)
    {

        Item[] artifactList = PlayerInventory.GetArtifactList();
        if (artifactList[i] != null)
        {
            ButtonClicked();

            currSelectedItem = artifactList[i];
            UpdateDescription();
        }
    }


    public void UpdateArtifacts()
    {
        Item[] artifactList = PlayerInventory.GetArtifactList();

        for (int i = 0; i < artifactDisplays.Length; i++)
        {
            artifactDisplays[i].UpdateDisplay(artifactList[i].icon, artifactList[i].count);
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
        Time.timeScale = 0f;
        ShopPanel.SetActive(true);
        PlayerInfoPanel.SetActive(true);
        isOpen = true;
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
        Time.timeScale = 0f;
        LootPanel.SetActive(true);
        PlayerInfoPanel.SetActive(true);
        isOpen = true;
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
            //PlayerInventory.AddArtifact(currSelectedItem);
            Debug.Log("Chose " + currSelectedItem.name + ", but didn't add.");
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

    #endregion


    #region Quit

    public void QuitToMainMenu()
    {
        LevelHandler.SetToJungle();

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


    #region Dev Tools

    private void CheckDevPanel()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote) && devEnabled) // ~ key
        {
            if (DevPanel.activeSelf)
            {
                Time.timeScale = 1f;
                DevPanel.SetActive(false);
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

        Player.playerHealth.isInvulnerable = value;
    }

    public void SetInfDamage(bool value)
    {
        ButtonClicked();

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

