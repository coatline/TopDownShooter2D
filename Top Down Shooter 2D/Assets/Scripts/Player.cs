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
    public Slot selectedSlot = null;
    SpriteRenderer sr = null;
    Image fallBarFill = null;
    Rigidbody2D rb = null;
    GameObject inline;
    DeathCircle dc;

    Vector3 movement;

    public bool landed;
    public bool jumped;
    bool hasGun;

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
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        dc = FindObjectOfType<DeathCircle>();

        inline = dc.transform.Find("DeathCircleInLine").gameObject;

        EnableOrDisableChildren(false);

        UpdateHealthUI();
    }

    void Update()
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
                Jump();
            }
        }

        if (!jumped) { return; }

        movement = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        rb.velocity = movement.normalized * speed;

        LookAtMouse();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (selectedSlot.item != null)
            {
                selectedSlot.DropItem(transform);
            }
        }

        if (jumped && !landed)
        {
            fallBarFill.transform.parent.transform.position = transform.position - new Vector3(3, 0, 0);

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

        //if (transform.position < ((inline.transform.localScale / 2) + inline.transform.position))

        sm.SlotDisabledGroundItemFollow(transform.Find("HoldingPlace").transform);

        if (Input.GetMouseButton(0))
        {
            if (selectedSlot.item)
            {
                if (selectedSlot.item.itemType == "Gun")
                {
                    if (selectedSlot.item.GetComponent<Gun>().gunType == "AR")
                    {
                        selectedSlot.item.GetComponent<AR>().player = this;
                        selectedSlot.item.GetComponent<AR>().CalculateShotTime();
                    }
                    else if (selectedSlot.item.GetComponent<Gun>().gunType == "SMG")
                    {
                        selectedSlot.item.GetComponent<SMG>().player = this;
                        selectedSlot.item.GetComponent<SMG>().CalculateShotTime();
                    }
                }
                else if (selectedSlot.item.itemType == "Healing")
                {
                    var script = selectedSlot.item.GetComponent<Healing>();

                    if ((script.isShield && shield >= 100) || (!script.isShield && health >= 100)) { return; }

                    Heal(selectedSlot.item.GetComponent<Healing>().amount, selectedSlot.item.GetComponent<Healing>().isShield);
                    selectedSlot.DestroyItem();
                }
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
            print("Dead");
        }
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
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Pickupable"))
        {
            if (Input.GetKeyDown(KeyCode.E) && landed)
            {
                collision.gameObject.SetActive(false);

                if (!sm.OpenSlot())
                {
                    selectedSlot.DropItem(transform);
                }

                sm.OpenSlot().ChangeItem(collision.gameObject, selectedSlot);

            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                collision.gameObject.SetActive(false);

                if (sm.slots[0].item)
                {
                    sm.slots[0].DropItem(transform);
                }

                sm.slots[0].ChangeItem(collision.gameObject, selectedSlot);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                collision.gameObject.SetActive(false);

                if (sm.slots[1].item)
                {
                    sm.slots[1].DropItem(transform);
                }

                sm.slots[1].ChangeItem(collision.gameObject, selectedSlot);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                collision.gameObject.SetActive(false);

                if (sm.slots[2].item)
                {
                    sm.slots[2].DropItem(transform);
                }

                sm.slots[2].ChangeItem(collision.gameObject, selectedSlot);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                collision.gameObject.SetActive(false);

                if (sm.slots[3].item)
                {
                    sm.slots[3].DropItem(transform);
                }

                sm.slots[3].ChangeItem(collision.gameObject, selectedSlot);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                collision.gameObject.SetActive(false);

                if (sm.slots[4].item)
                {
                    sm.slots[4].DropItem(transform);
                }

                sm.slots[4].ChangeItem(collision.gameObject, selectedSlot);
            }
        }
        else if (collision.gameObject.CompareTag("DeathCircle"))
        {
            if (gasTimer >= 1)
            {
                TakeDmg(5);
                gasTimer = 0;
            }
            else
            {
                gasTimer += Time.deltaTime;
            }
        }
    }
}
