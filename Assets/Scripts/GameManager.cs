﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Manager References
    public static GameManager Instance; //Singleton of this manager. Can be called with static reference GameManager.Instance
    private MouseManager _mouseManager; //Reference to MouseManager.Instance
    #endregion

    #region Map generation
    public Texture2D _heightMap; //Reference to the height map texture file
    public GameObject[] _tilePrefabs; //References to the tile prefabs
    public Transform _tileParentObject; //Reference to the parent object in the hierarchy for all spawned tiles
    private Tile[,] _tileMap; //2D array of all spawned tiles
    private float _heightFactor = 50; //Multiplier for placement of tiles on the Y-axis
    #endregion

    #region Buildings
    public GameObject[] _buildingPrefabs; //References to the building prefabs
    public int _selectedBuildingPrefabIndex = 0; //The current index used for choosing a prefab to spawn from the _buildingPrefabs list
    private List<Building> _buildings; //List of all currently spawned buildings. Used for upkeep in economy ticks
    #endregion

    #region Economy
    private float _economyTickRate = 60; //Every X seconds the economy will tick
    private float _economyTimer; //The current progress within an economy tick cycle
    public float _money = 50000; //The currently available money
    private float _IncomePerPerson = 1; //Each person of the population pays taxes in every economy tick. This amount will be decided by population happiness.
    #endregion

    #region Resources
    public int _population; //Number of people available. Currently only one tier of workers
    public int _maximumResourceCountInWarehouse = 100; //How much of each resource can be stored in the global warehouse
    private Dictionary<ResourceTypes, float> _resourcesInWarehouse = new Dictionary<ResourceTypes, float>(); //Holds a number of stored resources for every ResourceType


    //A representation of _resourcesInWarehouse, broken into individual floats. Only for display in inspector, will be removed and replaced with UI later
    [SerializeField]
    private float _ResourcesInWarehouse_Fish;
    [SerializeField]
    private float _ResourcesInWarehouse_Wood;
    [SerializeField]
    private float _ResourcesInWarehouse_Planks;
    [SerializeField]
    private float _ResourcesInWarehouse_Wool;
    [SerializeField]
    private float _ResourcesInWarehouse_Clothes;
    [SerializeField]
    private float _ResourcesInWarehouse_Potato;
    [SerializeField]
    private float _ResourcesInWarehouse_Schnapps;
    #endregion

    #region Enumerations
    public enum ResourceTypes { None, Fish, Wood, Planks, Wool, Clothes, Potato, Schnapps }; //Enumeration of all available resource types. Can be addressed from other scripts by calling GameManager.ResourceTypes
    #endregion

    #region MonoBehaviour
    //Awake is called when creating this object
    private void Awake()
    {
        if (Instance)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateMap();
        _mouseManager = MouseManager.Instance;
        _mouseManager.InitializeBounds(0, _heightMap.width * 10, 0, _heightMap.height * 8.66f);
        _buildings = new List<Building>();
        PopulateResourceDictionary();
    }

    // Update is called once per frame
    void Update()
    {
        HandleKeyboardInput();
        UpdateEconomyTimer();
        UpdateInspectorNumbersForResources();
    }
    #endregion

    #region Methods
    //Makes the resource dictionary usable by populating the values and keys
    void PopulateResourceDictionary()
    {
        _resourcesInWarehouse.Add(ResourceTypes.None, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Fish, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Wood, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Planks, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Wool, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Clothes, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Potato, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Schnapps, 0);
    }

    //Handles the progression within an economy cycle
    void UpdateEconomyTimer()
    {
        _economyTimer += Time.deltaTime;

        if (_economyTimer > _economyTickRate)
        {
            _economyTimer = 0;
            TickEconomy();
        }
    }

    //Sets the index for the currently selected building prefab by checking key presses on the numbers 1 to 0
    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _selectedBuildingPrefabIndex = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _selectedBuildingPrefabIndex = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _selectedBuildingPrefabIndex = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _selectedBuildingPrefabIndex = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            _selectedBuildingPrefabIndex = 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            _selectedBuildingPrefabIndex = 5;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            _selectedBuildingPrefabIndex = 6;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            _selectedBuildingPrefabIndex = 7;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            _selectedBuildingPrefabIndex = 8;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            _selectedBuildingPrefabIndex = 9;
        }
    }

    //Updates the visual representation of the resource dictionary in the inspector. Only for debugging
    void UpdateInspectorNumbersForResources()
    {
        _ResourcesInWarehouse_Fish = _resourcesInWarehouse[ResourceTypes.Fish];
        _ResourcesInWarehouse_Wood = _resourcesInWarehouse[ResourceTypes.Wood];
        _ResourcesInWarehouse_Planks = _resourcesInWarehouse[ResourceTypes.Planks];
        _ResourcesInWarehouse_Wool = _resourcesInWarehouse[ResourceTypes.Wool];
        _ResourcesInWarehouse_Clothes = _resourcesInWarehouse[ResourceTypes.Clothes];
        _ResourcesInWarehouse_Potato = _resourcesInWarehouse[ResourceTypes.Potato];
        _ResourcesInWarehouse_Schnapps = _resourcesInWarehouse[ResourceTypes.Schnapps];
    }

    //Instantiates individual hexagonal tile prefabs
    void GenerateMap()
    {
        _tileMap = new Tile[_heightMap.height, _heightMap.width];

        //Spawn tiles on grid
        for (int h = 0; h < _heightMap.height; h++)
        {
            for (int w = 0; w < _heightMap.width; w++)
            {

                Color c = _heightMap.GetPixel(w, h);
                float max = c.maxColorComponent;
                float tileHeight = _heightFactor * max;

                //Determine tile type
                GameObject selectedPrefab;

                if (max == 0.0f)
                {
                    selectedPrefab = _tilePrefabs[0];
                }
                else if (max < 0.2f)
                {
                    selectedPrefab = _tilePrefabs[1];
                }
                else if (max < 0.4f)
                {
                    selectedPrefab = _tilePrefabs[2];
                }
                else if (max < 0.6f)
                {
                    selectedPrefab = _tilePrefabs[3];
                }
                else if (max < 0.8f)
                {
                    selectedPrefab = _tilePrefabs[4];
                }
                else
                {
                    selectedPrefab = _tilePrefabs[5];
                }

                GameObject go = Instantiate(selectedPrefab, _tileParentObject);
                go.transform.position = new Vector3(h * 8.66f, tileHeight, w * 10f + (h % 2 == 0 ? 0f : 5f));
                Tile t = go.GetComponent<Tile>();
                t._coordinateHeight = h;
                t._coordinateWidth = w;
                _tileMap[h, w] = t;
            }
        }

        //Populate list of neighbors
        for (int h = 0; h < _tileMap.GetLength(0); h++)
        {
            for (int w = 0; w < _tileMap.GetLength(1); w++)
            {
                Tile t = _tileMap[h, w];
                t._neighborTiles = FindNeighborsOfTile(t);

            }
        }
    }

    //Returns a list of all neighbors of a given tile
    private List<Tile> FindNeighborsOfTile(Tile t)
    {
        List<Tile> result = new List<Tile>();

        int height = t._coordinateHeight;
        int width = t._coordinateWidth;

        //top, top-left, left, right, bottom, bottom-left
        //Check edge cases
        //top
        if (height > 0)
        {
            result.Add(_tileMap[height - 1, width]);
        }
        //bottom
        if (height < _heightMap.height - 1)
        {
            result.Add(_tileMap[height + 1, width]);
        }
        //left
        if (width > 0)
        {
            result.Add(_tileMap[height, width - 1]);
        }
        //right
        if (width < _heightMap.width - 1)
        {
            result.Add(_tileMap[height, width + 1]);
        }

        //if the column is even
        //top-left + bottom-left
        if (height % 2 == 0)
        {
            if (height > 0 && width > 0)
            {
                result.Add(_tileMap[height - 1, width - 1]);
            }
            if (height < _heightMap.height - 1 && width > 0)
            {
                result.Add(_tileMap[height + 1, width - 1]);
            }
        }
        //if the column is uneven
        //top-right + bottom-right
        else
        {
            if (height > 0 && width < _heightMap.width - 1)
            {
                result.Add(_tileMap[height - 1, width + 1]);
            }
            if (height < _heightMap.height - 1 && width < _heightMap.width - 1)
            {
                result.Add(_tileMap[height + 1, width + 1]);
            }
        }

        return result;
    }

    //Calculates money income and upkeep when an economy cycle is completed
    private void TickEconomy()
    {
        //income
        float income = 0;
        income = _population * _IncomePerPerson;

        _money += income;

        //upkeep
        float upkeep = 0;
        for (int i = 0; i < _buildings.Count; i++)
        {
            upkeep += _buildings[i]._upkeep;
        }

        _money -= upkeep;
    }

    //Is called by MouseManager when a tile was clicked
    //Forwards the tile to the method for spawning buildings
    public void TileClicked(int height, int width)
    {
        Tile t = _tileMap[height, width];
        print(t._type);

        PlaceBuildingOnTile(t);
    }

    //Checks if the currently selected building type can be placed on the given tile and then instantiates an instance of the prefab
    private void PlaceBuildingOnTile(Tile t)
    {
        //if there is a selected building prefab
        if (_selectedBuildingPrefabIndex < _buildingPrefabs.Length)
        {
            //check if building can be placed
            Building buildingType = _buildingPrefabs[_selectedBuildingPrefabIndex].GetComponent<Building>();

            if (t._building == null && _money >= buildingType._buildCostMoney && _resourcesInWarehouse[ResourceTypes.Planks] >= buildingType._buildCostPlanks && buildingType._canBeBuiltOnTileTypes.Contains(t._type))
            {
                GameObject go = Instantiate(_buildingPrefabs[_selectedBuildingPrefabIndex], t.transform);
                go.transform.position = t.transform.position;
                Building b = go.GetComponent<Building>();
                b._tile = t;
                t._building = b;
                _buildings.Add(b);
                _money -= b._buildCostMoney;
                _resourcesInWarehouse[ResourceTypes.Planks] -= b._buildCostPlanks;
            }

        }
    }

    //Adds the amount of the specified resource to the dictionary
    public void AddResourceToWarehouse(ResourceTypes resource, float amount)
    {
        if (_resourcesInWarehouse[resource] + amount > _maximumResourceCountInWarehouse)
        {
            _resourcesInWarehouse[resource] = _maximumResourceCountInWarehouse;
        }
        else
        {
            _resourcesInWarehouse[resource] += amount;
        }
    }

    //Subtracts the amount of the specified resource to the dictionary
    public void RemoveResourceFromWarehouse(ResourceTypes resource, float amount)
    {
        if (_resourcesInWarehouse[resource] - amount >= 0)
        {
            _resourcesInWarehouse[resource] -= amount;
        }
        else
        {
            _resourcesInWarehouse[resource] = 0;
        }
    }

    //Checks if there is at least one material for the queried resource type in the warehouse
    public bool HasResourceInWarehoues(ResourceTypes resource)
    {
        return _resourcesInWarehouse[resource] >= 1;
    }
    #endregion
}
