using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] Canvas worldSpaceCanvas = null;
    [SerializeField] Sprite parachuteSprite = null;
    [SerializeField] TMP_Text bottomText = null;
    [SerializeField] Image fallBarPrefab = null;
    [SerializeField] SlotManager sm = null;
    [SerializeField] GameObject mapUI;
    [SerializeField] Image healthUi;
    public ItemDisplay selectedSlot = null;
    public GameObject bulletHole;
    SpriteRenderer sr = null;
    Image fallBarFill = null;
    GameObject holdingPlace;
    Rigidbody2D rb = null;
    public AudioSource a;
    AudioHandler ah;
    Item overItem;

    Vector3 movement;

    public bool landed;
    public bool jumped;
    public static bool dead;
    public static bool won;

    public int health = 100;
    public int shield = 0;
    public int kills;

    [SerializeField] float freeFallSpeed = 0;
    [SerializeField] float parachuteFallSpeed = 3;
    [SerializeField] float parachuteMoveSpeed = 7;
    public float groundWalkSpeed = 0;
    public float speed = 0;
    float gasTimer;
    float healTimer;

    void Awake()
    {
        dead = false;
        won = false;

        ah = FindObjectOfType<AudioHandler>();
        a = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        holdingPlace = transform.Find("HoldingPlace").gameObject;
        bulletHole = transform.Find("BulletHole").gameObject;

        EnableOrDisableChildren(false);

        UpdateHealthUI();
    }

    void Update()
    {
        Inputs();
        CheckForStormDamage();
        UpdatePickupPrompt();
    }

    void UpdatePickupPrompt()
    {
        if (won) { return; }

        if (landed)
        {
            if (overItem)
                bottomText.text = $"'E' ({overItem.itemName})";
            else if (selectedDoor)
                bottomText.text = $"'E' (open door)";
            else
            {
                bottomText.gameObject.SetActive(false);
                return;
            }
            bottomText.gameObject.SetActive(true);
        }
        else if (landed)
        {
            bottomText.gameObject.SetActive(false);
        }
    }

    public void KilledEnemy()
    {
        kills++;
    }

    float stormTimer;

    void CheckForStormDamage()
    {
        if (DeathCircle.IsOutsideCircle_Static(transform.position))
        {
            if (stormTimer > 1)
            {
                TakeDmg(5);
                stormTimer = 0;
            }
            else
            {
                stormTimer += Time.deltaTime;
            }
        }
    }

    void Inputs()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            mapUI.SetActive(true);
        }
        else
        {
            mapUI.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.buildIndex);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (!jumped)
            {
                ah.PlayJumpSound(a);
                Jump();
            }
        }

        if (!jumped) { return; }

        movement = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        rb.linearVelocity = movement.normalized * speed;

        LookAtMouse();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (selectedSlot.currentItemScript != null)
            {
                DropCurrentItem();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
        {
            if (landed)
            {
                if (overItem)
                {
                    PickupItem(overItem);
                    overItem = null;
                }
                else if (selectedDoor)
                {
                    selectedDoor.Interact(transform);
                }
            }

        }

        if (jumped && !landed)
        {
            fallBarFill.transform.parent.transform.position = transform.position - new Vector3(3, 0, 0);

            if (Input.GetKey(KeyCode.Space))
            {
                fallBarFill.fillAmount -= freeFallSpeed * Time.deltaTime;
                transform.localScale -= new Vector3(freeFallSpeed / 2, freeFallSpeed / 2) * Time.deltaTime;
                fallBarFill.color = Color.cyan;
            }
            else
            {
                fallBarFill.fillAmount -= parachuteFallSpeed * Time.deltaTime;
                transform.localScale -= new Vector3(parachuteFallSpeed / 2, parachuteFallSpeed / 2) * Time.deltaTime;
                fallBarFill.color = Color.white;
            }

            if (fallBarFill.fillAmount <= 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
                Land();
            }
        }

        if (!landed) return;

        if (Input.GetMouseButton(0))
        {
            if (selectedSlot.currentItemScript)
            {
                var item = selectedSlot.currentItemScript;

                if (item.itemType == "Healing")
                {
                    if (healTimer > 0)
                    {
                        healTimer -= Time.deltaTime;
                    }
                    else if (item.Use(this))
                    {
                        healTimer = 0.35f;
                        selectedSlot.DestroyItem();
                    }
                }
                else
                {
                    item.Use(this);
                }
            }
        }
    }

    public bool Heal(int amount, bool isShield)
    {
        if (isShield && shield < 100)
        {
            shield += amount;
            if (shield > 100)
            {
                shield = 100;
            }
        }
        else if (health < 100)
        {
            health += amount;
            if (health > 100)
            {
                health = 100;
            }
        }
        else
        {
            return false;
        }

        UpdateHealthUI();
        return true;
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

        UpdateHealthUI();

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        dead = true;

        for (int i = 0; i < sm.slots.Count; i++)
        {
            sm.slots[i].DropItem(transform);
        }

        if (!won)
        {
            ShowDeathUI();
        }

        Destroy(gameObject);
    }

    public void ShowWinUI()
    {
        won = true;

        if (!bottomText) { return; }

        var winText = Instantiate(bottomText.gameObject, bottomText.transform.parent);
        var rect = winText.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(500, 80);

        var winTxt = winText.GetComponent<TMP_Text>();
        winTxt.horizontalAlignment = HorizontalAlignmentOptions.Center;
        winTxt.verticalAlignment = VerticalAlignmentOptions.Top;
        winTxt.text = "You win!";
        winTxt.fontSize = 70;
        winTxt.color = Color.yellow;

        bottomText.gameObject.SetActive(true);
        bottomText.text = "Press R to restart";
    }

    void ShowDeathUI()
    {
        if (!bottomText) { return; }

        var diedText = Instantiate(bottomText.gameObject, bottomText.transform.parent);
        var rect = diedText.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        diedText.GetComponent<TMP_Text>().fontSize = 10;
        diedText.GetComponent<TMP_Text>().horizontalAlignment = HorizontalAlignmentOptions.Center;
        diedText.GetComponent<TMP_Text>().verticalAlignment = VerticalAlignmentOptions.Top;
        rect.sizeDelta = new Vector2(500, 80);

        var diedTxt = diedText.GetComponent<TMP_Text>();
        diedTxt.text = "You died";
        diedTxt.fontSize = 70;
        diedTxt.color = Color.red;

        bottomText.gameObject.SetActive(true);
        bottomText.text = "Press R to restart";
    }

    void UpdateHealthUI()
    {
        float healthFill = (health / 100f);
        var healthBarFill = healthUi.transform.Find("HealthBarFill");
        healthBarFill.GetComponent<Image>().fillAmount = healthFill;
        healthBarFill.Find("HealthTxt").GetComponent<TMP_Text>().text = $"{health}/100";

        float shieldFill = (shield / 100f);
        var shieldBarFill = healthUi.transform.Find("ShieldBarFill");
        shieldBarFill.GetComponent<Image>().fillAmount = shieldFill;
        shieldBarFill.Find("ShieldTxt").GetComponent<TMP_Text>().text = $"{shield}/100";
    }

    void LookAtMouse()
    {
        Vector3 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(angle - 90, Vector3.forward), Time.deltaTime * 20f);
    }

    void EnableOrDisableChildren(bool trueorfalse)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "PlayerBeacon") { continue; }
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
        var yah = Instantiate(fallBarPrefab, transform.position, Quaternion.Euler(0, 0, 90), worldSpaceCanvas.transform);
        fallBarFill = yah.transform.Find("Fill").GetComponent<Image>();
        bottomText.gameObject.SetActive(false);
        sr.sprite = parachuteSprite;
        speed = parachuteMoveSpeed;
        transform.parent = null;
        jumped = true;
    }

    void DropCurrentItem()
    {
        sm.selectedSlot.DropItem(transform);
    }

    void PickupItem(Item item)
    {
        item.PickUp();

        if (item.itemType == "Healing")
        {
            var slot = sm.ContatinsItem(item);

            if (slot)
            {
                slot.StackItem();
            }
            else
            {
                sm.OpenSlot().ChangeItem(transform, item, sm.selectedSlot);
            }
        }
        else
        {
            sm.OpenSlot().ChangeItem(transform, item, sm.selectedSlot);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Crate"))
        {
            collision.gameObject.GetComponent<Crate>().Open();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Pickupable"))
        {
            overItem = collision.gameObject.GetComponent<Item>();
        }
        else if (collision.gameObject.CompareTag("Door"))
        {
            selectedDoor = collision.gameObject.GetComponentInParent<Door>();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Pickupable"))
        {
            overItem = collision.gameObject.GetComponent<Item>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Pickupable"))
        {
            overItem = null;
        }
        else if (collision.gameObject.CompareTag("Door"))
        {
            selectedDoor = null;
        }
    }

    Door selectedDoor;
}
