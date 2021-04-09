using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimationController : MonoBehaviour
{
    public Animator animator;

    public bool isVisible;

    void Awake()
    {
        if (isVisible) // default false, if need to change
        {
            animator.SetBool("isVisible", isVisible);
            animator.SetTrigger("changeImmediate");
        }
    }

    public void SetVisible(bool value)
    {
        isVisible = value;
        animator.SetBool("isVisible", isVisible);
    }

    public void ToggleVisible()
    {
        SetVisible(!isVisible);
    }
}
