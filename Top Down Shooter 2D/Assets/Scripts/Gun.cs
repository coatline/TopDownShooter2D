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
        else if(gunrarity == "Epic")
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
