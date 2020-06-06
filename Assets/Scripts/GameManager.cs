using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    #region Map generation
    public Texture2D heightMap;
    private int dim;
    public GameObject waterTile, sandTile, grassTile, forestTile, stoneTile, mountainTile; // initiate tile prefabs
    private Tile[,] _tileMap; //2D array of all spawned tiles
    #endregion

    #region Buildings
    public GameObject[] _buildingPrefabs; //References to the building prefabs
    public int _selectedBuildingPrefabIndex = 0; //The current index used for choosing a prefab to spawn from the _buildingPrefabs list
    #endregion


    #region Resources
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

    #region Enconomy
    private int _money; // initial money
    private int _income = 100; // constant income per economy tick
    private float _enconomyTickInterval = 3f;
    #endregion

    #region MonoBehaviour

    // awake is called before any Start functions
    void Awake()
    {
        GenerateMap();
    }

    // Start is called before the first frame update
    void Start()
    {
        PopulateResourceDictionary();
         //StartCoroutine("TickEconomy");
        // StartCoroutine("ProductionCycle");
    }

    // Update is called once per frame
    void Update()
    {
        HandleKeyboardInput();
        UpdateInspectorNumbersForResources();
    }
    #endregion

    #region Methods
    void GenerateMap()
    {
        dim = heightMap.width;
        _tileMap = new Tile[dim, dim];

        for (int i = 0; i < dim; i++)
            for (int j = 0; j < dim; j++)
            {
                float height = heightMap.GetPixel(i, j).r;
                GameObject spawnTile = null;
                Tile.TileTypes _spawnType;
                // We need bias to arrange hexagons over X axis
                int bias = i % 2 == 0 ? 0 : 5;
                // Choose the tile to spawn
                float magicNum = 8.66f;

                if (height == 0f)
                {
                    spawnTile = waterTile;
                    _spawnType = Tile.TileTypes.Water;
                }
                else if (height > 0f && height <= 0.2f)
                {
                    spawnTile = sandTile;
                    _spawnType = Tile.TileTypes.Sand;
                }
                else if (height > 0.2f && height <= 0.4f)
                {
                    spawnTile = grassTile;
                    _spawnType = Tile.TileTypes.Grass;
                }
                else if (height > 0.4f && height <= 0.6f)
                {
                    spawnTile = forestTile;
                    _spawnType = Tile.TileTypes.Forest;
                }
                else if (height > 0.6f && height <= 0.8f)
                {
                    spawnTile = stoneTile;
                    _spawnType = Tile.TileTypes.Stone;
                }
                else
                {
                    spawnTile = mountainTile;
                    _spawnType = Tile.TileTypes.Mountain;
                }
                // Spawn tile
                Instantiate(spawnTile,
                    new Vector3(i * magicNum, height * 10, j * 10 + bias),
                    new Quaternion(0f, 0f, 0f, 0f));

                Tile _tile = gameObject.AddComponent<Tile>();
                _tile._type = _spawnType;
                _tile._coordinateHeight = i;
                _tile._coordinateWidth = j;

                _tileMap[i, j] = _tile;
            }
    }

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

    //Checks if there is at least one material for the queried resource type in the warehouse
    public bool HasResourceInWarehoues(ResourceTypes resource)
    {
        return _resourcesInWarehouse[resource] >= 1;
    }

    //Is called by MouseManager when a tile was clicked
    //Forwards the tile to the method for spawning buildings
    public void TileClicked(int height, int width)
    {
        Tile t = _tileMap[height, width];

        PlaceBuildingOnTile(t);
    }

    //Checks if the currently selected building type can be placed on the given tile and then instantiates an instance of the prefab
    private void PlaceBuildingOnTile(Tile t)
    {
        //if there is building prefab for the number input
        if (_selectedBuildingPrefabIndex < _buildingPrefabs.Length)
        {
            //TODO: check if building can be placed and then istantiate it
            GameObject selectedBuilding = _buildingPrefabs[_selectedBuildingPrefabIndex];

            Building b = selectedBuilding.GetComponent<Building>() as Building;

            if (t._building == null && b.possibleTileTypes.Contains(t._type) && _money >= b.costMoney && _ResourcesInWarehouse_Planks >= b.planksCost)
            {
                GameObject building = Instantiate(selectedBuilding, t.gameObject.transform) as GameObject;
                b.InitializeBuilding(_selectedBuildingPrefabIndex, t);
                t._building = b;
                // Update money and planks because of the placement
                _money -= b.costMoney;
                _resourcesInWarehouse[ResourceTypes.Planks] -= b.planksCost;
                Debug.Log("Building is placed.");
            }
        }
    }

    //Returns a list of all neighbors of a given tile
    private List<Tile> FindNeighborsOfTile(Tile t)
    {
        List<Tile> result = new List<Tile>();

        //TODO: put all neighbors in the result list
        int w = _tileMap.GetLength(1) - 1;  // number of columns
        int h = _tileMap.GetLength(0) - 1; // number of rows
        int x = t._coordinateWidth;
        int y = t._coordinateHeight;
        bool isEven = y % 2 == 0;

        // Left
        if (x > 0) result.Add(_tileMap[y, x - 1]);
        // Right
        if (x < w) result.Add(_tileMap[y, x + 1]);
        // Down
        if (y > 0)
        {
            if (isEven && x > 0) result.Add(_tileMap[y - 1, x - 1]);
            if (!isEven && x < w) result.Add(_tileMap[y - 1, x + 1]);

            result.Add(_tileMap[y - 1, x]);
        }
        // Up
        if (y < h)
        {
            if (isEven && x > 0) result.Add(_tileMap[y + 1, x - 1]);
            if (!isEven && x < w) result.Add(_tileMap[y + 1, x + 1]);

            result.Add(_tileMap[y + 1, x]);
        }

        return result;
    }

    // tracking the Economy every 60 seconds
    // Subtract the sum of all building's upkeep cost from the money pool
    // addition: constant income of 100 units money/econmy tick added

    IEnumerator TickEconomy()
    {
        while (true)
        {
            _money += _income;
            foreach (Building b in FindObjectsOfType(typeof(Building)) as Building[])
                _money -= b.upkeep;
            Debug.Log("economy tracker");

            yield return new WaitForSeconds(_enconomyTickInterval);
        }
    }
    // Production and efficient
    IEnumerator ProductionCycle()
    {
        while (true)
        {
            foreach (Building b in FindObjectsOfType(typeof(Building)) as Building[])
            {
                // Update surrounding tiles and compute its current efficiency
                b._tile._neighborTiles = FindNeighborsOfTile(b._tile);
                b.UpdateEfficiency();
                // skip production
                if (b.efficiency == 0f) continue;
                // waitingt time
                yield return new WaitForSeconds(b.resourceGenerationInterval / b.efficiency);

                // Update resources in warehouse
                bool costResource = b.inputResource != ResourceTypes.None;
                if (costResource && HasResourceInWarehoues(b.inputResource))
                    _resourcesInWarehouse[b.inputResource] -= 1;
                _resourcesInWarehouse[b.outputResource] += b.outputCount;
            }
        }
    }
    #endregion
}