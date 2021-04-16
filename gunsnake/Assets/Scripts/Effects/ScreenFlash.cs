using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ScreenFlash : MonoBehaviour
{
    private static ScreenFlash _instance;

    private static CanvasGroup myCanvasGroup;
    private const float baseAlpha = 0.75f;

    void Awake()
    {
        _instance = this;
        myCanvasGroup = GetComponent<CanvasGroup>();
    }

    public static void Flash(float duration)
    {
        _instance.StartCoroutine(DoFlash(duration));
    }

    private static IEnumerator DoFlash(float duration)
    {
        myCanvasGroup.alpha = baseAlpha;
        float time = 0;

        while (time < duration)
        {
            if (Time.timeScale == 0)
                break;

            time += Time.deltaTime;

            myCanvasGroup.alpha = baseAlpha * (1 - Mathf.Pow((time / duration), 2));

            yield return null;
        }

        myCanvasGroup.alpha = 0;
    }
}
