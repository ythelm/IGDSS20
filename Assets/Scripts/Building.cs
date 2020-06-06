using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    
    public BuildingType buildingType; //Building type

    public int upkeep;   //money cost per minute

    public int costMoney; //placement money cost

    public int planksCost; //placement planks cost

    public Tile tile; //reference to the tile it is build on

    public float efficiency; //calculated based on the surrounding tile types
    public float resourceGenerationInterval; //If operating at 100% efficiency, this is the time in seconds it takes for one production cycle to finish
    public float outputCount; // number of output resources per generation cycle

    public List<Tile.TileTypes> possibleTileTypes; //restriction on which types of tiles it can be placed on
    public Tile.TileTypes scalesWithNeighboringTiles = Tile.TileTypes.Empty; //choice if efficiency scales with a specific type of surrounding tile
    public int minNeighbors = 0;
    public int maxNeighbors = 6;
    public List<GameManager.ResourceTypes> inputResources; //Choice for input resource types (0,1,2)
    public GameManager.ResourceTypes outputResource;//Choice for output resource type

    


    //Based on the surrounding processes, the efficiency is generated
    public void generateEfficiency(List<Tile> neighborTiles)
    {
        if (scalesWithNeighboringTiles != Tile.TileTypes.Empty)
        {
            int fittingNeighbors = 0;
            foreach (Tile neighbor in neighborTiles)
            {
                if (neighbor._type == scalesWithNeighboringTiles)
                {
                    fittingNeighbors++;
                }
            }
            if (fittingNeighbors < minNeighbors)
            {
                efficiency = 0.0f;
            }
            else if (fittingNeighbors >= maxNeighbors)
            {
                efficiency = 1.0f;
            }
            else
            {
                efficiency = (float)fittingNeighbors / (float)maxNeighbors;
            }
        }
        else
        {
            efficiency = 1.0f;
        }

    }

        #region Enumerations
        public enum BuildingType { Empty, Fishery, Lumberjack, Sawmill, SheepFarm, FrameworkKnitters, PotatoFarm, SchnappsDistillery };
        #endregion




        

    
}
