using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    //rarity is chance to drop
    public Sprite groundSprite;
    public Sprite slotSprite;
    public string itemType;
    public float rarity;

    public void SetAllVariables(Sprite theGroundSprite, Sprite theSlotSprite, string theItemType, float theRarity)
    {
        groundSprite = theGroundSprite;
        slotSprite = theSlotSprite;
        itemType = theItemType;
        rarity = theRarity;
    }

    //public Item(string type, float rarity, string name)
    //{
    //    itemType = type;
    //    itemRarity = rarity;
    //    itemName = name;
    //}
}
