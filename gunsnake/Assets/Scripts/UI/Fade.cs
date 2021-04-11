using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    public CanvasGroup myCanvasGroup;
    public bool mFaded = false;
    public float Duration = .04f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void FadeOut()
    {
        StartCoroutine(DoFade(myCanvasGroup, myCanvasGroup.alpha, mFaded ? 1 : 0));
        mFaded = !mFaded;

    }
    public IEnumerator DoFade(CanvasGroup CanvasGroup, float start, float end)
    {
        float counter = 0f;

        while (counter < Duration)
        {
            counter += Time.deltaTime;
            CanvasGroup.alpha = Mathf.Lerp(start, end, counter / Duration);

            yield return null;
        }
    }

    public void FadeIn()
    {
        StartCoroutine(DoFadeIn(myCanvasGroup, myCanvasGroup.alpha, mFaded ? 1 : 0));
        mFaded = !mFaded;

    }
    public IEnumerator DoFadeIn(CanvasGroup CanvasGroup, float start, float end)
    {
        float counter = 0f;

        while (counter < Duration)
        {
            counter += Time.deltaTime;
            CanvasGroup.alpha = 1 - Mathf.Lerp(start, end, counter / Duration);

            yield return null;
        }
    }
}
