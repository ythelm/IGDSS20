using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    
    public BuildingType _type; //Building type
    public int upkeep;   //money cost per minute
    public int costMoney; //placement money cost
    public int planksCost; //placement planks cost public Tile tile; //reference to the tile it is build on
    public Tile _tile; // Reference to the tile it is built on 

    public float efficiency; //calculated based on the surrounding tile types
    public float resourceGenerationInterval; //If operating at 100% efficiency, this is the time in seconds it takes for one production cycle to finish
    public float outputCount; // number of output resources per generation cycle

   
    public List<Tile.TileTypes> possibleTileTypes; //restriction on which types of tiles it can be placed on
    public bool scalesWithNeighboringTiles; // A choice if its efficiency scales with a specific type of surrounding tile
    public int minNeighbors = 0;
    public int maxNeighbors = 6;
    public GameManager.ResourceTypes inputResource; //Choice for input resource types (0,1,2)
    public GameManager.ResourceTypes outputResource;//Choice for output resource type

    #region Enumerations
    public enum BuildingType { Empty, Fishery, Lumberjack, Sawmill, SheepFarm, FrameworkKnitters, PotatoFarm, SchnappsDistillery };
    #endregion


    public void InitializeBuilding(int index, Tile t)
    {
        this._tile = t;
        this._type = (BuildingType)index + 1; // increment by 1 since the first item in BuildingType is Empty 
        this.possibleTileTypes = new List<Tile.TileTypes>();

        switch (this._type)
        {
            case BuildingType.Fishery:
                this.costMoney = 100;
                this.planksCost = 2;
                this.upkeep = 40;
                this.outputCount = 1;
                this.scalesWithNeighboringTiles = true;
                this.resourceGenerationInterval = 30f;
                this.possibleTileTypes.Add(Tile.TileTypes.Sand);
                this.minNeighbors = 1;
                this.maxNeighbors = 3;
                this.inputResource = GameManager.ResourceTypes.None;
                this.outputResource = GameManager.ResourceTypes.Fish;
                this.efficiency = ComputeEfficiency();
                break;
            case BuildingType.Lumberjack:
                this.costMoney = 100;
                this.planksCost = 0;
                this.upkeep = 10;
                this.outputCount = 1;
                this.scalesWithNeighboringTiles = true;
                this.resourceGenerationInterval = 15f;
                this.possibleTileTypes.Add(Tile.TileTypes.Forest);
                this.minNeighbors = 1;
                this.maxNeighbors = 6;
                this.inputResource = GameManager.ResourceTypes.None;
                this.outputResource = GameManager.ResourceTypes.Wood;
                this.efficiency = ComputeEfficiency();
                break;
            case BuildingType.Sawmill:
                this.costMoney = 100;
                this.planksCost = 0;
                this.upkeep = 10;
                this.outputCount = 2;
                this.efficiency = 1f;
                this.scalesWithNeighboringTiles = false;
                this.resourceGenerationInterval = 15f;
                this.possibleTileTypes.Add(Tile.TileTypes.Grass);
                this.possibleTileTypes.Add(Tile.TileTypes.Forest);
                this.possibleTileTypes.Add(Tile.TileTypes.Stone);
                this.inputResource = GameManager.ResourceTypes.Wood;
                this.outputResource = GameManager.ResourceTypes.Planks;
                break;
            case BuildingType.SheepFarm:
                this.costMoney = 100;
                this.planksCost = 2;
                this.upkeep = 20;
                this.outputCount = 1;
                this.scalesWithNeighboringTiles = true;
                this.resourceGenerationInterval = 30f;
                this.possibleTileTypes.Add(Tile.TileTypes.Grass);
                this.minNeighbors = 1;
                this.maxNeighbors = 4;
                this.inputResource = GameManager.ResourceTypes.None;
                this.outputResource = GameManager.ResourceTypes.Wood;
                this.efficiency = ComputeEfficiency();
                break;
            case BuildingType.FrameworkKnitters:
                this.costMoney = 400;
                this.planksCost = 20;
                this.upkeep = 50;
                this.outputCount = 1;
                this.efficiency = 1f;
                this.scalesWithNeighboringTiles = false;
                this.resourceGenerationInterval = 30f;
                this.possibleTileTypes.Add(Tile.TileTypes.Grass);
                this.possibleTileTypes.Add(Tile.TileTypes.Forest);
                this.possibleTileTypes.Add(Tile.TileTypes.Stone);
                this.inputResource = GameManager.ResourceTypes.Wood;
                this.outputResource = GameManager.ResourceTypes.Clothes;
                break;
            case BuildingType.PotatoFarm:
                this.costMoney = 100;
                this.planksCost = 2;
                this.upkeep = 20;
                this.outputCount = 1;
                this.scalesWithNeighboringTiles = true;
                this.resourceGenerationInterval = 30f;
                this.possibleTileTypes.Add(Tile.TileTypes.Grass);
                this.minNeighbors = 1;
                this.maxNeighbors = 4;
                this.inputResource = GameManager.ResourceTypes.None;
                this.outputResource = GameManager.ResourceTypes.Potato;
                this.efficiency = ComputeEfficiency();
                break;
            case BuildingType.SchnappsDistillery:
                this.costMoney= 100;
                this.planksCost = 2;
                this.upkeep = 40;
                this.outputCount = 1;
                this.efficiency = 1f;
                this.scalesWithNeighboringTiles = false;
                this.resourceGenerationInterval = 30f;
                this.possibleTileTypes.Add(Tile.TileTypes.Grass);
                this.possibleTileTypes.Add(Tile.TileTypes.Forest);
                this.possibleTileTypes.Add(Tile.TileTypes.Stone);
                this.inputResource = GameManager.ResourceTypes.Potato;
                this.outputResource = GameManager.ResourceTypes.Schnapps;
                break;
        }
    }
    float ComputeEfficiency()
    {
        if (this.scalesWithNeighboringTiles)
        {
            Tile.TileTypes tt = Tile.TileTypes.Empty;
            switch (this._type)
            {
                case BuildingType.Fishery:
                    tt = Tile.TileTypes.Water;
                    break;
                case BuildingType.Lumberjack:
                    tt = Tile.TileTypes.Forest;
                    break;
                case BuildingType.SheepFarm:
                case BuildingType.PotatoFarm:
                    tt = Tile.TileTypes.Grass;
                    break;
            }
            int surroundingTiles = this._tile._neighborTiles.FindAll(t => t._type == tt).Count; ;
            if (this.maxNeighbors <= surroundingTiles) return 1f;
            if (this.minNeighbors > surroundingTiles) return 0f;
            return surroundingTiles / this.maxNeighbors;
        }
        return 1f;
    }


    public void UpdateEfficiency()
    {
        this.efficiency = ComputeEfficiency();
    }


    //Based on the surrounding processes, the efficiency is generated
    /* public void generateEfficiency(List<Tile> neighborTiles)
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
        */
}



