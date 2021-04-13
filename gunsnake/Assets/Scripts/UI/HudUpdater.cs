using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudUpdater : MonoBehaviour
{
    public List<HealthBarStuff> healthBars = new List<HealthBarStuff>();
    public GameObject singleHealthBar;
    public int healthBarCounter = 0;

    public int currentHealthnumber;
    public int maxHealthNumber;

    public int ydiff = 50;
    // coin stuff here
    public TextMeshProUGUI coinText;
    //
    public Image key;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        currentHealthnumber = Player.playerHealth.GetHealth();
        maxHealthNumber = Player.playerHealth.GetMaxHealth();

        coinText.text = PlayerInventory.GetGold().ToString();
        //make key appear
        if (PlayerInventory.HasKeys())
        {
            key.gameObject.SetActive(true);
        }
        else
        {
            key.gameObject.SetActive(false);
        }

        addOrDestoryBars();

        for(int i = 0; i < healthBarCounter; i++)
        {
            healthBars[i].healthBarUpdater(currentHealthnumber - (i)*5, maxHealthNumber - (i)*5);
        }

    }

    void addOrDestoryBars()
    {
        int testing_health = (maxHealthNumber - 1) / 5 + 1;
        if (testing_health > healthBarCounter)
        {
            GameObject g = Instantiate(singleHealthBar, transform);

            //g.transform.position -= new Vector3(0, healthBarCounter*ydiff, 0);
            g.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, healthBarCounter * ydiff);
            healthBars.Add(g.GetComponent<HealthBarStuff>());
            healthBarCounter++;
        }
        else if (testing_health < healthBarCounter)
        {
            healthBarCounter--;
            Debug.Log(" this is wack");
            Destroy(healthBars[healthBarCounter ].gameObject);

            Debug.Log(" about to destory" + healthBars[healthBarCounter].gameObject.name);
            healthBars.RemoveAt(healthBarCounter);
        }    
    }
}
