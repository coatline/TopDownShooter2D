using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour
{
    [SerializeField] Vector2[] startPositions = null;
    [SerializeField] Vector2[] endPositions = null;
    [SerializeField] float speed = 0;
    Vector3 startPos;
    Vector3 endPos;
    bool done;

    void Start()
    {
        startPos = startPositions[Random.Range(0, startPositions.Length)];
        endPos = endPositions[Random.Range(0, endPositions.Length)];

        if (Random.Range(0, 2) == 0)
        {
            var sp = startPos;
            startPos = endPos;
            endPos = sp;
        }

        transform.position = startPos;

        Vector3 dir = startPos - endPos;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 90);

    }

    void Update()
    {
        if (transform.position == endPos && !done)
        {
            if (transform.childCount > 0)
            {
                transform.DetachChildren();
            }

            StartCoroutine(DoDie(2f));

            var player = FindObjectOfType<Player>();

            if (!player)
            {
                return;
            }

            if (!player.GetComponent<Player>().jumped)
            {
                player.GetComponent<Player>().Jump();
            }


            //Destroy(gameObject);
        }

        transform.position = Vector3.MoveTowards(transform.position, endPos, Time.deltaTime * speed);
    }

    IEnumerator DoDie(float delay)
    {
        done = true;
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}