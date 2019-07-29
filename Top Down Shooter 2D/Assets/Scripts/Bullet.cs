using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime;
    public Player player;
    public int dmg;
    AudioSource a;

    private void Start()
    {
        a = GetComponent<AudioSource>();

        a.Play();

        Invoke("DoDie", lifeTime);
    }

    void DoDie()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet") || collision.gameObject.CompareTag("Pickupable") || collision.gameObject.CompareTag("Eyes") || collision.gameObject.CompareTag("DeathCircle") || collision.gameObject.CompareTag("Water")) { return; }

        else
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                var plHealth = collision.gameObject.GetComponent<Player>().health;

                if (plHealth - dmg <= 0)
                {
                    player.kills++;
                }

                collision.gameObject.GetComponent<Player>().TakeDmg(dmg);
            }

            DoDie();
        }
    }
}
