using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    ItemGenerator ig;
    int health = 5;

    private void Awake()
    {
        ig = FindObjectOfType<ItemGenerator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Open();
        }
        else if (collision.gameObject.CompareTag("Bot"))
        {
            Open();
        }
        else if (collision.gameObject.CompareTag("Bullet"))
        {
            health--;

            if (health <= 0)
            {
                Open();
            }
        }
    }

    void Open()
    {
        ig.GenerateItem(transform.position);
        Destroy(gameObject);
    }
}
