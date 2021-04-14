using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterTile : MonoBehaviour
{
    public Animator teleporterAnimator;

    public void SetTeleporterAnimation(bool value)
    {
        teleporterAnimator.SetBool("isOn", value);
    }

    public void SetTeleporterAnimation(bool value, float seconds)
    {
        StartCoroutine(SetTeleporterAnimationHelper(value, seconds));
    }

    public IEnumerator SetTeleporterAnimationHelper(bool value, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        teleporterAnimator.SetBool("isOn", value);
    }
}
