using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Slot : MonoBehaviour
{
    [SerializeField] Color hightlightedColor;
    [SerializeField] Color normalColor;
    GameObject playerHoldingPlace;
    public Item currentItemScript;
    Image backgroundImage;
    Image imageScript;
    Image holderImage;
    bool selected;

    //holderImage is the holder of the image of the gun in the slot

    private void Awake()
    {
        imageScript = GetComponent<Image>();
        backgroundImage = transform.Find("Background Sprite").GetComponent<Image>();
        holderImage = transform.Find("Holder").GetComponent<Image>();
        playerHoldingPlace = FindObjectOfType<Player>().transform.Find("HoldingPlace").gameObject;
    }

    public void DestroyItem()
    {
        Destroy(currentItemScript.gameObject);
        currentItemScript = null;

        UpdateItemInHand();

        holderImage.sprite = null;
        holderImage.color = new Color(0, 0, 0, 0);
        backgroundImage.color = new Color(0, 0, 0, 0);
    }

    public void DropItem(Transform tra)
    {
        if (!currentItemScript)
        {
            return;
        }

        currentItemScript.Drop(tra);

        //itemHolder.tag = "Pickupable";
        //itemHolder.GetComponent<SpriteRenderer>().sprite = item.groundSprite;
        //itemHolder.GetComponent<CircleCollider2D>().enabled = true;
        //itemHolder.transform.position = tra.position + new Vector3(Random.Range(-.5f,.5f), Random.Range(-.5f, .5f));
        //itemHolder.SetActive(true);
        //itemHolder = null;
        //item = null;

        currentItemScript = null;

        UpdateItemInHand();

        holderImage.sprite = null;
        holderImage.color = new Color(0, 0, 0, 0);
        backgroundImage.color = new Color(0, 0, 0, 0);
    }

    public void DeSelect()
    {
        selected = false;

        playerHoldingPlace.GetComponent<SpriteRenderer>().sprite = null;

        imageScript.color = normalColor;
    }

    public void Select()
    {
        selected = true;

        UpdateItemInHand();

        imageScript.color = hightlightedColor;
    }

    public void ChangeItem(Transform playerTransform, Item newItemScript, Slot selectedSlot)
    {
        //paramater selectedSlot to check if already selected then show the item

        //if already has item in slot, drop it
        if (currentItemScript)
        {
            DropItem(playerTransform);
        }

        currentItemScript = newItemScript;

        newItemScript.gameObject.SetActive(false);

        //WOULD CHANGE SPRITE TO SLOTSPRITE BUT DO NOT HAVE A DIFFERENT SPRITE FOR IT AT THE MOMENT
        holderImage.sprite = newItemScript.groundSprite;
        holderImage.color = Color.white;

        //if already selected slot enable inhand sprite for gun
        if (selectedSlot == this)
        {
            UpdateItemInHand();
        }

        SetColorToRarity(backgroundImage, newItemScript);
    }

    void UpdateItemInHand()
    {
        if (!currentItemScript)
        {
            if (playerHoldingPlace)
            {
                playerHoldingPlace.GetComponent<SpriteRenderer>().sprite = null;
            }

            return;
        }

        playerHoldingPlace.GetComponent<SpriteRenderer>().sprite = currentItemScript.inHandSprite;
    }

    void SetColorToRarity(Image image, Item theItemScript)
    {
        if (theItemScript.itemType != "Gun") { return; }

        var r = theItemScript.rarity;

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
