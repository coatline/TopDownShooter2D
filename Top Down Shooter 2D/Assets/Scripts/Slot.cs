using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public GameObject itemHolder;
    Image holderImage;
    public Item item;

    private void Awake()
    {
        holderImage = transform.Find("Holder").GetComponent<Image>();
    }

    public void DropItem(Transform tra)
    {
        itemHolder.GetComponent<SpriteRenderer>().sprite = item.groundSprite;
        itemHolder.GetComponent<CircleCollider2D>().enabled = true;
        itemHolder.transform.position = tra.position;
        itemHolder.SetActive(true);
        itemHolder = null;
        holderImage.sprite = null;
        holderImage.color = new Color(0, 0, 0, 0);

        item = null;
    }

    public void DeSelect()
    {
        var imageScript = GetComponent<Image>();
        imageScript.color = new Color(imageScript.color.r, imageScript.color.g, imageScript.color.b, .5f);
        if (item)
        {
            itemHolder.gameObject.SetActive(false);
        }
    }

    public void Select()
    {
        var imageScript = GetComponent<Image>();
        imageScript.color = new Color(imageScript.color.r, imageScript.color.g, imageScript.color.b, 1f);
        if (item)
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

        if (selectedSlot == this)
        {
            itemHolder.gameObject.SetActive(true);
            itemHolder.GetComponent<CircleCollider2D>().enabled = false;
            itemHolder.GetComponent<SpriteRenderer>().sprite = itemHolder.GetComponent<Item>().inHandSprite;
        }
    }
}
