using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] float fadeSpeed;
    SpriteRenderer sr;
    Rigidbody2D rb;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    float alph = 1;
    bool dying;

    public void DoDie()
    {
        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<SpriteRenderer>())
                {
                    transform.GetChild(i).GetComponent<SpriteRenderer>().color = new Color(sr.color.r, sr.color.g, sr.color.b, alph);
                }
            }
        }

        dying = true;

        if (GetComponent<Rigidbody2D>())
        {
            rb.linearVelocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        if (GetComponent<Collider2D>())
        {
            GetComponent<Collider2D>().enabled = false;
        }

        alph -= Time.deltaTime * fadeSpeed;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alph);

        if (alph <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (dying)
        {
            DoDie();
        }
    }
}
