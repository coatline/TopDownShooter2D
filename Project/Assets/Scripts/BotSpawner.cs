using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BotSpawner : MonoBehaviour
{
    [SerializeField] Image fallbarprefab;
    [SerializeField] Canvas worldSpaceCanvas;
    [SerializeField] int botcount = 99;
    [SerializeField] Bot bot;

    void Start()
    {
        var botHolder = new GameObject("Bot Holder");

        for (int i = 0; i < botcount; i++)
        {
            var newBot = Instantiate(bot, transform, false);
            var script = newBot.GetComponent<Bot>();
            script.worldSpaceCanvas = worldSpaceCanvas;
            script.botHolder = botHolder.transform;
            newBot.fallBarPrefab = fallbarprefab;
        }
    }

}
