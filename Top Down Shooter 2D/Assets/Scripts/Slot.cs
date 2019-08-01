using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Slot : MonoBehaviour
{
    GameObject playerHoldingPlace;
    Image backgroundImage;
    Image holderImage;
    public Item item;

    private void Awake()
    {
        backgroundImage = transform.Find("Background Sprite").GetComponent<Image>();
        holderImage = transform.Find("Holder").GetComponent<Image>();
        playerHoldingPlace = FindObjectOfType<Player>().transform.Find("HoldingPlace").gameObject;
    }

    public void DestroyItem()
    {
        Destroy(item);
        item = null;

        holderImage.sprite = null;
        holderImage.color = new Color(0, 0, 0, 0);
        backgroundImage.color = new Color(0, 0, 0, 0);
    }

    public void DropItem(Transform tra)
    {
        CreateItem(tra);

        item = null;
        print("SDf");

        //itemHolder.tag = "Pickupable";
        //itemHolder.GetComponent<SpriteRenderer>().sprite = item.groundSprite;
        //itemHolder.GetComponent<CircleCollider2D>().enabled = true;
        //itemHolder.transform.position = tra.position + new Vector3(Random.Range(-.5f,.5f), Random.Range(-.5f, .5f));
        //itemHolder.SetActive(true);
        //itemHolder = null;
        //item = null;

        holderImage.sprite = null;
        holderImage.color = new Color(0, 0, 0, 0);
        backgroundImage.color = new Color(0, 0, 0, 0);
    }

    void CreateItem(Transform tra)
    {
        var newItem = new GameObject();

        newItem.tag = "Pickupable";

        if (this.item.itemType == "Gun")
        {
            //itemHolder.transform.Find("Outline").gameObject.SetActive(true);

            var itemGunScript = this.item.GetComponent<Gun>();

            newItem.AddComponent<Gun>();
            newItem.GetComponent<Gun>().SetAllVariables(itemGunScript.bulletLifeTime, itemGunScript.damagePerBullet, itemGunScript.bulletSpeed, itemGunScript.gunType, itemGunScript.aimError);
        }

        newItem.AddComponent<SpriteRenderer>();
        newItem.GetComponent<SpriteRenderer>().sprite = item.groundSprite;

        newItem.AddComponent<CircleCollider2D>();
        newItem.GetComponent<CircleCollider2D>().isTrigger = true;

        item.transform.position = tra.position + new Vector3(Random.Range(-.5f, .5f), Random.Range(-.5f, .5f));
    }

    public void DeSelect()
    {
        playerHoldingPlace.GetComponent<SpriteRenderer>().sprite = null;

        var imageScript = GetComponent<Image>();
        imageScript.color = new Color(imageScript.color.r, imageScript.color.g, imageScript.color.b, .5f);
    }

    public void Select()
    {
        ShowItemInHand();

        var imageScript = GetComponent<Image>();
        imageScript.color = new Color(imageScript.color.r, imageScript.color.g, imageScript.color.b, 1f);
    }

    public void ChangeItem(GameObject groundedItem, Slot selectedSlot)
    {
        item = groundedItem.GetComponent<Item>();

        Destroy(groundedItem);

        holderImage.sprite = item.groundSprite;
        holderImage.color = Color.white;

        //if already selected slot enable inhand sprite for gun
        if (selectedSlot == this)
        {
            ShowItemInHand();
        }

        SetColorToRarity(backgroundImage, item);
    }

    void ShowItemInHand()
    {
        if (!item)
        {
            if (playerHoldingPlace)
            {
                playerHoldingPlace.GetComponent<SpriteRenderer>().sprite = null;
            }

            return;
        }

        playerHoldingPlace.GetComponent<SpriteRenderer>().sprite = item.inHandSprite;
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
