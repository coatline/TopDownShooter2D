using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject player;
    public float lifeTime;
    public bool botBullet;
    SpriteRenderer sr;
    Rigidbody2D rb;
    public int dmg;
    bool dying;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        Invoke("DoDie", lifeTime);
    }

    float alph = 1;

    void DoDie()
    {
        dying = true;

        rb.velocity = Vector2.zero;

        alph -= Time.deltaTime * 1.25f;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alph);

        if (alph <= 0)
        {
            Destroy(gameObject);
        }
    }

    bool done;

    private void Update()
    {
        if (!done)
        {
            done = true;
        }

        if (dying)
        {
            DoDie();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var collisionGameobjectTag = collision.gameObject.tag;

        if (collisionGameobjectTag == "Bullet" || collisionGameobjectTag == "Pickupable" || collisionGameobjectTag == "Eyes" || collisionGameobjectTag == "DeathCircle" || collisionGameobjectTag == "Water" || dying || !player) { return; }

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

                if (botScript.health - dmg <= 0)
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
        }
    }
}
