using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float bulletLifeTime;
    public int damagePerBullet;
    public float bulletSpeed;
    public string gunType; //Shotgun, AR, 
    public float aimError;
    //public float ammo;

    private void Start()
    {
        var gunrarity = GetComponent<Item>().rarity;

        if (gunrarity == "Common")
        {
            //stay the same?
        }
        else if (gunrarity == "Uncommon")
        {
            damagePerBullet++;
        }
        else if (gunrarity == "Rare")
        {
            damagePerBullet += 2;
        }
        else if (gunrarity == "Legendary")
        {
            damagePerBullet += 4;
        }
        else
        {
            damagePerBullet += 10;
        }
    }
}
