using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class Cam : MonoBehaviour
{
    [SerializeField] GameObject target = null;
    TMP_Text playerCountText;
    TMP_Text killCounterText;
    float counterTimer;

    void Start()
    {
        var texts = FindObjectsOfType<TMP_Text>();

        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i].name == "PlayerCountText")
            {
                playerCountText = texts[i];
            }
            else if (texts[i].name == "KillCounterText")
            {
                killCounterText = texts[i];
            }
        }
    }

    void Update()
    {
        if ((Player.dead || Player.won) && Input.GetKeyDown(KeyCode.R))
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.buildIndex);
        }

        counterTimer += Time.deltaTime;

        if (counterTimer >= 0.5f)
        {
            counterTimer = 0;
            UpdateCounters();
        }
    }

    void UpdateCounters()
    {
        var player = FindObjectOfType<Player>();

        int alive = player ? 1 : 0;

        var bots = FindObjectsOfType<Bot>();

        for (int i = 0; i < bots.Length; i++)
        {
            if (!bots[i].dead)
            {
                alive++;
            }
        }

        if (playerCountText)
        {
            playerCountText.text = $"{alive}";
        }

        if (player && killCounterText)
        {
            killCounterText.text = $"{player.kills}";
        }

        if (player && alive == 1 && !Player.won && !Player.dead)
        {
            player.ShowWinUI();
        }
    }

    void LateUpdate()
    {
        if (target == null)
        {
            var boys = FindObjectsOfType<Bot>();

            if (boys.Length == 0) { return; }

            var alive = new List<Bot>();

            for (int i = 0; i < boys.Length; i++)
            {
                if (!boys[i].dead)
                {
                    alive.Add(boys[i]);
                }
            }

            if (alive.Count == 0)
            {
                target = boys[Random.Range(0, boys.Length)].gameObject;
            }
            else
            {
                target = alive[Random.Range(0, alive.Count)].gameObject;
            }
        }
        else
        {
            transform.position = target.transform.position - new Vector3(0, 0, 10);
        }
    }
}