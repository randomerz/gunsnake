using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(RectTransform))]
public class ButtonPressEffect : MonoBehaviour
{
    public GameObject target;

    private Vector3 orig;
    private RectTransform rectTransform;

    private void Awake()
    {
        if (target == null)
            rectTransform = GetComponentsInChildren<RectTransform>()[1];
        else
            rectTransform = target.GetComponent<RectTransform>();
        orig = rectTransform.anchoredPosition;
    }

    public void _MoveDown()
    {
        rectTransform.anchoredPosition = orig + new Vector3(1, -1);
        //transform.position += new Vector3(1, -1);
    }

    public void _SetOrig()
    {
        rectTransform.anchoredPosition = orig;
    }
}
