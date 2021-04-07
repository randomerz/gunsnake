using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSwapToggle : MonoBehaviour
{
    public Sprite buttonA;
    public Sprite buttonPressedA;
    public Sprite buttonB;
    public Sprite buttonPressedB;

    public bool isA;
    public Button button;
    private SpriteState spriteState;

    void Awake()
    {
        spriteState = button.spriteState;
        if (!isA)
        {
            button.image.sprite = buttonB;
            spriteState.pressedSprite = buttonPressedB;
            button.spriteState = spriteState;
        }
    }

    public void ToggleSprite()
    {
        isA = !isA;
        if (isA)
        {
            button.image.sprite = buttonA;
            spriteState.pressedSprite = buttonPressedA;
        }
        else
        {
            button.image.sprite = buttonB;
            spriteState.pressedSprite = buttonPressedB;
        }
        button.spriteState = spriteState;
    }
}
