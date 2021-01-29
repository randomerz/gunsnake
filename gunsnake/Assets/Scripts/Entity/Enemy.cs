using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class Enemy : Entity
{
    public int maxHealth = -1;
    protected int health;

    protected SpriteRenderer spriteRenderer;

    private bool strobing;
    private Material oldMat;
    public static Material whiteFlashMat; // set in GameHandler.cs
    [HideInInspector]

    protected override void Awake()
    {
        base.Awake();
        health = maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        oldMat = spriteRenderer.material;

        EnemyManager.AddEnemy(this);
    }

    public abstract void EnemyTick(int tick);

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
        else
        {
            StrobeWhite(1);
        }
    }
    
    public virtual void Die()
    {
        Debug.Log("I died!!");

        // temp
        //gameObject.SetActive(false);
        EnemyManager.RemoveEnemy(gameObject);
    }



    #region Strobe Color

    protected void StrobeColor(int _strobeCount, Color _toStrobe)
    {
        if (strobing)
            return;

        strobing = true;

        Color oldColor = spriteRenderer.color;

        StartCoroutine(StrobeColorHelper(0, ((_strobeCount * 2) - 1), oldColor, _toStrobe));
    }

    protected void StrobeAlpha(int _strobeCount, float a)
    {
        Color toStrobe = new Color(spriteRenderer.color.r, spriteRenderer.color.b, spriteRenderer.color.g, a);
        StrobeColor(_strobeCount, toStrobe);
    }

    protected void StrobeWhite(int _strobeCount)
    {
        StrobeColor(_strobeCount, Color.white);
    }

    private IEnumerator StrobeColorHelper(int _i, int _stopAt, Color _color, Color _toStrobe)
    {
        if (_i <= _stopAt)
        {
            if (_i % 2 == 0)
                spriteRenderer.material = whiteFlashMat;
            else
                spriteRenderer.material = oldMat;

            yield return new WaitForSeconds(0.125f);
            StartCoroutine(StrobeColorHelper((_i + 1), _stopAt, _color, _toStrobe));
        }
        else
        {
            strobing = false;
        }
    }

    #endregion
}
