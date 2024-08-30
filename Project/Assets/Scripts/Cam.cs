using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    [SerializeField] GameObject target = null;

    void Start()
    {
        if (target.tag == "Player")
        {
            //set cam in player for switching cam when killed
        }
    }

    void LateUpdate()
    {
        if (target == null)
        {
            var boy = FindObjectOfType<Bot>();
            if (!boy) { return; }
            target = boy.gameObject;
        }
        else
        {
            transform.position = target.transform.position - new Vector3(0, 0, 10);
        }
    }
}
