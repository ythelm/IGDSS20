using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    
    public int type; //building name

    public int upkeep;   //money cost per minute

    public int costMoney; //placement money cost

    public int planksCost; //placement planks cost

    public Tile tile; //reference to the tile it is build on

    public float efficiency; //calculated based on the surrounding tile types
    public float resourceGenerationInterval; //If operating at 100% efficiency, this is the time in seconds it takes for one production cycle to finish
    public float outputCount; // number of output resources per generation cycle

    public List<Tile.TileTypes> canBeBuiltOn; //restriction on which types of tiles it can be placed on
    public bool efficiencyScales; //Choice it its efficiency scales with a specific type of surrounding tile
    public int minNeighbors = 0;
    public int maxNeighbors = 6;
    public List<GameManager.ResourceTypes> inputResources; //Choice for input resource types (0,1,2)
    public GameManager.ResourceTypes outputResource;//Choice for output resource type





    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    
        
    }
}
