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
    public int shield = 0;

    [SerializeField] bool inWater;
    public bool landed = false;
    bool jumped = false;
    public bool dead;

    GameObject currentTargetPlayer;
    public GameObject bulletHole;
    Crate currentTargetCrate;
    Item currentTargetItem;
    GameObject land;

    public Canvas worldSpaceCanvas = null;
    public Image fallBarPrefab = null;
    public Sprite parachuteSprite = null;
    SpriteRenderer sr = null;
    SpriteRenderer holdingPlacesr;
    Image fallBarFill = null;
    BoxCollider2D trigger;
    BoxCollider2D headbc;
    public AudioSource a;
    Item currentItem;
    SelfDestruct sd;
    Rigidbody2D rb;
    public Transform botHolder;

    List<Crate> seenCrates = null;
    List<Item> seenItems;
    List<GameObject> attackers;
    List<Item> items = null;
    int blockingMask;
    Vector3 lastMovePos;
    float stuckTimer;
    float ignoreTimer;
    GameObject ignoredAttacker;

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

        blockingMask = LayerMask.GetMask("Default", "Crate");

        sd = GetComponent<SelfDestruct>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        a = GetComponent<AudioSource>();

        Invoke("Jump", Random.Range(0f, 20f));

        if (transform.parent == botHolder)
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
    float noticeTimer;

    void Update()
    {
        rb.angularVelocity = 0;
        rb.linearVelocity = Vector2.zero;
        trigger.gameObject.transform.rotation = Quaternion.identity;

        if (dead) { return; }

        noticeTimer += Time.deltaTime;

        if (noticeTimer >= 0.5f)
        {
            noticeTimer = 0;
            NoticeEnemies();
        }

        if (ignoreTimer > 0)
        {
            ignoreTimer -= Time.deltaTime;
        }

        if (state == State.attacking)
        {
            CheckIfStuck();
        }

        Intelligence();
        DoStates();
        CheckForStormDamage();

        if (jumped && !landed)
        {
            Fall();
        }
    }

    void CheckIfStuck()
    {
        if (!currentTargetPlayer) { return; }

        if (Vector2.Distance(transform.position, currentTargetPlayer.transform.position) < 0.2f) { return; }

        var moved = Vector2.Distance(transform.position, lastMovePos);
        lastMovePos = transform.position;

        if (moved < 0.02f)
        {
            stuckTimer += Time.deltaTime;

            if (stuckTimer > 1f)
            {
                GiveUpOnTarget();
            }
        }
        else
        {
            stuckTimer = 0;
        }
    }

    void GiveUpOnTarget()
    {
        stuckTimer = 0;
        ignoreTimer = 2f;
        ignoredAttacker = currentTargetPlayer;

        attackers.Remove(currentTargetPlayer);
        currentTargetPlayer = null;

        state = State.searchingForItems;
    }

    void NoticeEnemies()
    {
        if (!landed) { return; }

        var hits = Physics2D.OverlapCircleAll(transform.position, 20, LayerMask.GetMask("Player"));

        for (int i = 0; i < hits.Length; i++)
        {
            var go = hits[i].gameObject;

            if (go.tag != "Player" && go.tag != "Bot") { continue; }

            if (!go.transform.parent) { continue; }

            var target = go.transform.parent.gameObject;

            if (target == gameObject) { continue; }

            if (target == ignoredAttacker && ignoreTimer > 0) { continue; }

            if (target.tag == "Player" && Player.dead) { continue; }

            var botScript = target.GetComponent<Bot>();
            if (botScript && botScript.dead) { continue; }

            if (!attackers.Contains(target))
            {
                attackers.Add(target);
            }
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
                TakeDmg(3);
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
        if (shield > 0)
        {
            if (damage > shield)
            {
                damage -= shield;
                shield = 0;
                health -= damage;
            }
            else
            {
                shield -= damage;
            }
        }
        else
        {
            health -= damage;
        }

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (!dead)
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].Drop(transform);
            }
        }

        dead = true;

        sd.DoDie();
    }

    void Intelligence()
    {
        //Attack GetItems SearchforItems FleeFromEnemy RunFromStorm

        if (!DeathCircle.isInsideSafeZone_Static(transform.position))
        {
            if (state != State.attacking)
            {
                state = State.fleeingFromStorm;
                return;
            }
        }

        if (state == State.fleeingFromStorm)
        {
            if (DeathCircle.isInsideSafeZone_Static(transform.position))
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
                    if (!currentTargetItem)
                    {
                        ReEvaluate();
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
                        if (state == State.goingToLand)
                        {
                        }
                        else
                        {
                            if (state == State.searchingForItems)
                            {
                                if (seenItems.Count > 0 || seenCrates.Count > 0)
                                {
                                    state = State.goingToItem;
                                }
                                if (inWater)
                                {
                                    state = State.goingToLand;
                                }

                                CheckForMissingSeenItems();
                                CheckForMissingSeenCrates();
                            }
                            else if (state != State.fleeingFromStorm)
                            {
                                state = State.searchingForItems;
                            }
                        }
                    }
                }
            }
        }
    }

    void ReEvaluate()
    {
        if (state == State.pickingUpItem || state == State.goingToItem)
        {
            if (attackers.Count > 0)
            {
                if (HasGun())
                {
                    Attack();
                }
                else
                {
                    state = State.fleeingFromEnemy;
                }
            }
            else
            {
                if (inWater)
                {
                    state = State.goingToLand;
                }
                //else if (seenCrates.Count > 0)
                //{
                //    ChooseTargetCrate();
                //    state = State.searchingForItems;
                //}
                //else if (seenItems.Count > 0)
                //{
                //    ChooseTargetItem();
                //    state = State.searchingForItems;
                //}
                else
                {
                    state = State.searchingForItems;
                }
            }
        }
        //else if(state==State.)
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
            rb.linearVelocity = new Vector2(transform.up.x, transform.up.y) * groundWalkSpeed;

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

    public void AddAttacker(GameObject attacker)
    {
        if (!attacker || attacker == gameObject) { return; }

        if (IsDeadTarget(attacker)) { return; }

        if (!attackers.Contains(attacker))
        {
            attackers.Add(attacker);
        }
    }

    void Attack()
    {
        if (!currentTargetPlayer || !HasGun()) { return; }

        if (IsDeadTarget(currentTargetPlayer))
        {
            GiveUpOnTarget();
            return;
        }

        if (!HasLineOfSight())
        {
            GiveUpOnTarget();
            return;
        }

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

    bool HasLineOfSight()
    {
        var dir = currentTargetPlayer.transform.position - transform.position;

        var hit = Physics2D.Raycast(transform.position, dir.normalized, dir.magnitude, blockingMask);

        return !hit;
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
                        ReEvaluate();
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
            else
            {
                ReEvaluate();
            }
        }

        if (!currentTargetItem || currentTargetItem.pickedUp)
        {
            seenItems.Remove(currentTargetItem);

            if (seenItems.Count > 0)
            {
                currentTargetItem = ChooseTargetItem();
            }

            ReEvaluate();
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

        if (!inWater)
        {
            ReEvaluate();
        }
    }

    void FleeFromStorm()
    {
        MoveTowards(DeathCircle.instance.transform, 1);
    }

    void FleeFromEnemy()
    {
        if (!ClosestAttacker()) { ReEvaluate(); return; }

        if (attackers.Count == 0)
        {
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
                    ReEvaluate();
                    currentTargetItem = ChooseTargetItem();
                }
                else
                {
                    ReEvaluate();
                    return;
                }
            }


            if (currentTargetItem.pickedUp)
                seenItems.Remove(currentTargetItem);

            ReEvaluate();
            return;
        }

        items.Add(currentTargetItem);

        if (!currentItem)
            currentItem = currentTargetItem;

        ShowCurrentItemInHand();
        currentTargetItem.PickUp();
        ChooseWhatItemToHold();
        seenItems.Remove(currentTargetItem);
        ReEvaluate();
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

            if (IsDeadTarget(attackers[i]))
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

    bool IsDeadTarget(GameObject target)
    {
        var botScript = target.GetComponent<Bot>();
        if (botScript && botScript.dead) { return true; }

        if (target.GetComponent<Player>() && Player.dead) { return true; }

        return false;
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
        Vector3 dir = transform.position - new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0).normalized;
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
        transform.parent = botHolder;
        trigger.enabled = true;
        jumped = true;
    }

    void MoveTowards(Transform target, int dir)
    {
        if ((transform.position - target.transform.position).magnitude < .1f) { return; }

        Vector3 dire = (transform.position - target.position);

        if (dir == -1)
        {
            dire = -dire;
        }

        float angele = Mathf.Atan2(dire.y, dire.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angele + 90), Time.deltaTime * turnSpeed);

        var toTargeta = (target.position - transform.position).normalized;

        if (items.Count > 0)
        {
            transform.Translate(toTargeta * speed * Time.deltaTime * dir, Space.World);
        }
        else
        {
            transform.Translate(toTargeta * (speed + .1f) * Time.deltaTime * dir, Space.World);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Crate"))
        {
            collision.gameObject.GetComponent<Crate>().Open();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!landed) { return; }

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Bot"))
        {
            if (!collision.gameObject.transform.parent) { return; }

            var target = collision.gameObject.transform.parent.gameObject;

            if (target != gameObject && !attackers.Contains(target) && !IsDeadTarget(target))
            {
                attackers.Add(target);
            }
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
        else if (collision.gameObject.CompareTag("Water"))
        {
            inWater = false;
        }
        else if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Bot"))
        {
            if (!collision.gameObject.transform.parent) { return; }

            var target = collision.gameObject.transform.parent.gameObject;

            if (attackers.Contains(target))
            {
                attackers.Remove(target);
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
            if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Bot"))
            {
                if (!collision.gameObject.transform.parent) { return; }

                var target = collision.gameObject.transform.parent.gameObject;

                if (target != gameObject && !attackers.Contains(target) && !IsDeadTarget(target))
                {
                    attackers.Add(target);
                }
            }
        }

        else if (collision.gameObject.CompareTag("Water"))
        {
            if (landed)
            {
                print("enterseddsdfs");

                inWater = true;
            }

            ChangeDir();
        }
    }
}