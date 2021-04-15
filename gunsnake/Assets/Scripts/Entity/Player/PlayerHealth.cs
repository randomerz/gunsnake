using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private int health;
    private static int maxHealth;
    private static int baseMaxHealth = 5;

    private static float dodgeChance = 0f;
    public bool isInvulnerable;
    private int ticksUntilCanTakeDamage;
    public int iFramesTicks = 8;

    [HideInInspector]
    public bool doesTakeDoubleDamage = false;

    public TextMeshProUGUI healthText;

    private bool strobing;
    //public GameObject[] spriteRenderers;

    void Start()
    {
        isInvulnerable = false;
        doesTakeDoubleDamage = false;
        health = maxHealth;
    }


    public void OnTick(int tick)
    {
        UpdateHUD();
        if (isInvulnerable)
        {
            ticksUntilCanTakeDamage -= 1;
            if (ticksUntilCanTakeDamage <= 0)
                isInvulnerable = false;
        }
    }


    private void UpdateHUD()
    {
        if (healthText != null)
            healthText.text = health.ToString();
    }

    public void GainHealth(int amount)
    {
        AudioManager.Play("player_heal");

        health = Mathf.Min(health + amount, maxHealth);
        //health += amount;
        UpdateHUD();
    }

    public void TakeDamage(int amount)
    {
        if (!isInvulnerable)
        {
            if(Random.Range(0f, 1f) <= dodgeChance)
            {
                //Add sound
                return;
            }
            AudioManager.Play("player_take_damage");// + Random.Range(1, 3));

            if (doesTakeDoubleDamage)
                health -= 2 * amount;
            else
                health -= amount;

            UpdateHUD();

            if (health <= 0)
            {
                Die();
            }

            SetInvulnerable(iFramesTicks);
            CameraShake.Shake(0.5f, 1f);
        }
    }

    public void UpdateDodge(int c)
    {
        dodgeChance = 1 - (1 / (.15f *c + 1));
    }

    public void Die()
    {
        AudioManager.Play("player_die");// + Random.Range(1, 3));

        Debug.Log("Player died! Health: " + health + "/" + maxHealth);

        health = 0;

        SetInvulnerable(3);
        Player.playerEffects.StartPlayerDieEffect(3);
        StartCoroutine(FinnaDie(3));
    }

    private IEnumerator FinnaDie(float seconds)
    {
        PlayerMovement.canMove = false;
        PlayerWeaponManager.canFire = false;

        yield return new WaitForSeconds(seconds);
        Player.EndGame(false);

        PlayerMovement.canMove = true;
        PlayerWeaponManager.canFire = true;
    }

    public int GetHealth()
    {
        return health;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public static bool IsMaxHealth()
    {
        return Player.playerHealth.GetHealth() == Player.playerHealth.GetMaxHealth();
    }

    public void SetInvulnerable(int frames)
    {
        isInvulnerable = true;
        ticksUntilCanTakeDamage = frames;
        StrobeAlpha(frames / 4, 0.5f);
    }

    public void ChangeMaxHealth()
    {
        maxHealth++;
        GainHealth(1);
    }

    #region Strobe Color

    // each _strobeCount lasts for 0.25 seconds (4 ticks)
    public void StrobeAlpha(int _strobeCount, float a)
    {
        if (strobing)
            return;

        strobing = true;

        Color _toStrobe = new Color(1, 1, 1, a);
        Color oldColor = Color.white;//spriteRenderers[0].color;
        StartCoroutine(StrobeAlphaHelper(0, ((_strobeCount * 2) - 1), oldColor, _toStrobe));
    }

    private IEnumerator StrobeAlphaHelper(int _i, int _stopAt, Color _color, Color _toStrobe)
    {
        if (_i <= _stopAt)
        {
            for (int j = 0; j < Player.sprites.Length; j++)
            {
                if (_i % 2 == 0)
                    Player.sprites[j].color = _toStrobe;
                else
                    Player.sprites[j].color = _color;
            }

            yield return new WaitForSeconds(0.25f);
            StartCoroutine(StrobeAlphaHelper((_i + 1), _stopAt, _color, _toStrobe));
        }
        else
        {
            strobing = false;
        }
    }

    #endregion

    public void ResetValuesToDefault()
    {
        maxHealth = baseMaxHealth;
        dodgeChance = 0f;

        //spriteRenderers = new SpriteRenderer[Player.sprites.Length];
        //for (int i = 0; i < Player.sprites.Length; i++)
        //    spriteRenderers[i] = Player.sprites[i];
    }
}
