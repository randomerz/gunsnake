using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudUpdater : MonoBehaviour
{

    public Sprite[] batterySpriteArray;

    //first row stuff
    public Image maxHealth;
    public Image currentHealth;
    public Image endPiece;

    //second row stuff
    public Image maxHealth1;
    public Image currentHealth1;
    public Image endPiece1;

    public int currentHealthnumber;
    public int maxHealthNumber;
    public bool over5max = true;
    public bool over5current = true;

   /* public Vector3 header10;
    public Vector3 header9;
    public Vector3 header8;
    public Vector3 header7;
    public Vector3 header6;
    public Vector3 header5;
    public Vector3 header4;
    public Vector3 header3;
    public Vector3 header2;
    public Vector3 header1;*/

    // coin stuff here
    public Text coinText;

    //
    public Image key;
    // Start is called before the first frame update
    void Start()
    {
        // starts at 5 max health, full health
        maxHealth.sprite = batterySpriteArray[0];
        currentHealth.sprite = batterySpriteArray[5];
        endPiece.sprite = batterySpriteArray[10];

        // stops showing the second level
        maxHealth1.gameObject.SetActive(false);
        currentHealth1.gameObject.SetActive(false);
        endPiece1.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        currentHealthnumber = Player.playerHealth.GetHealth();
        maxHealthNumber = Player.playerHealth.GetMaxHealth();

        coinText.text = PlayerInventory.gold.ToString();
        
        //make key appear
        if (PlayerInventory.HasKeys())
        {
            key.gameObject.SetActive(true);
        }
        else
        {
            key.gameObject.SetActive(false);
        }

        Currenthealthupdaterthing();
        Maxhealthupdaterthing();
        //Extrapiecemover();

    }

    void Currenthealthupdaterthing()
    {
        switch(currentHealthnumber)
        {
            case 0:
                currentHealth.sprite = batterySpriteArray[11];
                break;
            case 1:
                currentHealth.sprite = batterySpriteArray[9];
                currentHealth1.sprite = batterySpriteArray[11];
                break;
            case 2:
                currentHealth.sprite = batterySpriteArray[8];
                currentHealth1.sprite = batterySpriteArray[11];
                break;
            case 3:
                currentHealth.sprite = batterySpriteArray[7];
                currentHealth1.sprite = batterySpriteArray[11];
                break;
            case 4:
                currentHealth.sprite = batterySpriteArray[6];
                currentHealth1.sprite = batterySpriteArray[11];
                break;
            case 5:
                currentHealth.sprite = batterySpriteArray[5];
                currentHealth1.sprite = batterySpriteArray[11];
                break;
            case 6:
                currentHealth.sprite = batterySpriteArray[5];
                currentHealth1.sprite = batterySpriteArray[9];
                break;
            case 7:
                currentHealth.sprite = batterySpriteArray[5];
                currentHealth1.sprite = batterySpriteArray[8];
                break;
            case 8:
                currentHealth.sprite = batterySpriteArray[5];
                currentHealth1.sprite = batterySpriteArray[7];
                break;
            case 9:
                currentHealth.sprite = batterySpriteArray[5];
                currentHealth1.sprite = batterySpriteArray[6];
                break;
            case 10:
                currentHealth.sprite = batterySpriteArray[5];
                currentHealth1.sprite = batterySpriteArray[5];
                break;

        }
    }

    void Maxhealthupdaterthing()
    {
        // above 5 max health gen
        if (maxHealthNumber > 5)
        {
            if (over5max)
            {
                over5max = false;
                maxHealth1.gameObject.SetActive(true);
            }

        }
        // under 5 max
        else
        {
            if (!over5max)
            {
                over5max = true;
                maxHealth1.gameObject.SetActive(false);
            }
        }

        switch (maxHealthNumber)
        {
            case 0:
                maxHealth.sprite = batterySpriteArray[11];
                break;
            case 1:
                maxHealth.sprite = batterySpriteArray[4];
                maxHealth1.sprite = batterySpriteArray[11];
                break;
            case 2:
                maxHealth.sprite = batterySpriteArray[3];
                maxHealth1.sprite = batterySpriteArray[11];
                break;
            case 3:
                maxHealth.sprite = batterySpriteArray[2];
                maxHealth1.sprite = batterySpriteArray[11];
                break;
            case 4:
                maxHealth.sprite = batterySpriteArray[1];
                maxHealth1.sprite = batterySpriteArray[11];
                break;
            case 5:
                maxHealth.sprite = batterySpriteArray[0];
                maxHealth1.sprite = batterySpriteArray[11];
                break;
            case 6:
                maxHealth1.sprite = batterySpriteArray[4];
                maxHealth.sprite = batterySpriteArray[0];
                break;
            case 7:
                maxHealth1.sprite = batterySpriteArray[3];
                maxHealth.sprite = batterySpriteArray[0];
                break;
            case 8:
                maxHealth1.sprite = batterySpriteArray[2];
                maxHealth.sprite = batterySpriteArray[0];
                break;
            case 9:
                maxHealth1.sprite = batterySpriteArray[1];
                maxHealth.sprite = batterySpriteArray[0];
                break;
            case 10:
                maxHealth1.sprite = batterySpriteArray[0];
                maxHealth.sprite = batterySpriteArray[0];
                break;
        }
    }
    /*void Extrapiecemover()
    {
        // above 5 current health gen
        if (currentHealthnumber > 5)
        {
            if (over5current)
            {
                over5current = false;
                currentHealth.gameObject.SetActive(true);
                currentHealth1.gameObject.SetActive(true);
            }

        }
        // under 5 current
        else
        {
            if (!over5current)
            {
                over5current = true;
                currentHealth.gameObject.SetActive(false);
                currentHealth1.gameObject.SetActive(false);
            }
        }

        if (maxHealthNumber == currentHealthnumber)
        {
            switch (maxHealthNumber)
            {
                case 1:
                    endPiece.transform.position = header1;
                    endPiece.gameObject.SetActive(true);
                    break;
                case 2:
                    endPiece.transform.position = header2;
                    endPiece.gameObject.SetActive(true);
                    break;
                case 3:
                    endPiece.transform.position = header3;
                    endPiece.gameObject.SetActive(true);
                    break;
                case 4:
                    endPiece.transform.position = header4;
                    endPiece.gameObject.SetActive(true);
                    break;
                case 5:
                    endPiece.transform.position = header5;
                    endPiece.gameObject.SetActive(true);
                    break;
                case 6:
                    endPiece1.transform.position = header6;
                    endPiece1.gameObject.SetActive(true);
                    break;
                case 7:
                    endPiece1.transform.position = header7;
                    endPiece1.gameObject.SetActive(true);
                    break;
                case 8:
                    endPiece1.transform.position = header8;
                    endPiece1.gameObject.SetActive(true);
                    break;
                case 9:
                    endPiece1.transform.position = header9;
                    endPiece1.gameObject.SetActive(true);
                    break;
                case 10:
                    endPiece1.transform.position = header10;
                    endPiece1.gameObject.SetActive(true);
                    break;

            }
        }
        else
        {
            endPiece.gameObject.SetActive(false);
            endPiece1.gameObject.SetActive(false);
        }
        if (maxHealthNumber > 5)
        {
            endPiece.transform.position = header5;
            endPiece.gameObject.SetActive(true);
        }




        //.transform.position
    }*/
}
