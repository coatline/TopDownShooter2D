using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Bot : MonoBehaviour
{
    enum State
    {
        fleeingFromStorm,
        searchingForItems,
        pickingUpItem,
        goingToLand,
        goingToItem,
        attacking,
        fleeingFromEnemy
    }

    [SerializeField] State state = new State();
    [SerializeField] int inventorySpace;
    [SerializeField] float turnSpeed;
    float parachuteFallSpeed = .1f;
    float parachuteMoveSpeed = 6.5f;
    float freeFallSpeed = .25f;
    public float groundWalkSpeed = 0;
    public float speed = 0;
    public int health = 100;

    public bool landed = false;
    bool jumped = false;

    GameObject currentTargetPlayer;
    Crate currentTargetCrate;
    Item currentTargetItem;
    GameObject bulletHole;
    GameObject land;

    public Canvas worldSpaceCanvas = null;
    public Image fallBarPrefab = null;
    public Sprite parachuteSprite = null;
    SpriteRenderer sr = null;
    SpriteRenderer holdingPlacesr;
    Image fallBarFill = null;
    BoxCollider2D trigger;
    BoxCollider2D headbc;
    Item currentItem;
    SelfDestruct sd;
    Rigidbody2D rb;
    AudioSource a;

    List<Crate> seenCrates = null;
    List<Item> seenItems;
    List<GameObject> attackers;
    List<Item> items = null;

    //TODO MAKE A CONSISTANT SPEED THAT THEY MOVE AT

    void Awake()
    {
        seenItems = new List<Item>();
        attackers = new List<GameObject>();
        seenCrates = new List<Crate>();
        items = new List<Item>();

        holdingPlacesr = transform.Find("HoldingPlace").GetComponent<SpriteRenderer>();
        headbc = transform.Find("Head").GetComponent<BoxCollider2D>();
        trigger = transform.Find("Trigger").GetComponent<BoxCollider2D>();
        bulletHole = transform.Find("BulletHole").gameObject;

        land = GameObject.FindGameObjectWithTag("Land");

        sd = GetComponent<SelfDestruct>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        a = GetComponent<AudioSource>();

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

        state = State.searchingForItems;
    }

    bool startedCoroutine;

    void Update()
    {
        rb.angularVelocity = 0;
        rb.velocity = Vector2.zero;
        trigger.gameObject.transform.rotation = Quaternion.identity;

        Intelligence();
        DoStates();
        CheckForStormDamage();

        if (jumped && !landed)
        {
            Fall();
        }
    }

    void Fall()
    {
        fallBarFill.transform.parent.transform.position = transform.position - new Vector3(3, 0, 0);

        fallBarFill.fillAmount -= parachuteFallSpeed * Time.deltaTime;
        transform.localScale -= new Vector3(parachuteFallSpeed / 2, parachuteFallSpeed / 2) * Time.deltaTime;

        if (fallBarFill.fillAmount <= 0)
        {
            Land();
        }
    }

    void CheckForMissingSeenCrates()
    {
        if (seenCrates.Count == 0) { return; }

        for (int i = seenCrates.Count - 1; i >= 0; i--)
        {
            //if (seenItems[i].)
            //{
            //    continue;
            //}
            if (!seenCrates[i])
            {
                seenCrates.RemoveAt(i);
            }
        }

        seenCrates.TrimExcess();
    }

    void CheckForMissingSeenItems()
    {
        if (seenItems.Count == 0) { return; }

        for (int i = seenItems.Count - 1; i >= 0; i--)
        {
            //if (seenItems[i].)
            //{
            //    continue;
            //}
            if (seenItems[i].GetComponent<Item>().pickedUp || !seenItems[i] || !seenItems[i].gameObject.activeSelf)
            {
                seenItems.RemoveAt(i);
            }
        }

        seenItems.TrimExcess();
    }

    float stormTimer;

    void CheckForStormDamage()
    {
        if (DeathCircle.IsOutsideCircle_Static(transform.position))
        {
            if (stormTimer > 1)
            {
                TakeDmg(1);
                stormTimer = 0;
            }
            else
            {
                stormTimer += Time.deltaTime;
            }
        }
    }

    public void TakeDmg(int damage)
    {
        //if (shield > 0)
        //{
        //    if (damage > shield)
        //    {
        //        damage -= shield;
        //        shield = 0;
        //        health -= damage;
        //    }
        //    else
        //    {
        //        shield -= damage;
        //    }
        //}
        //else
        //{
        //    health -= damage;
        //}

        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].Drop(transform);
        }

        sd.DoDie();
    }

    void Intelligence()
    {
        //Attack GetItems SearchforItems FleeFromEnemy RunFromStorm

        if (state == State.fleeingFromStorm)
        {
            if (DeathCircle.isInsideSafeZone_Static(transform.position) && state != State.pickingUpItem || state != State.attacking || state != State.fleeingFromEnemy)
            {
                state = State.searchingForItems;
            }
        }

        if (attackers.Count > 0)
        {
            if (HasGun())
            {
                state = State.attacking;
            }
            else
            {
                if (state != State.goingToItem && state != State.pickingUpItem)
                {
                    state = State.fleeingFromEnemy;
                }
            }
        }

        if (state == State.attacking)
        {
            if (!currentTargetPlayer)
            {
                if (attackers.Count > 0)
                {
                    currentTargetPlayer = ClosestAttacker();
                }
                else
                {
                    state = State.searchingForItems;
                }
            }
            else if (currentTargetPlayer != ClosestAttacker())
            {
                currentTargetPlayer = ClosestAttacker();
            }
        }
        else
        {
            if (state == State.pickingUpItem)
            {

            }
            else
            {
                if (state == State.goingToItem)
                {
                    if (HasGun() && attackers.Count > 0)
                    {
                        state = State.attacking;
                    }
                }
                else
                {
                    if (state == State.fleeingFromEnemy)
                    {
                        if (HasGun())
                        {
                            ChooseWhatItemToHold();
                            state = State.attacking;
                        }
                        else if (ClosestAttacker() && Vector2.Distance(ClosestAttacker().transform.position, transform.position) > 20)
                        {
                            state = State.searchingForItems;
                        }
                    }
                    else
                    {
                        if ((!DeathCircle.isInsideSafeZone_Static(transform.position) && DeathCircle.IsMoving_Static()) || DeathCircle.IsOutsideCircle_Static(transform.position))
                        {
                            state = State.fleeingFromStorm;
                        }
                        else
                        {
                            if (state == State.searchingForItems)
                            {
                                if (seenItems.Count > 0 || seenCrates.Count > 0)
                                {
                                    state = State.goingToItem;
                                }

                                CheckForMissingSeenItems();
                                CheckForMissingSeenCrates();
                            }
                            else
                            {
                                if (state == State.goingToLand)
                                {
                                }
                            }
                        }
                    }
                }
            }
        }

    }

    void ChooseWhatItemToHold()
    {
        if (HasGun() && !GunEquipped())
        {
            EquipGun();
        }
    }

    //void UseConsumable()
    //{
    //    if (HoldingConsumable)
    //    {
    //        currentItem.
    //    }

    //    ShowCurrentItemInHand();
    //}

    //void HoldConsumable()
    //{
    //    var consumable

    //    currentItem = 
    //}

    bool HoldingConsumable()
    {
        if (currentItem.itemType == "Healing")
        {
            return true;
        }

        return false;
    }

    void DoStates()
    {
        if (state == State.searchingForItems)
        {
            rb.velocity = new Vector2(transform.up.x, transform.up.y) * groundWalkSpeed;

            if (!startedCoroutine)
            {
                StartCoroutine(Search());
            }
        }
        else if (state == State.goingToItem)
        {
            GoToItem();
        }
        else if (state == State.pickingUpItem)
        {
            PickUpItem();
        }
        else if (state == State.goingToLand)
        {
            GoToLand();
        }
        else if (state == State.attacking)
        {
            Attack();
        }
        else if (state == State.fleeingFromEnemy)
        {
            FleeFromEnemy();
        }
        else if (state == State.fleeingFromStorm)
        {
            FleeFromStorm();
        }
    }

    public void KilledEnemy()
    {
        currentTargetPlayer = null;
    }

    void Attack()
    {
        if (!currentTargetPlayer || !HasGun()) { state = State.searchingForItems; return; }

        if (!GunEquipped())
        {
            EquipGun();
        }

        Aim();
        var itemGunScript = currentItem.GetComponent<Gun>();
        itemGunScript.Shoot(bulletHole, gameObject, true, a);

        if (Vector2.Distance(currentTargetPlayer.transform.position, transform.position) > itemGunScript.bulletLifeTime / itemGunScript.bulletSpeed)
        {
            MoveTowards(currentTargetPlayer.transform, 1);
        }
    }

    void GoToItem()
    {
        if (seenItems.Count == 0)
        {
            if (seenCrates.Count > 0)
            {
                if (!currentTargetCrate)
                {
                    if (ChooseTargetCrate() == null)
                    {
                        state = State.searchingForItems;
                        return;
                    }
                    else
                    {
                        currentTargetCrate = ChooseTargetCrate();
                    }
                }

                startedCoroutine = false;
                StopAllCoroutines();

                MoveTowards(currentTargetCrate.transform, 1);

                if (Vector2.Distance(transform.position, currentTargetCrate.transform.position) <= .15f)
                {
                    if (landed)
                    {
                        state = State.pickingUpItem;
                    }
                    else
                    {
                        parachuteFallSpeed = freeFallSpeed;
                        fallBarFill.color = Color.cyan;
                    }
                }
                else
                {
                    MoveTowards(currentTargetCrate.transform, 1);
                }


            }
        }

        if (!currentTargetItem || currentTargetItem.pickedUp)
        {
            seenItems.Remove(currentTargetItem);

            if (seenItems.Count > 0)
            {
                currentTargetItem = ChooseTargetItem();
            }
            else
            {
                state = State.searchingForItems;
            }

            return;
        }

        startedCoroutine = false;
        StopAllCoroutines();

        if (Vector2.Distance(transform.position, currentTargetItem.transform.position) <= .15f)
        {
            if (landed)
            {
                state = State.pickingUpItem;
            }
            else
            {
                parachuteFallSpeed = freeFallSpeed;
                fallBarFill.color = Color.cyan;
            }
        }
        else
        {
            MoveTowards(currentTargetItem.transform, 1);
        }
    }

    void Aim()
    {
        var dir = currentTargetPlayer.transform.position - transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //angle += Random.Range(-5, 5);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle - 90, Vector3.forward), Time.deltaTime);
    }

    void GoToLand()
    {
        MoveTowards(land.transform, 1);

        if (Vector2.Distance(transform.position, land.transform.position) <= 199)
        {
            if (seenItems.Count > 0)
            {
                state = State.goingToItem;
            }
            else
            {
                state = State.searchingForItems;
            }
        }
    }

    void FleeFromStorm()
    {
        MoveTowards(DeathCircle.instance.transform, 1);
    }

    void FleeFromEnemy()
    {
        if (!ClosestAttacker()) { Intelligence(); return; }

        if (attackers.Count == 0)
        {
            state = State.searchingForItems;
        }
        else
        {
            MoveTowards(ClosestAttacker().transform, -1);
        }
    }

    void ShowCurrentItemInHand()
    {
        if (!currentItem)
        {
            print("No Item TO SHOW!");
            holdingPlacesr.sprite = null;
        }
        else
        {
            holdingPlacesr.sprite = currentItem.inHandSprite;
        }
    }

    bool HasGun()
    {
        if (items.Count == 0) { return false; }

        for (int i = items.Count - 1; i >= 0; i--)
        {
            if (!items[i])
            {
                items.RemoveAt(i);
                continue;
            }

            if (items[i].itemType == "Gun")
            {
                return true;
            }
        }

        return false;
    }

    void EquipGun()
    {
        for (int i = items.Count - 1; i >= 0; i--)
        {
            if (items[i].itemType == "Gun")
            {
                currentItem = items[i];
            }
        }

        ShowCurrentItemInHand();
    }

    bool GunEquipped()
    {
        if (currentItem.itemType == "Gun")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void PickUpItem()
    {
        //If I have a target item, pick it up
        //Otherwise, Choose a traget item that I have seen
        if (!currentTargetItem || currentTargetItem.pickedUp)
        {
            if (!currentTargetItem)
            {
                if (ChooseTargetItem())
                {
                    currentTargetItem = ChooseTargetItem();
                }
                else
                {
                    state = State.searchingForItems;
                    return;
                }
            }

            state = State.searchingForItems;

            if (currentTargetItem.pickedUp)
                seenItems.Remove(currentTargetItem);

            return;
        }

        items.Add(currentTargetItem);

        if (!currentItem)
            currentItem = currentTargetItem;

        ShowCurrentItemInHand();
        currentTargetItem.PickUp();
        ChooseWhatItemToHold();

        seenItems.Remove(currentTargetItem);
        state = State.searchingForItems;
    }

    Item ChooseTargetItem()
    {
        CheckForMissingSeenItems();

        if (seenItems.Count == 0)
        {
            return null;
        }

        Item closest = null;

        for (int i = 0; i < seenItems.Count; i++)
        {
            if (!closest)
            {
                closest = seenItems[i];
            }
            else if (Vector2.Distance(transform.position, closest.transform.position) < Vector2.Distance(transform.position, seenItems[i].transform.position))
            {
                closest = seenItems[i];
            }
        }

        return closest;
    }

    Crate ChooseTargetCrate()
    {
        //Crate closest = null;

        //for (int i = 0; i < seenCrates.Count; i++)
        //{
        //    if (!closest)
        //    {
        //        closest = seenCrates[i];
        //    }
        //    else if (Vector2.Distance(transform.position, closest.transform.position) < Vector2.Distance(transform.position, seenCrates[i].transform.position))
        //    {
        //        closest = seenCrates[i];
        //    }
        //}

        return seenCrates[Random.Range(0, seenCrates.Count)];
    }

    GameObject ClosestAttacker()
    {
        if (attackers.Count == 0)
        {
            return null;
        }

        GameObject closest = null;

        for (int i = attackers.Count - 1; i >= 0; i--)
        {
            if (!attackers[i])
            {
                attackers.RemoveAt(i);
                continue;
            }

            if (!closest)
            {
                closest = attackers[i];
            }
            //error because we do not notify the bot if the attacker gets killed so attackers[i] could be null causing an error
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
        yield return new WaitForSeconds(Random.Range(1, 5));
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360f))), Time.deltaTime * turnSpeed);
        StartCoroutine(Search());
    }

    void AddToSeenItems(Item obj)
    {
        seenItems.Add(obj);
    }

    void AddToSeenCrates(Crate obj)
    {
        seenCrates.Add(obj);
    }

    void ChangeDir()
    {
        Vector3 dir = transform.position - new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
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
    }

    void EnableOrDisableChildren(bool trueorfalse)
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
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

    void MoveTowards(Transform target, int dir)
    {
        Vector3 dire = (transform.position - target.position);

        if (dir == -1)
        {
            dire = -dire;
        }

        float angele = Mathf.Atan2(dire.y, dire.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angele + 90), Time.deltaTime * turnSpeed);

        var toTargeta = (target.position - transform.position).normalized;
        transform.Translate(toTargeta * speed * Time.deltaTime * dir, Space.World);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Crate"))
        {
            collision.gameObject.GetComponent<Crate>().Open();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Pickupable"))
        {
            var itemScript = (collision.gameObject.GetComponent<Item>());

            if (seenItems.Contains(itemScript))
            {
                //if (currentTargetItem == itemScript)
                //{
                //    currentTargetItem = ChooseTargetItem();
                //}

                seenItems.Remove(itemScript);
            }
        }

        else if (collision.gameObject.CompareTag("Crate"))
        {
            var crateScript = collision.gameObject.GetComponent<Crate>();

            if (seenCrates.Contains(crateScript))
            {
                //if (currentTargetCrate == crateScript)
                //{
                //    currentTargetCrate = ChooseTargetCrate();
                //}

                seenCrates.Remove(crateScript);
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Crate"))
        {
            if (seenCrates.Contains(collision.gameObject.GetComponent<Crate>())) { return; }

            AddToSeenCrates(collision.gameObject.GetComponent<Crate>());

            if (state != State.attacking && state != State.fleeingFromStorm)
            {
                state = State.goingToItem;
            }
        }

        if (collision.gameObject.CompareTag("Pickupable"))
        {
            AddToSeenItems(collision.gameObject.GetComponent<Item>());

            if (state == State.searchingForItems)
            {
                state = State.goingToItem;
            }

        }

        if (landed)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if (!attackers.Contains(collision.gameObject))
                {
                    attackers.Add(collision.gameObject.transform.parent.gameObject);
                }
            }
            else if (collision.gameObject.CompareTag("Bot"))
            {
                if (!attackers.Contains(collision.gameObject))
                {
                    attackers.Add(collision.gameObject.transform.parent.gameObject);
                }
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
            }

            ChangeDir();
        }
    }
}
