using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileStatsHolder : MonoBehaviour
{
    private enum MoistureCategory { Dry, MidWet, Wet }
    private enum TemperatureCategory { Cold, MidTemp, Hot }

    public enum BiomeCategory
    {
        ColdDry, ColdMidwet, ColdWet,
        MidtempDry, MidtempMidwet, MidtempWet,
        HotDry, HotMidwet, HotWet, Unassigned
    }

    Vector2Int _north = new Vector2Int(0, 1);
    Vector2Int _south = new Vector2Int(0, -1);
    Vector2Int _east = new Vector2Int(1, 0);
    Vector2Int _west = new Vector2Int(-1, 0);

    public static TileStatsHolder Instance;
    Grid _grid;

    //settings
    [SerializeField][Range(10, 100)] int _tileDimension = 10;
    public int TilemapDimensions => _tileDimension;
    [SerializeField] float _startingValue = 0.5f;

    [Header("Temperature/Moisture Thresholds")]
    [SerializeField] float _dryThreshold = 0.3f;
    [SerializeField] float _wetThreshold = 0.7f;
    [SerializeField] float _coldThreshold = 0.3f;
    [SerializeField] float _hotThreshold = 0.7f;
    [SerializeField] float _hillThreshold = 0.5f;
    [SerializeField] float _mountainThreshold = 0.7f;

    [SerializeField] float _treeClumpingFactor = 1f;


    //state
    public int Dimension => _tileDimension;

    float[,] _temperatureMap;
    float[,] _moistureMap;
    float[,] _elevationMap;
    float[,] _trafficMap;
    float[,] _vegetationMap;
    float[,] _habitabilityMap;
    float[,] _waterMap;
    BiomeCategory[,] _biomeMap;


    private void Awake()
    {
        Instance = this;
        PrepareMaps();
    }

    private void PrepareMaps()
    {
        _temperatureMap = new float[_tileDimension, _tileDimension];
        _moistureMap = new float[_tileDimension, _tileDimension];
        _elevationMap = new float[_tileDimension, _tileDimension];
        _trafficMap = new float[_tileDimension, _tileDimension];
        _vegetationMap = new float[_tileDimension, _tileDimension];
        _habitabilityMap = new float[_tileDimension, _tileDimension];
        _waterMap = new float[_tileDimension, _tileDimension];
        _biomeMap = new BiomeCategory[_tileDimension, _tileDimension];
        _grid = GetComponent<Grid>();
    }

    private void Start()
    {
        ResetMaps();
    }

    private void ResetMaps()
    {
        Array.Clear(_temperatureMap, 0, _tileDimension);
        Array.Clear(_moistureMap, 0, _tileDimension);
        Array.Clear(_elevationMap, 0, _tileDimension);
        Array.Clear(_trafficMap, 0, _tileDimension);
        Array.Clear(_vegetationMap, 0, _tileDimension);
        Array.Clear(_habitabilityMap, 0, _tileDimension);
        Array.Clear(_waterMap, 0, _tileDimension);
        Array.Clear(_biomeMap, 0, _tileDimension);

        Vector3Int coord = new Vector3Int(0, 0, 0);
        for (int x = 0; x < _tileDimension; x++)
        {
            for (int y = 0; y < _tileDimension; y++)
            {
                coord.x = x;
                coord.y = y;
                _temperatureMap[x, y] = _startingValue;
                _moistureMap[x, y] = _startingValue;
                _elevationMap[x, y] = _startingValue;

                _trafficMap[x, y] = 0;
                _vegetationMap[x, y] = 0;
                _habitabilityMap[x, y] = 0;
                _waterMap[x, y] = 0;
                _biomeMap[x, y] = BiomeCategory.MidtempMidwet;
            }
        }
    }



    #region Modify Data Maps at Coords
    public void ModifyTemperatureAtTile(int xCoord, int yCoord, float temperatureChange)
    {
        _temperatureMap[xCoord, yCoord] += temperatureChange;
    }

    public void SetTemperatureAtTile(int xCoord, int yCoord, float temperature)
    {
        _temperatureMap[xCoord, yCoord] = temperature;
    }

    public void ModifyMoistureAtTile(int xCoord, int yCoord, float moistureChange)
    {
        _moistureMap[xCoord, yCoord] += moistureChange;
    }

    public void SetMoistureAtTile(int xCoord, int yCoord, float moisture)
    {
        _moistureMap[xCoord, yCoord] = moisture;
    }

    /// <summary>
    /// Returns TRUE if the test coordinate or its neighbors already
    /// have a stream. Previous coordinate of stream is supplied to
    /// prevent this check from returning false from the last iteration.
    /// </summary>



    public void ModifyElevationAtTile(int xCoord, int yCoord, float elevationChange)
    {
        _elevationMap[xCoord, yCoord] += elevationChange;
    }

    public void SetElevationAtTileToMountainValue(int xCoord, int yCoord)
    {
        _elevationMap[xCoord, yCoord] = _mountainThreshold;
    }
    public void SetElevationAtTile(int xCoord, int yCoord, float elevation)
    {
        _elevationMap[xCoord, yCoord] = elevation;
    }

    public void SetPopulationAtTile(int xCoord, int yCoord, float newPopulation)
    {
        _habitabilityMap[xCoord, yCoord] = newPopulation;
    }


    public void ModifyPopulationAtTile(int xCoord, int yCoord, float populationChange)
    {
        _habitabilityMap[xCoord, yCoord] += populationChange;
    }

    public void ModifyTrafficAtTile(int xCoord, int yCoord, float trafficChange)
    {
        _trafficMap[xCoord, yCoord] += trafficChange;
    }

    internal void EnforceDeepWaterWithWaterAsNeighbor(int xCoord, int yCoord)
    {
        //if (xCoord + 1 < _tileDimension)
        //{
        //    _elevationMap[xCoord + 1, yCoord] = Mathf.Clamp(
        //        _elevationMap[xCoord + 1, yCoord],
        //        0, TileStatsRenderer.Instance.WaterThreshold);
        //}

        //if (xCoord - 1 >= 0)
        //{
        //    _elevationMap[xCoord - 1, yCoord] = Mathf.Clamp(
        //        _elevationMap[xCoord - 1, yCoord],
        //        0, TileStatsRenderer.Instance.WaterThreshold);
        //}

        //if (yCoord + 1 < _tileDimension)
        //{
        //    _elevationMap[xCoord, yCoord + 1] = Mathf.Clamp(
        //        _elevationMap[xCoord, yCoord + 1],
        //        0, TileStatsRenderer.Instance.WaterThreshold);
        //}

        //if (yCoord - 1 >= 0)
        //{
        //    _elevationMap[xCoord, yCoord - 1] = Mathf.Clamp(
        //        _elevationMap[xCoord, yCoord - 1],
        //        0, TileStatsRenderer.Instance.WaterThreshold);
        //}

        //if (_elevationMap.ContainsKey(coords + _south))
        //{
        //    _elevationMap[coords + _south] = Mathf.Clamp(
        //        _elevationMap[coords + _south],
        //        0, TileStatsRenderer.Instance.WaterThreshold);
        //}

        //if (_elevationMap.ContainsKey(coords + _west))
        //{
        //    _elevationMap[coords + _west] = Mathf.Clamp(
        //        _elevationMap[coords + _west],
        //        0, TileStatsRenderer.Instance.WaterThreshold);
        //}

        //if (_elevationMap.ContainsKey(coords + _east))
        //{
        //    _elevationMap[coords + _east] = Mathf.Clamp(
        //        _elevationMap[coords + _east],
        //        0, TileStatsRenderer.Instance.WaterThreshold);
        //}
    }

    public void SetVegetationChanceAtTile(int x, int y, float vegetationChance)
    {
        _vegetationMap[x, y] = vegetationChance;
    }

    public void ModifyVegetationAtTile(int xCoord, int yCoord, float vegetationChange)
    {
        _vegetationMap[xCoord, yCoord] += vegetationChange;
    }

    internal bool CheckWaterStatus(Vector2Int testCoord, Vector2Int coordToIgnore)
    {
        if (_waterMap[testCoord.x, testCoord.y] > 0) return true;

        if (testCoord.y + 1 < _tileDimension &&
            testCoord.y + 1 != coordToIgnore.y &&
            _waterMap[testCoord.x, testCoord.y + 1] > 0) return true;

        if (testCoord.x + 1 < _tileDimension &&
            testCoord.x + 1 != coordToIgnore.x &&
            _waterMap[testCoord.x + 1, testCoord.y] > 0) return true;

        if (testCoord.y - 1 > 0 &&
            testCoord.y - 1 != coordToIgnore.y &&
            _waterMap[testCoord.x, testCoord.y - 1] > 0) return true;

        if (testCoord.x - 1 > 0 &&
            testCoord.x - 1 != coordToIgnore.x &&
            _waterMap[testCoord.x - 1, testCoord.y] > 0) return true;

        return false;
    }

    public void ModifyWaterStatusAtTile(int xCoord, int yCoord, float waterVolume, bool isStream)
    {
        _waterMap[xCoord, yCoord] = waterVolume;

        if (waterVolume > 0 && isStream)
        {
            //Reduce the new stream's elevation to encourage cleaner
            //joinings with other nearby streams
            float currentElev = _elevationMap[xCoord, yCoord];
            Vector2Int lowCoords =
                FindNeighborCoordsWithGreatestElevationDecrease(
                xCoord, yCoord,0);
            float lowElev = _elevationMap[lowCoords.x, lowCoords.y];
            float splitElev = (currentElev + currentElev + currentElev + lowElev) / 4f;
            _elevationMap[xCoord, yCoord] = splitElev;
        }
    }

    #endregion

    #region Gets

    public Vector3Int GetTileCoord(Vector3 worldPos)
    {
        if (worldPos.x > _tileDimension || worldPos.y > _tileDimension ||
            worldPos.x < 0 || worldPos.y < 0)
        {
            //Debug.Log($"TileData grid doesn't contain this world pos: {worldPos.x}, {worldPos.y}");
            return new Vector3Int(0, 0, 0);
        }
        return _grid.WorldToCell(worldPos);
    }

    public TileStats GetTileDataAtTileCoord(int xCoord, int yCoord)
    {
        TileStats td = new TileStats();
        if (xCoord >= _tileDimension
            || yCoord >= _tileDimension)
        {
            Debug.LogWarning("Invalid tile coord");
            return td;
        }
        td.Temperature = _temperatureMap[xCoord, yCoord];
        td.Moisture = _moistureMap[xCoord, yCoord];
        td.Elevation = _elevationMap[xCoord, yCoord];
        td.Population = _habitabilityMap[xCoord, yCoord];
        td.Traffic = _trafficMap[xCoord, yCoord];
        td.Vegetation = _vegetationMap[xCoord, yCoord];

        return td;
    }

    public float GetElevationAtCoord(int xCoord, int yCoord)
    {
        if (xCoord >= 0 && xCoord < _tileDimension &&
            yCoord >= 0 && yCoord < _tileDimension)
        {
            return _elevationMap[xCoord, yCoord];
        }

        else return 0;
    }

    public float GetTemperatureAtCoord(int xCoord, int yCoord)
    {
        return _temperatureMap[xCoord, yCoord];
    }

    public float GetMoistureAtCoord(int xCoord, int yCoord)
    {
        return _moistureMap[xCoord, yCoord];
    }

    public float GetWaterVolumeAtCoord(int xCoord, int yCoord)
    {
        return _waterMap[xCoord, yCoord];
    }

    public (Vector2Int, float)[] GetNeighborCellsWithElevationAndLowWater(Vector2Int sourceCoord)
    {
        int orthoCount = 4;
        (Vector2Int, float)[] nce = new (Vector2Int, float)[orthoCount];
        Vector2Int[] nays = GridUtilities.GetNeighboringCellCoordinates(
            _elevationMap, sourceCoord);

        for (int i = 0; i < orthoCount; i++)
        {
            Vector2Int coord = nays[i];
            nce[i].Item1 = coord;
            if (coord.x <0 || coord.y < 0 ||
                _waterMap[coord.x, coord.y] > 5)
            {
                nce[i].Item2 = -1;

            }
            else
            {
                nce[i].Item2 = _elevationMap[coord.x, coord.y];

            }
        }
        return nce;
    }

    public BiomeCategory GetBiomeCategoryAtCoord(int xCoord, int yCoord)
    {
        return _biomeMap[xCoord, yCoord];
    }
    
    public float GetVegetationChanceAtCoord(int x, int y)
    {
        float chance = _vegetationMap[x, y];

        if (y + 1 < _tileDimension &&
            TreeRenderer.Instance.CheckForTreesAtCoord(new Vector2Int(x,y+1)))
        {
            chance += _treeClumpingFactor;
        }
        if (x + 1 < _tileDimension &&
            TreeRenderer.Instance.CheckForTreesAtCoord(new Vector2Int(x+1, y)))
        {
            chance += _treeClumpingFactor;
        }
        if (y-1>=0 &&
            TreeRenderer.Instance.CheckForTreesAtCoord(new Vector2Int(x, y - 1)))
        {
            chance += _treeClumpingFactor;
        }
        if (x-1 >= 0 &&
            TreeRenderer.Instance.CheckForTreesAtCoord(new Vector2Int(x-1, y)))
        {
            chance += _treeClumpingFactor;
        }
        return chance;
    }

    public float GetHabitabilityAtCoord(int x, int y)
    {
        return _habitabilityMap[x, y];
    }

    public bool CheckIfWaterShouldBePresentAtCoord(int xCoord, int yCoord)
    {
        if (_waterMap[xCoord, yCoord] > 0) return true;
        else return false;
    }

    public bool CheckIfMountainShouldBePresentAtCoord(int xCoord, int yCoord)
    {
        // no mountains allowed on water tiles
        if (CheckIfWaterShouldBePresentAtCoord(xCoord,yCoord)) return false;

        if (_elevationMap[xCoord, yCoord] >= _mountainThreshold) return true;
        else return false;
    }
    internal bool CheckIfHillShouldBePresentAtCoord(int x, int y, out BiomeCategory biomeCategory)
    {
        biomeCategory = _biomeMap[x, y];

        // no hills allowed on water tiles
        if (CheckIfWaterShouldBePresentAtCoord(x, y)) return false;

        //No hills allowed on transition tiles (where biome doesn't match neighbor biomes)
        if (!CheckIfNeighborsAreSameBiomeCategory(x, y, biomeCategory))
        {
            return false;
        }

        //if (_elevationMap[x, y+1] >= _mountainThreshold ||
        //    _elevationMap[x +1, y] >= _mountainThreshold ||
        //    _elevationMap[x, y-1] >= _mountainThreshold ||
        //    _elevationMap[x - 1, y] >= _mountainThreshold) return false;
        if (_elevationMap[x, y] >= _hillThreshold &&
            _elevationMap[x, y] < _mountainThreshold)
        {
            return true;
        }
        else
        {
            biomeCategory = BiomeCategory.Unassigned;
            return false;
        }

    }
    
    public bool CheckIfNeighborsAreSameBiomeCategory(int x, int y, BiomeCategory testBiome)
    {
        if (y+1 < _tileDimension &&
            _biomeMap[x, y + 1] != testBiome) return false;
        if (x+1 < _tileDimension &&
            _biomeMap[x+1, y] != testBiome) return false;
        if (y > 0 && _biomeMap[x, y - 1] != testBiome) return false;
        if (x > 0 && _biomeMap[x-1, y] != testBiome) return false;

        return true;
    }
    
    #endregion

    #region Finds
    public Vector2Int FindRandomBeachCoord()
    {
        //float targetValue = TileStatsRenderer.Instance.WaterThreshold;
        //int row;
        //int col;
        //if (!GridSearch.SpiralSearch_ClosestToMinValue(_elevationMap, targetValue, 0.03f,
        //    out row, out col))
        //{
        //    GridSearch.SpiralSearch_ClosestToMinValue(_elevationMap, targetValue, 0.09f,
        //    out row, out col);
        //}

        //Debug.Log($"found a central water spot at {col},{row}");
        //return new Vector2Int(col, row); //col = x coord, row = y coord

        Debug.LogError("commented out");
        return Vector2Int.zero;
    }

    /// <summary>
    /// Returns an array of 2 Vector2Int coordinates. index 0 is steepest neighbor,
    /// index 1 is the flattest neighbor
    /// </summary>
    /// <param name="xCoord"></param>
    /// <param name="yCoord"></param>
    /// <returns></returns>
    public Vector2Int[] FindNeighborCoordsWithGreatestElevationIncrease(int xCoord, int yCoord)
    {
        Vector2Int[] neighborsBySteepness = new Vector2Int[2];
        neighborsBySteepness[0] = new Vector2Int(xCoord,yCoord); //default to north if identical
        neighborsBySteepness[1] = new Vector2Int(xCoord, yCoord);
        float delta_steepest = 0;
        float delta_flattest = 1;
        float currentElev = _elevationMap[xCoord, yCoord];
        float testElev = 0;

        //north
        if ( yCoord + 1 < _tileDimension && _waterMap[xCoord, yCoord + 1] <= 0)
        {
            testElev = _elevationMap[xCoord, yCoord + 1];

            if (testElev > currentElev + delta_steepest)
            {
                delta_steepest = testElev - currentElev;
                neighborsBySteepness[0] = new Vector2Int(xCoord, yCoord) + _north;
            }
            else if (testElev > currentElev && testElev < currentElev + delta_flattest)
            {
                delta_flattest = testElev - currentElev;
                neighborsBySteepness[1] = new Vector2Int(xCoord, yCoord) + _north;
            }
        }

        //east
        if (xCoord + 1 < _tileDimension && _waterMap[xCoord + 1, yCoord] <= 0)
        {
            testElev = _elevationMap[xCoord + 1, yCoord];
            if (testElev > currentElev + delta_steepest)
            {
                delta_steepest = testElev - currentElev;
                neighborsBySteepness[0] = new Vector2Int(xCoord, yCoord) + _east;
            }
            else if (testElev > currentElev && testElev < currentElev + delta_flattest)
            {
                delta_flattest = testElev - currentElev;
                neighborsBySteepness[1] = new Vector2Int(xCoord, yCoord) + _east;
            }
        }

        //south

        if (yCoord - 1 > 0 && _waterMap[xCoord, yCoord - 1] <= 0)
        {
            testElev = _elevationMap[xCoord, yCoord - 1];

            if (testElev > currentElev + delta_steepest)
            {
                delta_steepest = testElev - currentElev;
                neighborsBySteepness[0] = new Vector2Int(xCoord, yCoord) + _south;
            }
            else if (testElev > currentElev && testElev < currentElev + delta_flattest)
            {
                delta_flattest = testElev - currentElev;
                neighborsBySteepness[1] = new Vector2Int(xCoord, yCoord) + _south;
            }
        }

        //west
        if (xCoord - 1 > 0 && _waterMap[xCoord - 1, yCoord] <= 0)
        {
            testElev = _elevationMap[xCoord-1, yCoord];

            if (testElev > currentElev + delta_steepest)
            {
                delta_steepest = testElev - currentElev;
                neighborsBySteepness[0] = new Vector2Int(xCoord, yCoord) + _west;
            }
            else if (testElev > currentElev && testElev < currentElev + delta_flattest)
            {
                delta_flattest = testElev - currentElev;
                neighborsBySteepness[1] = new Vector2Int(xCoord, yCoord) + _west;
            }
        }

        //if (xCoord -1 >= 0 && 
        //    _elevationMap[xCoord - 1, yCoord] > currentElev + delta_steepest)
        //{
        //    delta_steepest = _elevationMap[xCoord-1, yCoord] - currentElev;
        //    nay = (xCoord - 1, yCoord);
        //}

        //north a second time for flattest check.
        if (yCoord + 1 < _tileDimension && _waterMap[xCoord, yCoord + 1] <= 0)
        {
            testElev = _elevationMap[xCoord, yCoord + 1];
            if (testElev > currentElev && testElev < currentElev + delta_flattest)
            {
                delta_flattest = testElev - currentElev;
                neighborsBySteepness[1] = new Vector2Int(xCoord, yCoord) + _north;
            }
        }

        return neighborsBySteepness;

    }

    /// <summary>
    /// Returns the Coordinates of the neighbor with the steepest descent from the 
    /// source coordinate. If no neighbors are found, the search will be run with the
    /// uphill tolerance applied to 'creep' over small local minima.
    /// </summary>

    public Vector2Int FindNeighborCoordsWithGreatestElevationDecrease(int xCoord, int yCoord, float uphillTolerance)
    {
        bool foundNeighbor = false;
        Vector2Int lowestNeighbor = new Vector2Int(xCoord,yCoord);
        float currentElev = _elevationMap[xCoord, yCoord];
        float elevationToBeat = currentElev;

        //north
        if (yCoord + 1 < _tileDimension)
        {
            if (_elevationMap[xCoord, yCoord + 1] < elevationToBeat)
            {
                elevationToBeat = _elevationMap[xCoord, yCoord + 1];
                lowestNeighbor = new Vector2Int(xCoord, yCoord) + _north;
                foundNeighbor = true;
            }
        }

        //east
        if (xCoord + 1 < _tileDimension)
        {
            if (_elevationMap[xCoord + 1, yCoord] < elevationToBeat)
            {
                elevationToBeat = _elevationMap[xCoord + 1, yCoord];
                lowestNeighbor = new Vector2Int(xCoord, yCoord) + _east;
                foundNeighbor = true;
            }
        }

        //south
        if (yCoord - 1 > 0 )
        {
            if (_elevationMap[xCoord, yCoord - 1] < elevationToBeat)
            {
                elevationToBeat = _elevationMap[xCoord, yCoord - 1];
                lowestNeighbor = new Vector2Int(xCoord, yCoord) + _south;
                foundNeighbor = true;
            }
        }

        //west
        if (xCoord - 1 > 0)
        {
            if (_elevationMap[xCoord - 1, yCoord] < elevationToBeat)
            {
                elevationToBeat = _elevationMap[xCoord - 1, yCoord];
                lowestNeighbor = new Vector2Int(xCoord, yCoord) + _west;
                foundNeighbor = true;
            }
        }

        if (!foundNeighbor && uphillTolerance > 0)
        {
            elevationToBeat += uphillTolerance;
            //north
            if (yCoord + 1 < _tileDimension)
            {
                if (_elevationMap[xCoord, yCoord + 1] < elevationToBeat)
                {
                    elevationToBeat = _elevationMap[xCoord, yCoord + 1];
                    lowestNeighbor = new Vector2Int(xCoord, yCoord) + _north;
                }
            }

            //east
            if (xCoord + 1 < _tileDimension)
            {
                if (_elevationMap[xCoord + 1, yCoord] < elevationToBeat)
                {
                    elevationToBeat = _elevationMap[xCoord + 1, yCoord];
                    lowestNeighbor = new Vector2Int(xCoord, yCoord) + _east;
                }
            }

            //south
            if (yCoord - 1 > 0)
            {
                if (_elevationMap[xCoord, yCoord - 1] < elevationToBeat)
                {
                    elevationToBeat = _elevationMap[xCoord, yCoord - 1];
                    lowestNeighbor = new Vector2Int(xCoord, yCoord) + _south;
                }
            }

            //west
            if (xCoord - 1 > 0)
            {
                if (_elevationMap[xCoord - 1, yCoord] < elevationToBeat)
                {
                    elevationToBeat = _elevationMap[xCoord - 1, yCoord];
                    lowestNeighbor = new Vector2Int(xCoord, yCoord) + _west;
                }
            }
        }

        return lowestNeighbor;
    }

    internal Vector2Int FindHighestElevationCellWithinGrid(Vector2Int origin, Vector2Int farCorner)
    {
        int xWidth = farCorner.x - origin.x + 1;
        int yHeight = farCorner.y - origin.y + 1;

        //Debug.Log($"Passing array of size {xWidth},{yHeight}");
        float[,] arr = GridUtilities.ExtractSubArray(_elevationMap,
            origin.x, origin.y, xWidth, yHeight);

        return origin + GridUtilities.FindCellWithHighestValue(arr);
    }

    internal float FindTotalMoistureWithinWaterGrid(Vector2Int origin, Vector2Int farCorner)
    {
        int xWidth = farCorner.x - origin.x;
        int yHeight = farCorner.y - origin.y;
        float[,] arr = GridUtilities.ExtractSubArray(_moistureMap,
            origin.x, origin.y, xWidth, yHeight);
        return GridUtilities.FindSumValueWithinGrid(arr);
    }

    internal Vector2Int FindLeastPopulatedCellsWithinGrid(Vector2Int origin, Vector2Int farCorner)
    {
        int xWidth = farCorner.x - origin.x + 1;
        int yHeight = farCorner.y - origin.y + 1;

        //Debug.Log($"Passing array of size {xWidth},{yHeight}");
        float[,] arr = GridUtilities.ExtractSubArray(_habitabilityMap,
            origin.x, origin.y, xWidth, yHeight);

        return origin + GridUtilities.FindCellWithLowestValue(arr);
    }

    internal float FindTotalPopulationWithinPopulationGrid(Vector2Int origin, Vector2Int farCorner)
    {
        int xWidth = farCorner.x - origin.x;
        int yHeight = farCorner.y - origin.y;
        float[,] arr = GridUtilities.ExtractSubArray(_habitabilityMap,
            origin.x, origin.y, xWidth, yHeight);
        return GridUtilities.FindSumValueWithinGrid(arr);
    }

    public Vector2Int FindNeighborCellWithHigherPopulationValue(int xCoord, int yCoord)
    {
        Vector2Int highestNay = new Vector2Int(xCoord, yCoord);
        float currentPop = _habitabilityMap[xCoord, yCoord];
        float popToBeat = currentPop;

        //north
        if (yCoord + 1 < _tileDimension)
        {
            if (_habitabilityMap[xCoord, yCoord + 1] > popToBeat)
            {
                popToBeat = _habitabilityMap[xCoord, yCoord + 1];
                highestNay = new Vector2Int(xCoord, yCoord) + _north;
            }
        }

        //east
        if (xCoord + 1 < _tileDimension)
        {
            if (_habitabilityMap[xCoord + 1, yCoord] > popToBeat)
            {
                popToBeat = _habitabilityMap[xCoord + 1, yCoord];
                highestNay = new Vector2Int(xCoord, yCoord) + _east;
            }
        }

        //south
        if (yCoord - 1 > 0)
        {
            if (_habitabilityMap[xCoord, yCoord - 1] > popToBeat)
            {
                popToBeat = _habitabilityMap[xCoord, yCoord - 1];
                highestNay = new Vector2Int(xCoord, yCoord) + _south;
            }
        }

        //west
        if (xCoord - 1 > 0)
        {
            if (_habitabilityMap[xCoord - 1, yCoord] > popToBeat)
            {
                popToBeat = _habitabilityMap[xCoord - 1, yCoord];
                highestNay = new Vector2Int(xCoord, yCoord) + _west;
            }
        }
        return highestNay;
    }


    public Vector2Int GetCoordinatesForPopulationArrayFromSpiralCoord(Vector2Int startCoord, int index)
    {
       return GridUtilities.GetSpiralCoordinateAtIndex(_habitabilityMap, startCoord, index);
    }

    public bool CheckIfCellIsHabitable(Vector2Int coord)
    {
        if (coord.x > _tileDimension || coord.y > _tileDimension ||
            coord.x < 0 || coord.y < 0) return false;

        if (TreeRenderer.Instance.CheckForTreesAtCoord(coord) ||
            MountainRenderer.Instance.CheckForMountainsOrHillsAtCoord(coord) ||
            WaterRenderer.Instance.CheckIfHasWaterTileAtCoord(coord))
        {          
            return false;
        }

        if (!CheckIfNeighborsAreSameBiomeCategory(coord.x, coord.y,
            _biomeMap[coord.x, coord.y])) return false;

        return true;
    }

    #endregion

        #region Biome Categorization

        public void CategorizeTileAtCoord(int xCoord, int yCoord)
    {
        float moisture = _moistureMap[xCoord, yCoord];
        float temp = _temperatureMap[xCoord,yCoord];
        MoistureCategory moistureCategory = ConvertMoistureIntoMoistureCat(moisture);
        TemperatureCategory tempCat = ConvertTemperatureIntoTempCat(temp);

        BiomeCategory bc = BiomeCategory.Unassigned;
        switch (moistureCategory)
        {
            case MoistureCategory.Dry:
                switch (tempCat)
                {
                    case TemperatureCategory.Cold:
                        bc = BiomeCategory.ColdDry;
                        break;
                    case TemperatureCategory.MidTemp:
                        bc = BiomeCategory.MidtempDry;
                        break;
                    case TemperatureCategory.Hot:
                        bc = BiomeCategory.HotDry;
                        break;
                }
                break;

            case MoistureCategory.MidWet:
                switch (tempCat)
                {
                    case TemperatureCategory.Cold:
                        bc = BiomeCategory.ColdMidwet;
                        break;
                    case TemperatureCategory.MidTemp:
                        bc = BiomeCategory.MidtempMidwet;
                        break;
                    case TemperatureCategory.Hot:
                        bc = BiomeCategory.HotMidwet;
                        break;
                }
                break;

            case MoistureCategory.Wet:
                switch (tempCat)
                {
                    case TemperatureCategory.Cold:
                        bc = BiomeCategory.ColdWet;
                        break;
                    case TemperatureCategory.MidTemp:
                        bc = BiomeCategory.MidtempWet;
                        break;
                    case TemperatureCategory.Hot:
                        bc = BiomeCategory.HotWet;
                        break;
                }
                break;
        }


        //float chanceforVegetation = ConvertBiomeAndMoistureIntoVegetationChance(moistureCategory, moisture);
        //_vegetationMap[xCoord, yCoord] = chanceforVegetation;
        _biomeMap[xCoord, yCoord] = bc;
    }

    private float ConvertBiomeAndMoistureIntoVegetationChance(MoistureCategory moistureCat, float moisture)
    {
        float chance = 0;
        switch (moistureCat)
        {
            case MoistureCategory.Dry:
                chance = moisture / _dryThreshold /10f;
                break;
            case MoistureCategory.MidWet:
                chance = moisture / _wetThreshold / 5f;
                break;
            case MoistureCategory.Wet:
                chance = moisture / 2f;
                break;
        }
        return chance;
    }

    private MoistureCategory ConvertMoistureIntoMoistureCat(float moisture)
    {
        if (moisture < _dryThreshold)
        {
            return MoistureCategory.Dry;
        }
        if (moisture > _wetThreshold)
        {
            return MoistureCategory.Wet;
        }
        else return MoistureCategory.MidWet;
    }

    private TemperatureCategory ConvertTemperatureIntoTempCat(float temperature)
    {
        if (temperature < _coldThreshold)
        {
            return TemperatureCategory.Cold;
        }
        if (temperature > _hotThreshold)
        {
            return TemperatureCategory.Hot;
        }
        else return TemperatureCategory.MidTemp;
    }

    #endregion
}
