using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : MonoBehaviour
{
    public int _upkeep; // money cost per minute
    public int _build_cost_money; // placement money cost
    public int _build_cost_planks; // placement planks cost
    public Tile _tile; // tile the building is built on

    public bool water_can_be_built_on;
    public bool sand_can_be_built_on;
    public bool grass_can_be_built_on;
    public bool forest_can_be_built_on;
    public bool stone_can_be_built_on;
    public bool mountain_can_be_built_on;

    public bool productionBuilding = false;

    public GameObject gameManager;

    protected virtual void Start()
    {
        gameManager = GameObject.Find("GameManager");
    }

    public bool CanBeBuiltOn(Tile.TileTypes tile)
    {
        if (tile == Tile.TileTypes.Water)
        {
            return water_can_be_built_on;
        }
        if (tile == Tile.TileTypes.Sand)
        {
            return sand_can_be_built_on;
        }
        if (tile == Tile.TileTypes.Grass)
        {
            return grass_can_be_built_on;
        }
        if (tile == Tile.TileTypes.Forest)
        {
            return forest_can_be_built_on;
        }
        if (tile == Tile.TileTypes.Stone)
        {
            return stone_can_be_built_on;
        }
        if (tile == Tile.TileTypes.Mountain)
        {
            return mountain_can_be_built_on;
        }
        return false;

    }

    #region Manager References
    JobManager _jobManager; //Reference to the JobManager
    #endregion
    
    #region Workers
    public List<Worker> _workers; //List of all workers associated with this building, either for work or living
    #endregion

    #region Jobs
    public List<Job> _jobs; // List of all available Jobs. Is populated in Start()
    #endregion
    

    #region Methods   
    public void WorkerAssignedToBuilding(Worker w)
    {
        _workers.Add(w);
    }

    public void WorkerRemovedFromBuilding(Worker w)
    {
        _workers.Remove(w);
    }
    #endregion
}
