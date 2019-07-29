using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCircle : MonoBehaviour
{
    [SerializeField] GameObject deathCircleInLine;
    [SerializeField] GameObject[] sides;
    public Vector3 targetScale;
    Vector3 beforeScale;
    List<Transform> sidePos;
    Vector3 targetPos;
    bool canStartNewPhase;
    bool canMove;

    void Start()
    {
        //sidePos = new List<Transform>();

        //for (int i = 0; i < 4; i++)
        //{
        //    sidePos.Add(transform.GetChild(i).transform);
        //}

        //for (int j = 0; j < sides.Length; j++)
        //{
        //    sides[j].transform.position = sidePos[j].transform.position;
        //}

        targetPos = transform.position;
        targetScale = transform.localScale;

        Begin();
    }

    void Update()
    {
        if ((Vector2.Distance(transform.localScale, targetScale) > 1f || Vector2.Distance(transform.position, targetPos) > .01f) && canMove)
        {
            transform.localScale -= targetScale / 500;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, .2f);

            //for (int j = 0; j < sides.Length; j++)
            //{
            //    sides[j].transform.position = sidePos[j].position;

            //    switch (sides[j].name)
            //    {
            //        case "right": sides[j].transform.localScale -= new Vector3(0, .01125f); break;
            //        case "left": sides[j].transform.localScale -= new Vector3(0, .01125f); break;
            //        case "up": sides[j].transform.localScale -= new Vector3(.01125f, 0); break;
            //        case "down": sides[j].transform.localScale -= new Vector3(.01125f, 0); break;

            //    }

            //}
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
            targetScale = transform.localScale / 3.5f;
        }
        else
        {
            targetScale = transform.localScale / 2f;
        }

        targetPos += new Vector3(Random.Range(-targetScale.x * 4, targetScale.x * 4), Random.Range(-targetScale.y * 4, targetScale.y * 4));
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
