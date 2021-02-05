using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject
{
    public new string name;
    public string description;

    public Sprite icon;

    public enum ItemType
    {
        weapon,
        artifact,
    }
    public ItemType itemType;

    public GameObject prefab; // i dont know if this is right

}
