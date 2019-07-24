using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] GameObject obj;
    [SerializeField] bool randItem;
    [SerializeField] float chance;
    ItemGenerator ig;

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
