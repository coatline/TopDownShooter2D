using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMG : MonoBehaviour
{
    [SerializeField] Bullet bulletPrefab = null;
    public float burstDelay;
    public float shotRate;
    public Player player = null;
    Gun gun = null;
    float timer;

    private void Awake()
    {
        gun = GetComponent<Gun>();
    }

    public void CalculateShotTime()
    {
        if (timer >= shotRate)
        {
            timer = 0;
            Shoot();
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    void Shoot()
    {
        var newBullet = Instantiate(bulletPrefab, transform.Find("Bullet Hole").transform.position, Quaternion.identity);
        var bulletScript = newBullet.GetComponent<Bullet>();
        bulletScript.lifeTime = gun.bulletLifeTime;
        bulletScript.dmg = gun.damagePerBullet;
        bulletScript.player = player;
        newBullet.GetComponent<Rigidbody2D>().AddForce(transform.up * gun.bulletSpeed);
        newBullet.transform.rotation = transform.rotation;
    }
}
