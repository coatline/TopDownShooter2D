using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] Canvas worldSpaceCanvas = null;
    [SerializeField] TMP_Text killCountText = null;
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

    public int health = 100;
    public int shield = 0;
    public int kills;

    [SerializeField] float freeFallSpeed = 0;
    [SerializeField] float parachuteFallSpeed = 3;
    [SerializeField] float parachuteMoveSpeed = 7;
    public float groundWalkSpeed = 0;
    public float speed = 0;
    float gasTimer;

    void Awake()
    {
        var tmpTexts = FindObjectsOfType<TMP_Text>();

        for (int i = 0; i < tmpTexts.Length; i++)
        {
            if (tmpTexts[i].name == "KillCounterText")
            {
                killCountText = tmpTexts[i];
                break;
            }
        }

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
    }

    public void KilledEnemy()
    {
        kills++;
        killCountText.text = $"{kills}";
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

        if (Input.GetKeyDown(KeyCode.Space))
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
                selectedSlot.currentItemScript.Use(this);
            }
        }
    }

    void Heal(int amount, bool isShield)
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

        UpdateHealthUI();
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
        for (int i = 0; i < sm.slots.Count; i++)
        {
            sm.slots[i].DropItem(transform);
        }

        Destroy(gameObject);
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
