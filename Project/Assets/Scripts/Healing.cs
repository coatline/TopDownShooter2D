using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healing : MonoBehaviour
{
    public bool isShield;
    public int amount;

    public void Heal(Player player = null, Bot bot = null)
    {
        if (isShield)
        {
            if (player)
            {
                player.shield += amount;
            }
            else
            {
                bot.shield += amount;
            }
        }
        else
        {
            if (player)
            {
                player.health += amount;
            }
            else
            {
                bot.health += amount;
            }
        }
    }
}
