using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{


    public float economyWaitTime = 60.0f;
    private float timer = 0.0f;
    #region Map generation
    private Tile[,] _tileMap; //2D array of all spawned tiles
    #endregion

    #region Buildings
    public GameObject fishery, lumberjack, sheepfarm, frameworkknitters, potatofarm, schnappsdistillery, sawmill;
    public List<GameObject> _buildingPrefabs; //References to the building prefabs
    public int _selectedBuildingPrefabIndex = 0; //The current index used for choosing a prefab to spawn from the _buildingPrefabs list
    private List<Building> _buildings = new List<Building>();
    #endregion


    #region Resources
    public int money;
    public Dictionary<ResourceTypes, float> _resourcesInWarehouse = new Dictionary<ResourceTypes, float>(); //Holds a number of stored resources for every ResourceType

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


    public GameObject water_tile, sand_tile, mountain_tile, forest_tile, grass_tile, stone_tile;



    public float height_factor = 20f;
    // Start is called before the first frame update
    
    float x_step = 17.321f;
    float y_step = 5f;
    float line_offset = 8.661f;
    Texture2D heightmap;
    void Start()
    {
        heightmap = Resources.Load("Heightmap_16", typeof(Texture2D)) as Texture2D; //Resources.Load("Assets/Textures/Heightmap_16") as Texture2D;
        //Texture2D heightmap = (Texture2D)Resources.LoadAssetAtPath("Assets/Textures/Heightmap_16", typeof(Texture2D));
        int x, y;
        _buildingPrefabs.Add(fishery);
        _buildingPrefabs.Add(lumberjack);
        _buildingPrefabs.Add(sheepfarm);
        _buildingPrefabs.Add(frameworkknitters);
        _buildingPrefabs.Add(potatofarm);
        _buildingPrefabs.Add(schnappsdistillery);
        _buildingPrefabs.Add(sawmill);

        PopulateResourceDictionary();

        _tileMap = new Tile[heightmap.width, heightmap.height];
        // Loop through the images pixels to reset color.
        for (x = 0; x < heightmap.width; x++)
        {
            for (y = 0; y < heightmap.height; y++)
            {
                Color pixelColor = heightmap.GetPixel(x, y);
                Vector3 pos = new Vector3(y % 2 * line_offset + x * x_step, pixelColor.b * height_factor, y * y_step);
                GameObject go;
                if (pixelColor.b < 0.01)
                {

                    go = Instantiate(water_tile, pos, Quaternion.identity);
                }
                else if (pixelColor.b < 0.2)
                {
                    go = Instantiate(sand_tile, pos, Quaternion.identity);
                }
                else if (pixelColor.b < 0.4)
                {
                    go = Instantiate(grass_tile, pos, Quaternion.identity);
                }
                else if (pixelColor.b < 0.6)
                {
                    go = Instantiate(forest_tile, pos, Quaternion.identity);
                }
                else if (pixelColor.b < 0.8)
                {
                    go = Instantiate(stone_tile, pos, Quaternion.identity);
                }
                else
                {
                    go = Instantiate(mountain_tile, pos, Quaternion.identity);
                }

                _tileMap[x, y] = go.GetComponent<Tile>() as Tile;
                _tileMap[x, y]._coordinateWidth = x;
                _tileMap[x, y]._coordinateHeight = y;
            }
        }

        for (x = 0; x < heightmap.width; x++)
        {
            for (y = 0; y < heightmap.height; y++)
            {
                _tileMap[x, y]._neighborTiles = FindNeighborsOfTile(_tileMap[x, y]);
            }
        }
    }

    #region MonoBehaviour
    // Update is called once per frame
    void Update()
    {
        HandleKeyboardInput();
        UpdateInspectorNumbersForResources();
        timer += Time.deltaTime;

        if (timer > economyWaitTime)
        {
            EconomyCycle();
            timer = timer - economyWaitTime;
        }
    }
    #endregion

    #region Methods
    //Makes the resource dictuionary usable by populating the values and keys
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
    public void TileClicked(int width, int height)
    {
        Tile t = _tileMap[width, height];

        PlaceBuildingOnTile(t);
    }

    //Checks if the currently selected building type can be placed on the given tile and then instantiates an instance of the prefab
    private void PlaceBuildingOnTile(Tile t)
    {
        //if there is building prefab for the number input
        if (_selectedBuildingPrefabIndex < _buildingPrefabs.Count && t._building == null && _buildingPrefabs[_selectedBuildingPrefabIndex].GetComponent<Building>().CanBeBuiltOn(t._type) && CheckResourcesForBuilding())
        {
            int x = t._coordinateWidth;
            int y = t._coordinateHeight;
            Color pixelColor = heightmap.GetPixel(x, y);
            Vector3 pos = new Vector3(y % 2 * line_offset + x * x_step, pixelColor.b * height_factor, y * y_step);
            GameObject go = Instantiate(_buildingPrefabs[_selectedBuildingPrefabIndex], pos, Quaternion.identity);
            Building b = go.GetComponent<Building>();
            t._building = b;
            b._tile = t;
            if (b.productionBuilding)
            {
                ((ProductionBuilding) b).calc_efficiency();
            }
            // Building currentBuilding = _buildingPrefabs[_selectedBuildingPrefabIndex].GetComponent<Building>();
            _resourcesInWarehouse[ResourceTypes.Planks] -= b._build_cost_planks;
            money -= b._build_cost_money;
            _buildings.Add(b);
        }
    }

    private void EconomyCycle()
    {
        money += 100;
        PayUpkeep();
    }

    private void PayUpkeep()
    {
        foreach(Building b in _buildings)
        {
            money -= b._upkeep;
        }
    }

    private bool CheckResourcesForBuilding()
    {
        Building currentBuilding = _buildingPrefabs[_selectedBuildingPrefabIndex].GetComponent<Building>();
        return (money >= currentBuilding._build_cost_money && _resourcesInWarehouse[ResourceTypes.Planks] >= currentBuilding._build_cost_planks);
    }

    //Returns a list of all neighbors of a given tile
    private List<Tile> FindNeighborsOfTile(Tile t)
    {
        List<Tile> result = new List<Tile>();
        int x = t._coordinateWidth;
        int y = t._coordinateHeight;

        int xSize = _tileMap.GetLength(0);
        int ySize = _tileMap.GetLength(1);

        if (y - 1 >= 0)
        {
            result.Add(_tileMap[x, y - 1]);
        }
        if(y + 1 < ySize)
        {
            result.Add(_tileMap[x, y + 1]);
        }

        if (y - 2 >= 0)
        {
            result.Add(_tileMap[x, y - 2]);
        }
        if (y + 2 < ySize)
        {
            result.Add(_tileMap[x, y + 2]);
        }

        // if height even :  width - 1
        // if height odd : width + 1
        bool even = (y % 2) == 0;

        if(even && (x - 1 >= 0))
        {
            if (y - 1 >= 0)
            {
                result.Add(_tileMap[x - 1, y - 1]);
            }
            if (y + 1 < ySize)
            {
                result.Add(_tileMap[x - 1, y + 1]);
            }
        }

        if(!even && (x + 1 < xSize))
            {
                if (y - 1 >= 0)
                {
                    result.Add(_tileMap[x + 1, y - 1]);
                }
                if (y + 1 < ySize)
                {
                    result.Add(_tileMap[x + 1, y + 1]);
            }


        }

        return result;
    }
    #endregion
}
