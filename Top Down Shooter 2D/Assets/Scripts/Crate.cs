using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    ItemGenerator ig;
    SelfDestruct sd;

    private void Awake()
    {
        ig = FindObjectOfType<ItemGenerator>();
        sd = GetComponent<SelfDestruct>();
    }

    public void Open()
    {
        ig.GenerateItem(transform.position);
        sd.DoDie();
    }
}
