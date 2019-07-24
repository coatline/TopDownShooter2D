using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    [SerializeField] List<Item> itemPrefabs;

    void Start()
    {

    }

    Item RandomItem()
    {
        return itemPrefabs[Random.Range(0, itemPrefabs.Count)];
    }

    public void GenerateItem(Vector3 pos)
    {
        if(itemPrefabs.Count == 0)
        {
            Debug.LogError("No items in pool"); 
        }

        //common uncommon rare epic mythic
        // 40%     30%    15%   10%   5%

        var r = Random.Range(0, 101);

        if (r <= 40)
        {
            //Spawn COMMON item
            var newItem = Instantiate(RandomItem(), pos, Quaternion.identity);
            newItem.GetComponent<Item>().rarity = "Common";
        }
        else if (r > 40 && r <= 70)
        {
            //Spawn UNCOMMON item
            var newItem = Instantiate(RandomItem(), pos, Quaternion.identity);
            newItem.GetComponent<Item>().rarity = "Uncommon";
        }
        else if (r > 70 && r <= 85)
        {
            //Spawn RARE item
            var newItem = Instantiate(RandomItem(), pos, Quaternion.identity);
            newItem.GetComponent<Item>().rarity = "Rare";
        }
        else if (r > 85 && r <= 95)
        {
            //Spawn EPIC item
            var newItem = Instantiate(RandomItem(), pos, Quaternion.identity);
            newItem.GetComponent<Item>().rarity = "Epic";
        }
        else
        {
            //Spawn MYTHIC item
            var newItem = Instantiate(RandomItem(), pos, Quaternion.identity);
            newItem.GetComponent<Item>().rarity = "Mythic";
        }
    }
}
