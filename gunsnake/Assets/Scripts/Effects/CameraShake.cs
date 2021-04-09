using UnityEngine;
using System.Collections;

// script from @ixikos

public class CameraShake : MonoBehaviour
{

    private Vector3 _originalPos;
    public static CameraShake _instance;

    private static float curDuration;

    void Awake()
    {
        _originalPos = transform.localPosition;

        _instance = this;
    }

    public static void Shake(float duration, float amount)
    {
        if (duration < curDuration)
            return;
        _instance.StopAllCoroutines();
        _instance.StartCoroutine(_instance.cShake(duration, amount));
    }

    public IEnumerator cShake(float duration, float amount)
    {
        float endTime = Time.time + duration;

        while (Time.time < endTime)
        {
            if (Time.timeScale == 0)
                break;

            transform.localPosition = _originalPos + Random.insideUnitSphere * amount;

            duration -= Time.deltaTime;
            curDuration = duration;

            yield return null;
        }

        transform.localPosition = _originalPos;
    }
}