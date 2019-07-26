using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public GameObject itemHolder;
    Image backgroundImage;
    Image holderImage;
    public Item item;

    private void Awake()
    {
        backgroundImage = transform.Find("Background Sprite").GetComponent<Image>();
        holderImage = transform.Find("Holder").GetComponent<Image>();
    }

    public void DropItem(Transform tra)
    {
        if (item.itemType == "Gun")
        {
            itemHolder.transform.Find("Outline").gameObject.SetActive(true);
        }

        itemHolder.GetComponent<SpriteRenderer>().sprite = item.groundSprite;
        itemHolder.GetComponent<CircleCollider2D>().enabled = true;
        itemHolder.transform.position = tra.position;
        itemHolder.SetActive(true);
        itemHolder = null;
        item = null;

        holderImage.sprite = null;
        holderImage.color = new Color(0, 0, 0, 0);
        backgroundImage.color = new Color(0, 0, 0, 0);

    }

    public void DeSelect()
    {
        var imageScript = GetComponent<Image>();
        imageScript.color = new Color(imageScript.color.r, imageScript.color.g, imageScript.color.b, .5f);

        if (item && itemHolder)
        {
            itemHolder.gameObject.SetActive(false);
        }
    }

    public void Select()
    {

        var imageScript = GetComponent<Image>();
        imageScript.color = new Color(imageScript.color.r, imageScript.color.g, imageScript.color.b, 1f);

        if (item && itemHolder)
        {
            itemHolder.gameObject.SetActive(true);
            itemHolder.GetComponent<SpriteRenderer>().sprite = itemHolder.GetComponent<Item>().inHandSprite;
        }
    }

    public void ChangeItem(GameObject groundedItem, Slot selectedSlot)
    {
        itemHolder = groundedItem;
        item = groundedItem.GetComponent<Item>();
        holderImage.sprite = item.groundSprite;
        holderImage.color = Color.white;

        //if already selected slot enable inhand sprite for gun
        if (selectedSlot == this)
        {
            itemHolder.gameObject.SetActive(true);
            itemHolder.GetComponent<SpriteRenderer>().sprite = itemHolder.GetComponent<Item>().inHandSprite;
        }

        itemHolder.GetComponent<CircleCollider2D>().enabled = false;

        SetColorToRarity(backgroundImage, item);

        if (item.itemType == "Gun")
        {
            itemHolder.transform.Find("Outline").gameObject.SetActive(false);
        }
    }

    void SetColorToRarity(Image image, Item item)
    {
        if (item.itemType != "Gun") { return; }

        var r = item.rarity;

        if (r == "Common")
        {
            image.color = Color.gray;
        }
        else if (r == "Uncommon")
        {
            image.color = Color.green;
        }
        else if (r == "Rare")
        {
            image.color = Color.cyan;
        }
        else if (r == "Epic")
        {
            image.color = new Color(.9f, .1f, .9f);
        }
        else if (r == "Legendary")
        {
            image.color = Color.yellow;
        }
        else if (r == "Mythic")
        {
            image.color = Color.red;
        }

    }
}
