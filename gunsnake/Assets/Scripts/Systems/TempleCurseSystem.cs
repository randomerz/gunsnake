using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class TempleCurseSystem : MonoBehaviour
{
    [Tooltip("16 ticks = 1 second here")]
    public int curseMax;
    public int curseEffectPoint;
    public int amountRemoveOnKill;
    private static int curseTicks;

    public PostProcessVolume curseProcess;
    public CanvasGroup vignetteGroup;
    public Image impulseImage;
    public Animator impulseAnimator;

    public HudUpdater HUD;

    private static bool isCursed;

    private static TempleCurseSystem instance;
    public static bool isEnabled;

    private void Awake()
    {
        instance = this;
        isEnabled = true;

        curseTicks = 0;
        
        TimeTickSystem.OnTick_Dungeon += TimeTickSystem_OnTick;
    }

    private void TimeTickSystem_OnTick(object sender, TimeTickSystem.OnTickEventArgs e)
    {
        //Debug.Log("Curse: " + curseTicks);

        curseTicks = Mathf.Min(curseTicks + 1, curseMax);

        if (!isCursed)
        {
            CheckCursed();
        }

        UpdatePostProcessing((float)curseTicks / curseMax);
    }

    private static void CheckCursed()
    {
        if (curseTicks >= instance.curseEffectPoint)
        {
            SetIsCursed(true);
        }
        else
        {
            SetIsCursed(false);
        }
    }

    private static void SetIsCursed(bool value)
    {
        isCursed = value;

        HudUpdater.SetHealthBarPurple(value);

        Player.playerHealth.doesTakeDoubleDamage = value;

        if (value)
        {
            AudioManager.Play("dungeon_start_curse");

            Sprite ss = GetScreenshot();
            instance.impulseImage.sprite = ss;
            instance.impulseAnimator.SetTrigger("doImpulse");
        }
        else
        {
            AudioManager.Play("dungeon_end_curse");
        }
    }

    public static void GetKill()
    {
        instance.GetKillHelper();
    }

    private void GetKillHelper()
    {
        curseTicks = Mathf.Clamp(curseTicks - amountRemoveOnKill, 0, curseMax);
        //Debug.Log("Got kill! curse is now " + curseTicks);

        CheckCursed();
        UpdatePostProcessing((float)curseTicks / curseMax);
    }

    public static void Reset()
    {
        if (instance != null)
            instance.ResetHelper();
    }

    private void ResetHelper()
    {
        curseTicks = 0;
        SetIsCursed(false);
        UpdatePostProcessing(0);
    }

    private static void UpdatePostProcessing(float percent)
    {
        if (isCursed)
        {
            // spawn effect
            instance.vignetteGroup.alpha = percent;
            instance.curseProcess.weight = percent;
        }
        else
        {
            instance.vignetteGroup.alpha = percent * 0.8f;
            instance.curseProcess.weight = percent * 0.8f;
        }
    }

    private static Sprite GetScreenshot()
    {
        Camera mainCam = Camera.main;
        int photoWidth = 256;
        int photoHeight = 144;

        RenderTexture rt = new RenderTexture(photoWidth, photoHeight, 24);
        mainCam.targetTexture = rt;
        RenderTexture.active = rt;
        mainCam.Render();
        Texture2D screenShot = new Texture2D(photoWidth, photoHeight, TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(0, 0, photoWidth, photoHeight), 0, 0);
        screenShot.Apply();
        mainCam.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        Destroy(rt);

        //GameObject flyToHud = GameObject.Instantiate(photoToHudAnimation);
        Sprite sprite = Sprite.Create(screenShot, new Rect(0, 0, photoWidth, photoHeight), new Vector2(0, 0));

        //Camera.main.targetTexture = null;

        return sprite;
    }
}
