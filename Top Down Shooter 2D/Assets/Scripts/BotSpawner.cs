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
        for (int i = 0; i < botcount; i++)
        {
            var newBot = Instantiate(bot, transform, false);
            newBot.GetComponent<Bot>().worldSpaceCanvas = worldSpaceCanvas;
            newBot.fallBarPrefab = fallbarprefab;
        }
    }

}
