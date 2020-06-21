using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingType
{
    string _typename;
    public int _upkeep; // money cost per minute
    public int _build_cost_money; // placement money cost
    public int _build_cost_planks; // placement planks cost
    private int _resource_generation_interval; // If operating at 100% efficiency, this is the time in seconds it takes for one production cycle to finish
    private int _output_count; // The number of output resources per generation cycle (for example the Sawmill produces 2 planks at a time)
    private List<Tile> _can_be_built_on; // A restriction on which types of tiles it can be placed on
    //private ListDictionary _efficiency_scale_with_neighbor_tile; // A choice if its efficiency scales with a specific type of surrounding tile
    private int _min_neighbors; // The minimum and maximum number of surrounding tiles its efficiency scales with (0-6)
    private int _max_neighbors; // The minimum and maximum number of surrounding tiles its efficiency scales with (0-6)

    // TODO these 2 probably shouldn't be a string
    private string input_ressource; // A choice for input resource types (0, 1 or 2 types)
    private string output_ressource; // A choice for output resource type

    public BuildingType(string typename)
    {
        //if typename == fishery{
        //    _upkeep = 100
        //}
    }
}
