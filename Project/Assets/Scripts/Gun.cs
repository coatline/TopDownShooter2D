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
    [SerializeField] int bulletCount;
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
        ChooseRarity();
    }

    float burstTimer;
    int burstNumber;
    bool bursting;

    public void CalculateShotTime()
    {
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

    void FireBullet()
    {
        ah.PlayGunShotSound(playerAudioSource);

        for (int i = 1; i < bulletCount + 1; i++)
        {
            var newBullet = Instantiate(bulletPrefab, bulletHole.transform.position, player.transform.rotation);
            newBullet.transform.Rotate(new Vector3(0, 0, Random.Range(-aimError * i, aimError * i)));
            newBullet.GetComponent<Rigidbody2D>().AddForce(newBullet.transform.up * bulletSpeed);
            var bulletScript = newBullet.GetComponent<Bullet>();
            bulletScript.lifeTime = bulletLifeTime;
            bulletScript.dmg = damagePerBullet;
            bulletScript.botBullet = isbot;
            bulletScript.player = player;
        }
    }


    public void Shoot(GameObject bulletHole, GameObject player, bool isBot, AudioSource audioSource)
    {
        this.playerAudioSource = audioSource;
        this.bulletHole = bulletHole;
        this.player = player;
        this.isbot = isBot;

        CalculateShotTime();
    }


    void ChooseRarity()
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
}
