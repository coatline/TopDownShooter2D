using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Slot : MonoBehaviour
{
    Image holderImage;
    public Item item;

    private void Awake()
    {
        holderImage = GetComponentInChildren<Image>();
    }

    public void DropItem(Transform pos)
    {
        var dropItem = new GameObject();

        dropItem.AddComponent<Item>();

        var dropItemScript = dropItem.GetComponent<Item>();

        dropItemScript.SetAllVariables(item.groundSprite, item.slotSprite, item.itemType, item.rarity);

        if (item.itemType == "Gun")
        {
            dropItem.AddComponent<Gun>();

            if (dropItem.GetComponent<Gun>().gunType == "AR")
            {
                dropItem.AddComponent<AR>();
            }

            dropItem.AddComponent<SpriteRenderer>();

            dropItem.GetComponent<SpriteRenderer>().sprite = item.groundSprite;
        }

        dropItem.AddComponent<CircleCollider2D>();

        dropItem.GetComponent<CircleCollider2D>().isTrigger = true;

        item = null;
    }

    public void ChangeItem(Item newItem)
    {
        item = newItem;
        holderImage.sprite = item.slotSprite;
        holderImage.color = Color.white;
    }
}
