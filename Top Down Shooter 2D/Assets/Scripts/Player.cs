using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Sprite parachuteSprite;
    [SerializeField] float speed;
    SpriteRenderer sr;
    Vector3 movement;
    Rigidbody2D rb;
    bool hasGun;
    bool jumped;
    bool landed;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        EnableOrDisableChildren(false);
    }

    void Update()
    {
        movement = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        rb.velocity = movement * speed;

        LookAtMouse();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!jumped)
            {
                sr.sprite = parachuteSprite;
                transform.parent = null;
                jumped = true;
            }
        }

        if (Input.GetKey(KeyCode.T) && jumped && !landed)
        {
            EnableOrDisableChildren(true);
            sr.sprite = null;
            landed = true;
        }
    }

    void LookAtMouse()
    {
        Vector3 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    void EnableOrDisableChildren(bool trueorfalse)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(trueorfalse);
        }
    }
}
