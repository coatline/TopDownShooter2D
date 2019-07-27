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
            if (!collision.gameObject.GetComponentInParent<Player>().landed) { return; }
            collision.gameObject.GetComponentInParent<Player>().speed = waterWalkSpeed;
        }
        else if (collision.gameObject.CompareTag("Bot"))
        {
            if (!collision.gameObject.GetComponentInParent<Bot>().landed) { return; }
            collision.gameObject.GetComponentInParent<Bot>().speed = waterWalkSpeed;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!collision.gameObject.GetComponentInParent<Player>().landed) { return; }
            var script = collision.gameObject.GetComponentInParent<Player>();
            script.speed = script.groundWalkSpeed;
        }
        else if (collision.gameObject.CompareTag("Bot"))
        {
            if (!collision.gameObject.GetComponentInParent<Bot>().landed) { return; }
            var script = collision.gameObject.GetComponentInParent<Bot>();
            script.speed = script.groundWalkSpeed;
        }
    }
}
