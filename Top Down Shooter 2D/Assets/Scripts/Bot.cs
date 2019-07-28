using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Bot : MonoBehaviour
{
    enum State
    {
        goingToLand,
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
    List<GameObject> seenCrates;
    public bool landed = false;
    State state = new State();
    Image fallBarFill = null;
    SpriteRenderer sr = null;
    List<Item> items = null;
    public float speed = 0;
    BoxCollider2D trigger;
    BoxCollider2D headbc;
    bool jumped = false;
    GameObject target;
    GameObject land;

    void Start()
    {
        headbc = transform.Find("Head").GetComponent<BoxCollider2D>();

        land = GameObject.FindGameObjectWithTag("Land");

        seenCrates = new List<GameObject>();

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

        state = State.searching;
    }

    bool startedCoroutine;

    void Update()
    {
        trigger.gameObject.transform.rotation = Quaternion.identity;

        for (int i = 0; i < seenCrates.Count; i++)
        {
            if (!seenCrates[i])
            {
                seenCrates.RemoveAt(i);
            }
        }

        if (!jumped)
        {
            transform.position = transform.parent.transform.position;
        }

        if (state == State.searching)
        {
            if (seenCrates.Count == 0)
            {
                transform.Translate(.1f, 0, 0);

                if (!startedCoroutine)
                {
                    StartCoroutine(Search());
                }
            }
            else
            {
                state = State.getting;
            }
        }
        else if (state == State.getting)
        {
            startedCoroutine = false;
            StopAllCoroutines();

            if (target != null)
            {
                //Vector3 dir = transform.position - target.transform.position;
                //float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                //transform.rotation = Quaternion.Euler(0, 0, angle + 90);

                //transform.Translate(.1f, 0,0,Space.Self);
                var toTarget = (target.transform.position - transform.position).normalized;
                transform.Translate(toTarget * speed * Time.deltaTime);
            }
            else
            {
                if (seenCrates.Count > 0)
                {
                    ChooseTarget();
                }
                else
                {
                    state = State.searching;
                }
            }
        }
        else if (state == State.goingToLand)
        {
            transform.position = Vector3.MoveTowards(transform.position, land.transform.position, .1f);
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

    IEnumerator Search()
    {
        startedCoroutine = true;
        yield return new WaitForSeconds(5f);
        transform.rotation = Quaternion.Euler(new Vector3(Random.Range(-1, 1), Random.Range(-10, 10), 0f));
        StartCoroutine(Search());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Crate") && landed)
        {
            for (int i = 0; i < seenCrates.Count; i++)
            {
                if (seenCrates[i] == collision.gameObject)
                {
                    seenCrates.RemoveAt(i);
                    break;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Crate") && state == State.searching)
        {
            if (seenCrates.Contains(collision.gameObject)) { return; }

            AddToSeen(collision.gameObject);

            if (state != State.attacking)
            {
                state = State.getting;
            }
        }
        else if (collision.gameObject.CompareTag("Pickupable"))
        {
            //AddToSeen(collision.gameObject);

            //if (state != State.attacking)
            //{
            //    state = State.getting;
            //}
        }
        else if (collision.gameObject.CompareTag("Player"))
        {

        }
        else if (collision.gameObject.CompareTag("Water"))
        {
            if (landed && collision.IsTouching(headbc))
            {
                state = State.goingToLand;
            }
            else if (!landed)
            {
                state = State.goingToLand;
            }

            ChangeDir();
        }
    }

    void AddToSeen(GameObject obj)
    {
        seenCrates.Add(obj);
    }

    void ChooseTarget()
    {
        GameObject closest = null;

        for (int i = 0; i < seenCrates.Count; i++)
        {
            if (!closest)
            {
                closest = seenCrates[i];
            }
            else if (Vector2.Distance(transform.position, seenCrates[i].transform.position) < Vector2.Distance(transform.position, closest.transform.position))
            {
                closest = seenCrates[i];
            }
        }

        target = closest;
    }

    void ChangeDir()
    {
        Vector3 dir = transform.position - new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 90);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Water"))
        {
            if (seenCrates.Count > 0)
            {
                state = State.getting;
            }
            else
            {
                state = State.goingToLand;
            }
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
