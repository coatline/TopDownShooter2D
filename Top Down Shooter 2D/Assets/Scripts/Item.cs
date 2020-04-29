using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    //rarity is chance to drop
    public Sprite groundSprite;
    public Sprite inHandSprite;
    public Sprite slotSprite;
    public string itemType;
    public string itemName;
    public string rarity;
    public bool pickedUp;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Pickupable"))
        {
            //Move away from other items on the ground
            transform.position = Vector3.MoveTowards(transform.position, collision.gameObject.transform.position, -Time.deltaTime * 1.5f);
        }
    }

    public void PickUp()
    {
        gameObject.SetActive(false);

        pickedUp = true;

        transform.position = new Vector3(1000, 1000, 0);
    }

    public void Drop(Transform playerTransform)
    {
        gameObject.SetActive(true);

        pickedUp = false;

        transform.position = playerTransform.position + new Vector3(Random.Range(-.5f, .5f), Random.Range(-.5f, .5f));
    }
}
