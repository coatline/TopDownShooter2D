using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour
{
    [SerializeField] Vector2[] startPositions;
    [SerializeField] Vector2[] endPositions;
    [SerializeField] float speed;
    Vector3 startPos;
    Vector3 endPos;

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
        if (transform.position == endPos)
        {
            print("Done!");
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, endPos, Time.deltaTime * speed);
    }
}
