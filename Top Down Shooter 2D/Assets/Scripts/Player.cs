using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] Sprite parachuteSprite = null;
    [SerializeField] TMP_Text bottomText = null;
    [SerializeField] float parachuteSpeed = 0;
    [SerializeField] Image fallBarFill = null;
    [SerializeField] float freeFallSpeed = 0;
    [SerializeField] SlotManager sm = null;
    [SerializeField] float speed = 0;
    SpriteRenderer sr = null;
    Rigidbody2D rb = null;
    public Slot selectedSlot;
    public bool jumped;
    Vector3 movement;
    bool hasGun;
    bool landed;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        EnableOrDisableChildren(false);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Pickupable"))
        {
            if (Input.GetKeyDown(KeyCode.E) && landed)
            {
                print(sm.OpenSlot());
                sm.OpenSlot().ChangeItem(collision.gameObject.GetComponent<Item>());
                Destroy(collision.gameObject);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!jumped)
            {
                Jump();
            }
        }

        if (!jumped) { return; }

        movement = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        rb.velocity = movement * speed;

        LookAtMouse();

        if (Input.GetKeyDown(KeyCode.Q) && selectedSlot)
        {
            if (selectedSlot.item)
            {
                selectedSlot.DropItem(transform);
            }
        }

        if (jumped && !landed)
        {
            if (!Input.GetKey(KeyCode.Space))
            {
                fallBarFill.fillAmount -= parachuteSpeed * Time.deltaTime;
                fallBarFill.color = Color.white;
            }

            if (Input.GetKey(KeyCode.Space))
            {
                fallBarFill.fillAmount -= freeFallSpeed * Time.deltaTime;
                fallBarFill.color = Color.blue;
            }

            if (fallBarFill.fillAmount <= 0)
            {
                Land();
            }
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

    void Land()
    {
        fallBarFill.transform.parent.gameObject.SetActive(false);
        EnableOrDisableChildren(true);
        sr.sprite = null;
        landed = true;
    }

    public void Jump()
    {
        fallBarFill.transform.parent.gameObject.SetActive(true);
        bottomText.gameObject.SetActive(false);
        sr.sprite = parachuteSprite;
        transform.parent = null;
        jumped = true;
    }
}
