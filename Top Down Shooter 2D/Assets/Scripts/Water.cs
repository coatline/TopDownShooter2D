using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] float waterWalkSpeed = 3;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!collision.gameObject.GetComponent<Player>().landed) { return; }
            collision.gameObject.GetComponent<Player>().speed = waterWalkSpeed;
            print("WATER!");
        }
        else if (collision.gameObject.CompareTag("Bot"))
        {
            if (!collision.gameObject.GetComponent<Bot>().landed) { return; }
            collision.gameObject.GetComponent<Bot>().speed = waterWalkSpeed;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!collision.gameObject.GetComponent<Player>().landed) { return; }
            var script = collision.gameObject.GetComponent<Player>();
            script.speed = script.groundWalkSpeed;
        }
        else if (collision.gameObject.CompareTag("Bot"))
        {
            if (!collision.gameObject.GetComponent<Bot>().landed) { return; }
            var script = collision.gameObject.GetComponent<Bot>();
            script.speed = script.groundWalkSpeed;
        }
    }
}
