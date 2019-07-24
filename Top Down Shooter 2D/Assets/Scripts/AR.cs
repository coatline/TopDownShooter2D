using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AR : MonoBehaviour
{
    [SerializeField] Bullet bulletPrefab;
    public float burstDelay;
    public float shotRate;
    float timer;
    Gun gun;

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
        var newBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        var bulletScript = newBullet.GetComponent<Bullet>();
        bulletScript.lifeTime = gun.bulletLifeTime;
        bulletScript.dmg = gun.damagePerBullet;
        newBullet.GetComponent<Rigidbody2D>().AddForce(transform.up * gun.bulletSpeed);
        newBullet.transform.rotation = transform.rotation;
    }
}
