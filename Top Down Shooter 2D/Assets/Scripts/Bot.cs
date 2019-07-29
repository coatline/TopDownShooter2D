using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Bot : MonoBehaviour
{
    enum State
    {
        fleeingFromStorm,
        goingToLand,
        searching,
        getting,
        attacking,
        fleeing
    }

    public Image fallBarPrefab = null;
    public Sprite parachuteSprite = null;
    float parachuteFallSpeed = .1f;
    public Canvas worldSpaceCanvas = null;
    float parachuteMoveSpeed = 9;
    float freeFallSpeed = .25f;
    public float groundWalkSpeed = 0;
    List<GameObject> seenItems;
    public bool landed = false;
    State state = new State();
    Image fallBarFill = null;
    SpriteRenderer sr = null;
    List<Item> items = null;
    Item currentItem;
    public float speed = 0;
    BoxCollider2D trigger;
    BoxCollider2D headbc;
    bool jumped = false;
    GameObject target;
    GameObject land;
    List<GameObject> attackers;
    DeathCircle dc;
    Rigidbody2D rb;

    void Start()
    {
        seenItems = new List<GameObject>();
        attackers = new List<GameObject>();
        items = new List<Item>();

        headbc = transform.Find("Head").GetComponent<BoxCollider2D>();
        trigger = transform.Find("Trigger").GetComponent<BoxCollider2D>();

        land = GameObject.FindGameObjectWithTag("Land");

        dc = FindObjectOfType<DeathCircle>();

        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

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
        rb.angularVelocity = 0;
        rb.velocity = Vector2.zero;
        trigger.gameObject.transform.rotation = Quaternion.identity;

        for (int i = 0; i < seenItems.Count; i++)
        {
            if (!seenItems[i])
            {
                seenItems.RemoveAt(i);
            }
            else if (seenItems[i].CompareTag("Pickupable"))
            {
                seenItems.RemoveAt(i);
            }
        }

        if (!jumped)
        {
            transform.position = transform.parent.transform.position;
        }

        DoStates();

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

    void DoStates()
    {
        if (state == State.searching)
        {
            if (seenItems.Count == 0)
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
                transform.Translate(toTarget * speed * Time.deltaTime, Space.World);
            }
            else
            {
                if (seenItems.Count > 0)
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

            if (Vector2.Distance(transform.position, land.transform.position) <= 100)
            {
                if (seenItems.Count > 0)
                {
                    state = State.getting;
                }
                else
                {
                    state = State.searching;
                }
            }
        }
        else if (state == State.attacking)
        {
            if (items.Count > 0)
            {
                state = State.attacking;

            }
        }
        else if (state == State.fleeing)
        {
            if (attackers.Count == 0)
            {
                state = State.searching;
            }
            else
            {
                if (items.Count > 0)
                {
                    state = State.attacking;
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, ClosestAttacker().transform.position, -.1f);
                }
            }
        }
        else if (state == State.fleeingFromStorm)
        {
            //TODO MAKE SURE STORM IS MOVING

            if (Vector3.Distance(transform.position, dc.transform.position) <= dc.targetScale.x / 2.75f)
            {
                state = State.searching;
            }

            transform.position = Vector3.MoveTowards(transform.position, dc.transform.position, .1f);
        }
    }

    GameObject ClosestAttacker()
    {
        GameObject closest = null;

        for (int i = 0; i < attackers.Count; i++)
        {
            if (!closest)
            {
                closest = attackers[i];
            }
            else if (Vector2.Distance(transform.position, attackers[i].transform.position) < Vector2.Distance(transform.position, closest.transform.position))
            {
                closest = attackers[i];
            }
        }

        return closest;
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
            for (int i = 0; i < seenItems.Count; i++)
            {
                if (seenItems[i] == collision.gameObject)
                {
                    seenItems.RemoveAt(i);
                    break;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Crate") && state == State.searching)
        {
            if (seenItems.Contains(collision.gameObject)) { return; }

            AddToSeen(collision.gameObject);

            if (state != State.attacking && state != State.fleeingFromStorm)
            {
                state = State.getting;
            }
        }
        else if (collision.gameObject.CompareTag("Pickupable"))
        {
            AddToSeen(collision.gameObject);
            if (state != State.attacking && state != State.fleeingFromStorm)
            {
                state = State.getting;
            }

        }
        else if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Bot") && landed)
        {
            if (landed)
            {
                if (items.Count > 0)
                {
                    state = State.attacking;
                }
                else
                {
                    if (collision.gameObject.CompareTag("Player"))
                    {
                        if (collision.gameObject.GetComponentInParent<Player>().selectedSlot.item != null)
                        {
                            state = State.fleeing;
                        }
                    }
                    else
                    {
                        //if (collision.IsTouching(headbc))
                        //{
                        //    if (collision.gameObject.GetComponentInParent<Bot>().items.Count > 0)
                        //    {
                        //        state = State.fleeing;
                        //    }
                        //}
                        //else
                        {
                            if (collision.gameObject.GetComponentInParent<Bot>().items.Count > 0)
                            {
                                state = State.fleeing;
                            }
                        }
                    }
                }

                attackers.Add(collision.gameObject);
            }
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
        else if (collision.gameObject.CompareTag("DeathCircle"))
        {
            if (collision.gameObject.GetComponent<DeathCircle>().isMoving)
            {
                state = State.fleeingFromStorm;
            }
        }
    }

    void AddToSeen(GameObject obj)
    {
        seenItems.Add(obj);
    }

    void ChooseTarget()
    {
        GameObject closest = null;

        for (int i = 0; i < seenItems.Count; i++)
        {
            if (!closest)
            {
                closest = seenItems[i];
            }
            else if (Vector2.Distance(transform.position, seenItems[i].transform.position) < Vector2.Distance(transform.position, closest.transform.position))
            {
                closest = seenItems[i];
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
