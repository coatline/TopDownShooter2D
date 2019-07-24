using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    //rarity is chance to drop
    public Sprite groundSprite;
    public Sprite inHandSprite;
    public string itemType;
    public string itemName;
    public string rarity;

    public void SetAllVariables(Sprite theGroundSprite, Sprite theInHandSprite, string theItemType, string theRarity, string theName)
    {
        groundSprite = theGroundSprite;
        inHandSprite = theInHandSprite;
        itemType = theItemType;
        rarity = theRarity;
        itemName = theName;
    }

    //public Item(string type, float rarity, string name)
    //{
    //    itemType = type;
    //    itemRarity = rarity;
    //    itemName = name;
    //}
}
