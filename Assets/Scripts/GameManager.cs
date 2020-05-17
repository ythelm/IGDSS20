using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    //initiate Level Objects and heightmap
    public Texture2D heightmap;
    public float heightscale = 10f;
    public GameObject waterTile, sandTile, grassTile, forestTile, stoneTile, mountainTile;
  
 
    void Start()
    {
        float multiplier = 10;
        float xmult = (5 * multiplier / 6f);

        float xLimit = ((heightmap.height - 1) * (5f / 6f) * multiplier) / 2f;
        float zLimit = ((heightmap.height - 0.5f) * multiplier) / 2f;


        //Add tiles to the heightmap
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


   //adding prefabs to the heightmap
    GameObject HeightToTile(float height)
    {
        if (height == 0)
        {
            return waterTile;
        }
        else if (height <= 0.2)
        {
            return sandTile;
        }
        else if (height <= 0.4)
        {
            return grassTile;
        }
       else if (height <= 0.6)
        {
            return forestTile;
        }
        else if (height <= 0.8)
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