using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //create heightmap
    public Texture2D heightmap;

    //heightscaling
    public float heightscale = 10f;

    //GameObjects
    public GameObject waterTile;
    public GameObject sandTile;
    public GameObject grassTile;
    public GameObject forestTile;
    public GameObject stoneTile;
    public GameObject mountainTile;

  

    void Start()
    {
        float multiplier = 10;
        float xmult = (5 * multiplier / 6f);

        float xLimit = ((heightmap.height - 1) * (5f / 6f) * multiplier) / 2f;
        float zLimit = ((heightmap.height - 0.5f) * multiplier) / 2f;

        //Set Tiles
        for (int x = 0; x < heightmap.width; x++)
        {
            for (int z = 0; z < heightmap.height; z++)
            {
                float height = heightmap.GetPixel(x - 1, z - 1).grayscale;
                float xposition = -xLimit + (x) * xmult;
                float zposition = -zLimit + z * multiplier + ((multiplier / 2f) * (x % 2));
                Instantiate(HeightToTile(height), new Vector3(xposition, heightscale * height, zposition), Quaternion.identity);
            }
        }
    }


    // Returns a Prefab depending on given height
    GameObject HeightToTile(float height)
    {
        if (height == 0)
        {
            return waterTile;
        }
        if (height <= 0.2)
        {
            return sandTile;
        }
        if (height <= 0.4)
        {
            return grassTile;
        }
        if (height <= 0.6)
        {
            return forestTile;
        }
        if (height <= 0.8)
        {
            return stoneTile;
        }
        return mountainTile;
    }

    // Update is called once per frame
    void Update()
    {

    }
}