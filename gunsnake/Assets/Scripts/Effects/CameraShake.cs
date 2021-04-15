using UnityEngine;
using System.Collections;
using Cinemachine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraShake : MonoBehaviour
{

    private CinemachineVirtualCamera vCam;
    public static CameraShake _instance;

    private static float startingIntensity;
    private static float curIntensity;

    void Awake()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();

        _instance = this;
    }

    public static void Shake(float duration, float amount)
    {
        if (amount < startingIntensity)
            return;
        _instance.StopAllCoroutines();
        _instance.StartCoroutine(_instance.cShake(duration, amount));
    }

    public IEnumerator cShake(float duration, float amount)
    {
        float curTime = 0;

        CinemachineBasicMultiChannelPerlin perlin = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        perlin.m_AmplitudeGain = amount;

        while (curTime <= duration)
        {
            if (Time.timeScale == 0)
                break;

            //transform.localPosition = _originalPos + Random.insideUnitSphere * amount;

            curIntensity = Mathf.Lerp(amount, 0, curTime / duration);
            perlin.m_AmplitudeGain = curIntensity;

            curTime += Time.deltaTime;

            yield return null;
        }

        perlin.m_AmplitudeGain = 0;
    }
}