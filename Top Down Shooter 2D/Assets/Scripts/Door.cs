using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] float openSpeed;
    BoxCollider2D bc;
    Rigidbody2D rb;
    HingeJoint2D hj;
    bool closing;
    bool open;

    void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
        hj = GetComponent<HingeJoint2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Interact(Transform playerTransform)
    {
        if (open)
        {
            Close();
            return;
        }

        if (playerTransform.position.y > transform.position.y)
        {
            Open(1);
        }
        else
        {
            Open(-1);
        }
    }

    void Close()
    {
        closing = true;

        hj.useMotor = true;

        JointMotor2D jm = hj.motor;

        jm.motorSpeed = openSpeed * Mathf.Sign(-hj.jointAngle);

        hj.motor = jm;

        open = false;
    }

    void Open(int dir)
    {
        hj.useMotor = true;
        JointMotor2D jm = hj.motor;
        jm.motorSpeed = openSpeed * dir;
        hj.motor = jm;
        open = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    void Update()
    {
        if (closing)
        {
            if (Mathf.Abs(hj.jointAngle - 0) < 4)
            {
                hj.useMotor = false;
                closing = false;
                bc.enabled = true;
                rb.bodyType = RigidbodyType2D.Static;
            }
            else
            {
                bc.enabled = false;
            }
        }

        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    Close();
        //}

        //if (Input.GetKeyDown(KeyCode.O))
        //{
        //    Open(1);
        //}
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    Open(-1);
        //}
    }
}
