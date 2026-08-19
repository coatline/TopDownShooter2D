using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healing : MonoBehaviour
{
    public bool isShield;
    public int amount;

    public bool Heal(Player player = null, Bot bot = null)
    {
        if (player)
        {
            return player.Heal(amount, isShield);
        }
        else
        {
            if (isShield)
            {
                if (bot.shield >= 100) { return false; }
                bot.shield += amount;
                if (bot.shield > 100) { bot.shield = 100; }
            }
            else
            {
                if (bot.health >= 100) { return false; }
                bot.health += amount;
                if (bot.health > 100) { bot.health = 100; }
            }

            return true;
        }
    }
}
