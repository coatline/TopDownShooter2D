using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Bot : MonoBehaviour
{
    enum State
    {
        searching,
        getting,
        attacking
    }

    public Image fallBarPrefab = null;
    public Sprite parachuteSprite = null;
    float parachuteFallSpeed = .1f;
    public Canvas worldSpaceCanvas = null;
    float parachuteMoveSpeed = 9;
    float freeFallSpeed = .25f;
    public float groundWalkSpeed = 0;
    public bool landed = false;
    State state = new State();
    Image fallBarFill = null;
    SpriteRenderer sr = null;
    BoxCollider2D bc = null;
    List<Item> items = null;
    public float speed = 0;
    bool jumped = false;
    GameObject target;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();

        items = new List<Item>();

        Invoke("Jump", Random.Range(0f, 20f));
        if (transform.parent == null)
        {
            jumped = true;
            Land();
        }
        else
        {
            EnableOrDisableChildren(false);
            bc.enabled = false;
        }
    }

    void Update()
    {
        if (!jumped)
        {
            transform.position = transform.parent.transform.position;
        }

        if (state == State.searching)
        {
            transform.Translate(.1f, 0, 0);
        }

        if (state == State.getting)
        {
            if (target != null)
            {
                var toTarget = (target.transform.position - transform.position).normalized;
                transform.Translate(toTarget * speed * Time.deltaTime);
                //transform.position = Vector3.MoveTowards(transform.position, target.transform.position, .1f);
            }
            else
                state = State.searching;
        }

        if (jumped && !landed)
        {
            fallBarFill.transform.parent.transform.position = transform.position - new Vector3(3, 0, 0);

            fallBarFill.fillAmount -= parachuteFallSpeed * Time.deltaTime;
            transform.localScale -= new Vector3(parachuteFallSpeed / 2, parachuteFallSpeed / 2) * Time.deltaTime;
            fallBarFill.color = Color.white;


            if (fallBarFill.fillAmount <= 0)
            {
                Land();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Crate") && state == State.searching)
        {
            target = collision.gameObject;
            print(collision.gameObject.name);
            state = State.getting;
        }
        else if (collision.gameObject.CompareTag("Pickupable"))
        {
            state = State.searching;
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            target = collision.gameObject;
            state = State.getting;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Crate"))
        {
            if (target == collision.gameObject)
                state = State.searching;
        }
    }

    void Land()
    {
        if (fallBarFill != null)
            Destroy(fallBarFill.transform.parent.gameObject);
        EnableOrDisableChildren(true);
        sr.sprite = null;
        landed = true;
        speed = groundWalkSpeed;
        state = State.searching;
    }

    void EnableOrDisableChildren(bool trueorfalse)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(trueorfalse);
        }
    }

    public void Jump()
    {
        if (jumped) { return; }

        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 361));
        var yah = Instantiate(fallBarPrefab, transform.position, Quaternion.Euler(0, 0, 90), worldSpaceCanvas.transform);
        fallBarFill = yah.transform.Find("Fill").GetComponent<Image>();
        sr.sprite = parachuteSprite;
        speed = parachuteMoveSpeed;
        transform.parent = null;
        bc.enabled = true;
        jumped = true;
    }
}
