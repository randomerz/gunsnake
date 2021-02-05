using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LazyShopUI : MonoBehaviour
{
    public TextMeshProUGUI[] snakeWeapons = new TextMeshProUGUI[4];
    public TextMeshProUGUI[] storageWeapons = new TextMeshProUGUI[2];

    public TextMeshProUGUI healthText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI keysText;

    public GameObject ShopPanel;
    public GameObject PlayerInfoPanel;
    public bool isOpen = false;

    void Start()
    {
        PlayerInventory.AddGold(100);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isOpen)
                CloseShop();
            else
                OpenShop();
        }

        if (isOpen)
        {
            healthText.text = "" + Player.playerHealth.GetHealth();
            goldText.text = "" + PlayerInventory.gold;
            keysText.text = "" + PlayerInventory.keys;

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



    public void OpenShop()
    {
        ShopPanel.SetActive(true);
        PlayerInfoPanel.SetActive(true);
        isOpen = true;
    }

    public void CloseShop()
    {
        ShopPanel.SetActive(false);
        PlayerInfoPanel.SetActive(false);
        isOpen = false;
    }
}
