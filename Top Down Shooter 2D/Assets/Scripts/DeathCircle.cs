using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCircle : MonoBehaviour
{
    [SerializeField] GameObject deathCircleInLine;
    public Vector3 targetScale;
    Vector3 targetPos;
    bool canStartNewPhase;
    bool canMove;

    void Start()
    {
        targetPos = transform.position;
        targetScale = transform.localScale;

        Begin();
    }

    void Update()
    {
        if (Vector2.Distance(transform.localScale, targetScale) > .01f && canMove)
        {
            transform.localScale -= targetScale / 500;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, 1f);
        }
        else if (canStartNewPhase)
        {
            Begin();
        }
    }

    void Begin()
    {
        SetValues();
        Invoke("NewPhase", Random.Range(3f, 5f));
        canStartNewPhase = false;
    }

    void SetValues()
    {
        if (targetScale == transform.localScale)
        {
            targetScale = transform.localScale / 7f;
        }
        else
        {
            targetScale = transform.localScale / 2f;
        }

        targetPos += new Vector3(Random.Range(-targetScale.x / 2, targetScale.x / 2), Random.Range(-targetScale.y / 2, targetScale.y / 2));
        deathCircleInLine.transform.position = targetPos;
        deathCircleInLine.transform.localScale = targetScale;
        canMove = false;
    }

    void NewPhase()
    {
        canStartNewPhase = true;
        canMove = true;
    }
}
