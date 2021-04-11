using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class HealthBarStuff : MonoBehaviour
{
    public Image maxHealthImage;
    public Image currentHealth;
    public Sprite[] healthSpriteArray;
    public Sprite[] emptySpriteArray;
    public Sprite[] purpleSpriteArray;
    public Sprite empty;




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void healthBarUpdater(int currentHealthNumber, int MaxhealthNumber)
    {
        if (currentHealthNumber >= 5)
        {
            currentHealth.sprite = healthSpriteArray[4];
        }
        else if (currentHealthNumber <= 0)
        {
            currentHealth.sprite = empty;
        }
        else
        {
            switch (currentHealthNumber)
            {
            case 1:
                    currentHealth.sprite = healthSpriteArray[0];
                break;
            case 2:
                    currentHealth.sprite = healthSpriteArray[1];
                break;
            case 3:
                    currentHealth.sprite = healthSpriteArray[2];
                break;
            case 4:
                    currentHealth.sprite = healthSpriteArray[3];
                break;
            
            }
        }
            if (MaxhealthNumber >= 5)
            {
                maxHealthImage.sprite = emptySpriteArray[4];
            }
            else
            {
                switch (MaxhealthNumber)
                {
                    case 0:
                        maxHealthImage.sprite = empty;
                        break;
                    case 1:
                        maxHealthImage.sprite = emptySpriteArray[0];
                        break;
                    case 2:
                        maxHealthImage.sprite = emptySpriteArray[1];
                        break;
                    case 3:
                        maxHealthImage.sprite = emptySpriteArray[2];
                        break;
                    case 4:
                        maxHealthImage.sprite = emptySpriteArray[3];
                        break;
                }
            }
    }
}
