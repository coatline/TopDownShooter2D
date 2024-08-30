using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using UnityEditor;

public class WorldGenerator : MonoBehaviour
{
    [Range(0.1f, 200)]
    [SerializeField] float noiseScale;
    [Range(0, 1)]
    [SerializeField] float waterThreshold;
    [Range(0, 1)]
    [SerializeField] float dirtThreshold;

    [SerializeField] GameObject cratePrefab;
    [SerializeField] Tilemap tilemap;
    [SerializeField] Tile grassTile;
    [SerializeField] Tile waterTile;
    [SerializeField] Tile dirtTile;

    [SerializeField] int worldHeight;
    [SerializeField] int worldWidth;
    [SerializeField] int crateCount;
    [SerializeField] int pathCount;

    //[Range(0, 1)]
    //[SerializeField] float grassThreshold = 0.8f;
    List<GameObject> crates;

    void OnEnable()
    {
        GenerateWorld();
    }

    void GenerateWorld()
    {
        Initiate();
        GenerateLand();
        //GenerateAreas();
        GenerateCrates();
        //GeneratePaths();
    }

    void Initiate()
    {
        ClearMap();
        tilemap.size = new Vector3Int(worldWidth, worldHeight, 0);
        tilemap.transform.position = new Vector3(-worldWidth / 2, -worldHeight / 2);

        crates = new List<GameObject>();
    }

    void GenerateLand()
    {
        int offsetX = Random.Range(0, 100000);
        int offsety = Random.Range(0, 100000);

        for (int y = 0; y < worldHeight; y++)
        {
            for (int x = 0; x < worldWidth; x++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), GetTileType(offsetX + x, offsety + y));
            }
        }
    }

    void GeneratePaths()
    {
        for (int i = 0; i < pathCount; i++)
        {
            GeneratePath();
        }
    }

    void GeneratePath()
    {
        int startingX = Random.Range(-worldWidth / 2, worldWidth / 2);
        int endingX = Random.Range(startingX, startingX + Mathf.Abs((worldWidth / 2) - startingX));
        int startingY = Random.Range(-worldHeight / 2, worldHeight / 2);
        int endingY = Random.Range(startingY, startingY + Mathf.Abs((worldHeight / 2) - startingY));

        for (int x = startingX; x < endingX; x++)
        {
            for (int y = startingY; y < endingY; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y + Random.Range(-x, x), 0), dirtTile);
            }
        }
    }

    void GenerateCrates()
    {
        for (int i = 0; i < crateCount; i++)
        {
            var newCrate = Instantiate(cratePrefab, new Vector3(Random.Range(-worldWidth / 2, worldWidth / 2), Random.Range(-worldHeight / 2, worldHeight / 2), 0), Quaternion.identity, transform);
            crates.Add(newCrate);
        }

        for (int i = crates.Count - 1; i >= 0; i--)
        {
            for (int j = 0; j < crates.Count; j++)
            {
                if (crates[i] == crates[j] || crates[i] == null) { continue; }

                if (crates[i].transform.position.x == crates[j].transform.position.x)
                {
                    Destroy(crates[i]);
                }
            }
        }
    }

    Tile GetTileType(int x, int y)
    {
        var val = Mathf.PerlinNoise((float)(x * noiseScale / worldHeight), (float)(y * noiseScale / worldHeight));

        if (val < waterThreshold)
        {
            return waterTile;
        }
        else if (val < dirtThreshold)
        {
            return dirtTile;
        }
        else
        {
            return grassTile;
        }
    }

    void ClearMap()
    {
        tilemap.ClearAllTiles();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            ClearMap();
            GenerateLand();
        }
    }
}
