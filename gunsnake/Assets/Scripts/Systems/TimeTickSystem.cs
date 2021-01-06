using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// from Code Monkey https://youtu.be/FNFJ_R9zqXI

public class TimeTickSystem
{
    public class OnTickEventArgs : EventArgs
    {
        public int tick;
    }

    public static event EventHandler<OnTickEventArgs> OnTick;
    public static event EventHandler<OnTickEventArgs> OnTick_4;

    private const float TICK_TIMER_MAX = 0.0625f;

    private static GameObject timeTickSystemGameObject;
    private static int tick;

    public static void Create()
    {
        if (timeTickSystemGameObject == null)
        {
            timeTickSystemGameObject = new GameObject("TimeTickSystem");
            timeTickSystemGameObject.AddComponent<TimeTickSystemObject>();
        }
    }

    public static int GetTick()
    {
        return tick;
    }


    private class TimeTickSystemObject : MonoBehaviour
    {
        private float tickTimer;

        private void Awake()
        {
            tick = 0;
        }

        private void Update()
        {
            tickTimer += Time.deltaTime;
            if (tickTimer >= TICK_TIMER_MAX)
            {
                tickTimer -= TICK_TIMER_MAX;
                tick++;
                if (OnTick != null) OnTick(this, new OnTickEventArgs { tick = tick });

                if (tick % 4 == 0)
                    if (OnTick_4 != null) OnTick_4(this, new OnTickEventArgs { tick = tick });

            }
        }
    }
}
