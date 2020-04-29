using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab = null;
    [SerializeField] float burstCount;
    public GameObject player = null;
    AudioSource playerAudioSource;
    public float burstDelay;
    public float shotRate;
    GameObject bulletHole;
    AudioHandler ah;
    float shotTimer;
    //////////////////////
    public float bulletLifeTime;
    public int damagePerBullet;
    public float bulletSpeed;
    public string gunType; //Shotgun, AR, 
    public float aimError;
    bool shooting;
    bool isbot;
    //public float ammo;

    private void Awake()
    {
        ah = FindObjectOfType<AudioHandler>();
    }

    private void Start()
    {
        var outsr = transform.Find("Outline").GetComponent<SpriteRenderer>();

        var gunrarity = GetComponent<Item>().rarity;

        if (gunrarity == "Common")
        {
            //stay the same?
            outsr.color = Color.gray;
        }
        else if (gunrarity == "Uncommon")
        {
            damagePerBullet++;
            outsr.color = Color.green;
        }
        else if (gunrarity == "Rare")
        {
            damagePerBullet += 3;
            outsr.color = Color.cyan;
        }
        else if (gunrarity == "Epic")
        {
            damagePerBullet += 5;
            outsr.color = new Color(.9f, .1f, .9f);
        }
        else if (gunrarity == "Legendary")
        {
            damagePerBullet += 7;
            outsr.color = Color.yellow;
        }
        else
        {
            damagePerBullet += 10;
            outsr.color = Color.red;
        }
    }

    float burstTimer;
    int burstNumber;
    bool bursting;

    public void CalculateShotTime()
    {
        shooting = true;

        shotTimer += Time.deltaTime;

        if (burstCount > 0)
        {
            if (burstTimer > burstDelay && !bursting)
            {
                bursting = true;
            }
            else if (bursting)
            {
                //start burst

                if (burstNumber < burstCount)
                {
                    if (shotTimer >= shotRate)
                    {
                        shotTimer = 0;
                        FireBullet();
                        burstNumber++;
                    }
                }
                else
                {
                    burstNumber = 0;
                    burstTimer = 0;
                    bursting = false;
                }

            }
            else
            {
                burstTimer += Time.deltaTime;
            }
        }
        else
        {
            if (shotTimer >= shotRate)
            {
                shotTimer = 0;
                FireBullet();
            }
        }
    }

    private void Update()
    {
        if (!shooting)
        {
            shotTimer = shotRate;
            burstTimer = burstDelay;
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                shooting = false;
            }
        }
    }

    void FireBullet()
    {
        ah.PlayGunShotSound(playerAudioSource);
        var newBullet = Instantiate(bulletPrefab, bulletHole.transform.position, Quaternion.identity);
        var bulletScript = newBullet.GetComponent<Bullet>();
        bulletScript.lifeTime = bulletLifeTime;
        bulletScript.dmg = damagePerBullet;
        bulletScript.botBullet = isbot;
        bulletScript.player = player;
        newBullet.GetComponent<Rigidbody2D>().AddForce(player.transform.up * bulletSpeed);
        newBullet.transform.rotation = player.transform.rotation;
    }


    public void Shoot(GameObject holdingPlace, GameObject player, bool isBot, AudioSource audioSource)
    {
        this.playerAudioSource = audioSource;
        this.bulletHole = holdingPlace;
        this.player = player;
        this.isbot = isBot;

        CalculateShotTime();
    }

}
