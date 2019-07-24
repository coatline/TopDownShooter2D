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
    }

    public void Select()
    {
        var imageScript = GetComponent<Image>();
        imageScript.color = new Color(imageScript.color.r, imageScript.color.g, imageScript.color.b, 1f);
    }

    public void ChangeItem(GameObject groundedItem)
    {
        itemHolder = groundedItem;
        item = groundedItem.GetComponent<Item>();
        holderImage.sprite = item.groundSprite;
        holderImage.color = Color.white;
    }
}
