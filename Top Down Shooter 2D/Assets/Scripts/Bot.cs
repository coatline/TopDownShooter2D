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
    List<Item> items = null;
    public float speed = 0;
    List<Crate> seenCrates;
    BoxCollider2D trigger;
    bool jumped = false;
    GameObject target;
    Vector3 dir;

    void Start()
    {
        seenCrates = new List<Crate>();

        trigger = transform.Find("Trigger").GetComponent<BoxCollider2D>();

        sr = GetComponent<SpriteRenderer>();

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
            trigger.enabled = false;
        }

        dir = transform.position - new Vector3(transform.position.x + Random.Range(-5, 5), transform.position.y + Random.Range(-5, 5));

        state = State.searching;
    }

    void Update()
    {
        trigger.gameObject.transform.rotation = Quaternion.identity;

        if (!jumped)
        {
            transform.position = transform.parent.transform.position;
        }

        if (state == State.searching)
        {
            transform.rotation = Quaternion.identity;
            transform.Translate(dir);
        }

        if (state == State.getting)
        {
            if (target != null)
            {
                Vector3 dir = transform.position - target.transform.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle + 90);
                var toTarget = (target.transform.position - transform.position).normalized;
                transform.Translate(toTarget * speed * Time.deltaTime);
            }
            else
            {
                if (seenCrates.Count == 0)
                {
                    state = State.searching;
                }
                else
                {
                    ChooseNewTarget();
                }
            }
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

    void ChooseNewTarget()
    {
        if (seenCrates.Count == 0) { state = State.searching; return; }
        target = seenCrates[0].gameObject;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Crate"))
        {
            for (int i = 0; i < seenCrates.Count; i++)
            {
                if (seenCrates[i].gameObject == collision.gameObject)
                {
                    seenCrates.Remove(seenCrates[i]);

                    state = State.searching;

                    ChooseNewTarget();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Crate") && state == State.searching)
        {
            seenCrates.Add(collision.gameObject.GetComponent<Crate>());
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
        else if (collision.gameObject.CompareTag("Water"))
        {
            ChangeDir();
        }
    }

    void ChangeDir()
    {
        dir = transform.position - new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 90);
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
            if (transform.GetChild(i).name == "Trigger") { continue; }
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
        trigger.enabled = true;
        jumped = true;
    }
}
