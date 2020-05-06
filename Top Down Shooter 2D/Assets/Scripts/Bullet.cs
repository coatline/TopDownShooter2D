using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject player;
    public float lifeTime;
    public bool botBullet;
    SelfDestruct sd;
    public int dmg;
    bool dying;

    private void Start()
    {
        sd = GetComponent<SelfDestruct>();
        Invoke("Die", lifeTime);
    }

    void Die()
    {
        sd.DoDie();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var collisionGameobjectTag = collision.gameObject.tag;

        if (collisionGameobjectTag == "Bullet" || collisionGameobjectTag == "Pickupable" || collisionGameobjectTag == "Eyes" || collisionGameobjectTag == "DeathCircle" || collisionGameobjectTag == "Water" || dying || !player || collisionGameobjectTag == "Door") { return; }

        else
        {
            if (collisionGameobjectTag == "Player")
            {
                var playerScript = collision.gameObject.GetComponentInParent<Player>();

                if (playerScript.health - dmg <= 0)
                {
                    if (botBullet)
                    {
                        player.GetComponent<Bot>().KilledEnemy();
                    }
                    else
                    {
                        player.GetComponent<Player>().KilledEnemy();
                    }
                }

                playerScript.TakeDmg(dmg);
                Destroy(gameObject);
            }
            else if (collisionGameobjectTag == "Bot")
            {
                var botScript = collision.gameObject.GetComponentInParent<Bot>();

                if (botScript.health - dmg <= 0 && !botScript.dead)
                {
                    if (botBullet)
                    {
                        player.GetComponent<Bot>().KilledEnemy();
                    }
                    else
                    {
                        player.GetComponent<Player>().KilledEnemy();
                    }
                }

                botScript.TakeDmg(dmg);
                Destroy(gameObject);
            }

            dying = true;
            Die();
        }
    }
}
