using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationManager : MonoBehaviour
{
    private static Dictionary<Tile.TileTypes, int> _weightValues = new Dictionary<Tile.TileTypes, int>()
                                            {
                                                {Tile.TileTypes.Water,30},
                                                {Tile.TileTypes.Sand, 2},
                                                {Tile.TileTypes.Grass,1},
                                                {Tile.TileTypes.Forest,2},
                                                {Tile.TileTypes.Stone,1},
                                                {Tile.TileTypes.Mountain,3}
                                            };

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static int[,] generateMap(Tile tile, GameManager gameManager)
    {
        int[,] pathFindingMap = new int[gameManager._heightMap.height, gameManager._heightMap.width];

        // each tile is assigned a weight value, depending on its type
        for (int h = 0; h < gameManager._heightMap.height; h++)
        {
            for (int w = 0; w < gameManager._heightMap.width; w++)
            {
                pathFindingMap[h, w] = _weightValues[gameManager._tileMap[h, w]._type];
            }
        }

        // The tile the building is standing on however has a weight value of 0. 
        pathFindingMap[tile._coordinateHeight, tile._coordinateWidth] = 0;

        // From there, recursively take all neighboring tiles and set their weight to the total weight up to this point plus their own weight value. This simulates the ease of travel each tile type allows.
        List<(int, int)> alreadyVisited = new List<(int, int)> 
        {
            (tile._coordinateHeight, tile._coordinateWidth)
        }; // last visited tiles
        List<(int, int)> tilesUpNext = determineNeighbors((tile._coordinateHeight, tile._coordinateWidth), gameManager); // tiles visited next

        while (tilesUpNext.Count > 0)
        {
            List<(int, int)> tilesUpNextNew = new List<(int, int)>();

            foreach ( (int, int) node in tilesUpNext )
            {
                List<(int, int)> neighbors = determineNeighbors(node, gameManager);
                List<(int, int)> visitedNeighbors = new List<(int, int)>();

                foreach ((int, int) neighbor in neighbors) // determine visited and unvisited neighbors
                {
                    if (containsField((neighbor.Item1, neighbor.Item2), alreadyVisited))
                    {
                        visitedNeighbors.Add((neighbor.Item1, neighbor.Item2));
                    }
                    else {
                        if (!containsField((neighbor.Item1, neighbor.Item2), tilesUpNextNew))
                        {
                            tilesUpNextNew.Add((neighbor.Item1, neighbor.Item2));
                        }
                    }
                }

                if (visitedNeighbors.Count > 1) // use path with less weight
                {
                    pathFindingMap[node.Item1, node.Item2] = pathFindingMap[node.Item1, node.Item2] + Math.Min(pathFindingMap[visitedNeighbors[0].Item1, visitedNeighbors[0].Item2], pathFindingMap[visitedNeighbors[1].Item1, visitedNeighbors[1].Item2]);   
                }
                else if (visitedNeighbors.Count == 1) {
                    pathFindingMap[node.Item1, node.Item2] = pathFindingMap[node.Item1, node.Item2] + pathFindingMap[visitedNeighbors[0].Item1, visitedNeighbors[0].Item2];
                }
            }

            foreach ((int, int) oldNext in tilesUpNext)
            {
                alreadyVisited.Add((oldNext.Item1, oldNext.Item2));
                tilesUpNextNew.Remove((oldNext.Item1, oldNext.Item2));
            }

            tilesUpNext = tilesUpNextNew;
        }

        return pathFindingMap;
    }

    private static List<(int, int)> determineNeighbors((int, int) field, GameManager gameManager)
    {
        List<(int, int)> list = new List<(int, int)>();
        list.Add((-1,0));
        list.Add((1,0));
        list.Add((0,-1));
        list.Add((0,1));

        if (field.Item1 % 2 == 0)
        {
            list.Add((1,-1));
            list.Add((-1,-1));  
        }
        else
        {
            list.Add((1,1));
            list.Add((-1,1)); 
        }

        List<(int, int)> neighbors = new List<(int, int)>();

        foreach ((int,int) x in list)
        {
            int nh = field.Item1+x.Item1; // height coordinate of neighbor
            int nw = field.Item2+x.Item2; // width coordinate of neighbor
            if (nh >= 0 && nh < gameManager._heightMap.height && nw >= 0 && nw < gameManager._heightMap.width) // if neighbor exists
            {
                neighbors.Add((nh, nw));   
            }
        }

        return neighbors;   
    }

    private static bool containsField((int, int) field, List<(int, int)> list)
    {
        foreach ((int, int) el in list)
        {
            if (field.Item1 == el.Item1 && field.Item2 == el.Item2)
            {
                return true;
            }
        }
        return false;  
    }
}
