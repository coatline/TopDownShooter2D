using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] GameObject obj = null;
    [SerializeField] bool randItem = false;
    [SerializeField] float chance = 0;
    ItemGenerator ig = null;

    void Start()
    {
        if (randItem)
        {
            ig = FindObjectOfType<ItemGenerator>();
            ig.GenerateItem(transform.position);
        }
        else
        {
            if (Random.Range(0, 101) <= chance)
            {
                Instantiate(obj, transform.position, Quaternion.identity);
            }
        }

        Destroy(gameObject);
    }
}
