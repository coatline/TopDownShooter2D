using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime;
    public int dmg;

    private void Start()
    {
        Invoke("DoDie", lifeTime);
    }

    void DoDie()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet")) { return; }
        else { DoDie(); }
    }
}
