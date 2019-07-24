using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] Sprite parachuteSprite = null;
    [SerializeField] float parachuteMoveSpeed = 7;
    [SerializeField] float parachuteFallSpeed = 3;
    [SerializeField] TMP_Text bottomText = null;
    [SerializeField] float groundWalkSpeed = 0;
    [SerializeField] float waterWalkSpeed = 0;
    [SerializeField] Image fallBarFill = null;
    [SerializeField] float freeFallSpeed = 0;
    [SerializeField] SlotManager sm = null;
    [SerializeField] float speed = 0;
    public Slot selectedSlot = null;
    SpriteRenderer sr = null;
    Rigidbody2D rb = null;
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
                collision.gameObject.SetActive(false);
                sm.OpenSlot().ChangeItem(collision.gameObject);
            }
        }
        else if (collision.gameObject.CompareTag("Water"))
        {
            speed = waterWalkSpeed;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Water"))
        {
            speed = groundWalkSpeed;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!jumped)
            {
                Jump();
            }
        }

        if (!jumped) { return; }

        movement = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        rb.velocity = movement.normalized * speed;

        LookAtMouse();

        if (Input.GetMouseButton(0))
        {
            if (selectedSlot.item)
            {
                if (selectedSlot.item.itemType == "Gun")
                {
                    if (selectedSlot.item.GetComponent<Gun>().gunType == "AR")
                    {
                        selectedSlot.item.GetComponent<AR>().CalculateShotTime();
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (selectedSlot.item != null)
            {
                selectedSlot.DropItem(transform);
            }
        }

        if (jumped && !landed)
        {
            if (!Input.GetKey(KeyCode.Space))
            {
                fallBarFill.fillAmount -= parachuteFallSpeed * Time.deltaTime;
                transform.localScale -= new Vector3(parachuteFallSpeed / 2, parachuteFallSpeed / 2) * Time.deltaTime;
                fallBarFill.color = Color.white;
            }

            if (Input.GetKey(KeyCode.Space))
            {
                fallBarFill.fillAmount -= freeFallSpeed * Time.deltaTime;
                transform.localScale -= new Vector3(freeFallSpeed / 2, freeFallSpeed / 2) * Time.deltaTime;
                fallBarFill.color = Color.cyan;
            }

            if (fallBarFill.fillAmount <= 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
                Land();
            }
        }

        if (!landed) return;

        sm.SlotDisabledGroundItemFollow(transform);

        if (selectedSlot.item)
        {
            ShowItemHolding();
        }
    }

    void LookAtMouse()
    {
        Vector3 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    void ShowItemHolding()
    {
        if (selectedSlot.item)
        {
            selectedSlot.itemHolder.GetComponent<SpriteRenderer>().sprite = selectedSlot.item.inHandSprite;
            selectedSlot.itemHolder.gameObject.SetActive(true);
        }
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
        speed = groundWalkSpeed;
    }

    public void Jump()
    {
        fallBarFill.transform.parent.gameObject.SetActive(true);
        bottomText.gameObject.SetActive(false);
        sr.sprite = parachuteSprite;
        speed = parachuteMoveSpeed;
        transform.parent = null;
        jumped = true;
    }

    bool lastOnWater;

    //void CheckForWater()
    //{
    //    RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up);

    //    if (hit)
    //    {
    //        if (hit.transform.gameObject.CompareTag("Water"))
    //        {
    //            lastOnWater = true;
    //            print("ONWATWE");
    //            speed = waterWalkSpeed;
    //        }
    //        else
    //        {
    //            if (lastOnWater)
    //            {
    //                speed = groundWalkSpeed;
    //            }
    //            lastOnWater = false;
    //        }
    //    }
    //}
}
