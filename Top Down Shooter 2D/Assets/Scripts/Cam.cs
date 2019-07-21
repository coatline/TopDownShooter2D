using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    [SerializeField] GameObject target;

    void Start()
    {
        if (target.tag == "Player")
        {
            //set cam in player for switching cam when killed
        }
    }

    void Update()
    {
        transform.position = target.transform.position - new Vector3(0, 0, 10);
    }
}
