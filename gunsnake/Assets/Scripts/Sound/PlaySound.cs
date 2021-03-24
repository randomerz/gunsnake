using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public string soundName;
    public bool playOnStart;

    void Start()
    {
        if (playOnStart)
            Play();
    }

    public void Play()
    {
        AudioManager.Play(soundName);
    }
}
