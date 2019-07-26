using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    [SerializeField] List<Item> itemPrefabs = null;

    Item RandomItem()
    {
        return itemPrefabs[Random.Range(0, itemPrefabs.Count)];
    }

    public void GenerateItem(Vector3 pos)
    {
        if (itemPrefabs.Count == 0)
        {
            Debug.LogError("No items in pool");
        }

        //common uncommon rare epic legendary mythic
        // 40%     25%     18%   10%    5%      2%

        var r = Random.Range(0, 101);

        if (r <= 40)
        {
            //Spawn COMMON item
            var newItem = Instantiate(RandomItem(), pos, Quaternion.identity);
            newItem.GetComponent<Item>().rarity = "Common";
        }
        else if (r > 40 && r <= 65)
        {
            //Spawn UNCOMMON item
            var newItem = Instantiate(RandomItem(), pos, Quaternion.identity);
            newItem.GetComponent<Item>().rarity = "Uncommon";
        }
        else if (r > 65 && r <= 83)
        {
            //Spawn RARE item
            var newItem = Instantiate(RandomItem(), pos, Quaternion.identity);
            newItem.GetComponent<Item>().rarity = "Rare";
        }
        else if (r > 83 && r <= 93)
        {
            //Spawn EPIC item
            var newItem = Instantiate(RandomItem(), pos, Quaternion.identity);
            newItem.GetComponent<Item>().rarity = "Epic";
        }
        else if (r > 93 && r <= 97)
        {
            //Spawn Legendary item
            var newItem = Instantiate(RandomItem(), pos, Quaternion.identity);
            newItem.GetComponent<Item>().rarity = "Legendary";
        }
        else if (r > 97 && r <= 100)
        {
            //Spawn MYTHIC item
            var newItem = Instantiate(RandomItem(), pos, Quaternion.identity);
            newItem.GetComponent<Item>().rarity = "Mythic";
        }
    }
}
