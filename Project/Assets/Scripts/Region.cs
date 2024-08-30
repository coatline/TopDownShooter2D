using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Region : MonoBehaviour
{
    //[SerializeField] string names;
    [SerializeField] GameObject[] buildingPrefabs;
    public int buildingNum;
    public Vector2 size;

    void Awake()
    {
        for (int i = 0; i < buildingNum; i++)
        {
            Instantiate(buildingPrefabs[Random.Range(0, buildingPrefabs.Length)], BuildingPosition(), Quaternion.identity, transform);
        }
    }

    Vector3 BuildingPosition()
    {
        return new Vector3(Random.Range(transform.position.x + Random.Range(-size.x / 2, size.x / 2), transform.position.y + Random.Range(-size.y / 2, size.y / 2)), 0);
    }
}
