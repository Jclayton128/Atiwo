using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileStatsHolder : MonoBehaviour
{

    /// <summary>
    /// This holds multiple Dictionary<Vec2Int, float>, one for each tile data parameter (temperature, moisture, etc).
    /// 
    /// </summary>
    /// 
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

    //state
    public int Dimension => _tileDimension;
    //Dictionary<Vector3Int, float> _temperatureMap = new Dictionary<Vector3Int, float>();
    //Dictionary<Vector3Int, float> _moistureMap = new Dictionary<Vector3Int, float>();
    //Dictionary<Vector3Int, float> _elevationMap = new Dictionary<Vector3Int, float>();

    //Dictionary<Vector3Int, float> _trafficMap = new Dictionary<Vector3Int, float>();
    //Dictionary<Vector3Int, float> _vegetationMap = new Dictionary<Vector3Int, float>();
    //Dictionary<Vector3Int, float> _populationMap = new Dictionary<Vector3Int, float>();

    float[,] _temperatureMap;
    float[,] _moistureMap;
    float[,] _elevationMap;
    float[,] _trafficMap;
    float[,] _vegetationMap;
    float[,] _populationMap;
    float[,] _waterMap;

    private void Awake()
    {
        Instance = this;
        _temperatureMap = new float[_tileDimension, _tileDimension];
        _moistureMap = new float[_tileDimension, _tileDimension];
        _elevationMap = new float[_tileDimension, _tileDimension];
        _trafficMap = new float[_tileDimension, _tileDimension];
        _vegetationMap = new float[_tileDimension, _tileDimension];
        _populationMap = new float[_tileDimension, _tileDimension];
        _waterMap = new float[_tileDimension, _tileDimension];

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
        Array.Clear(_populationMap, 0, _tileDimension);
        Array.Clear(_waterMap, 0, _tileDimension);

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
                _populationMap[x, y] = 0;
                _waterMap[x, y] = 0;
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

    public void SetElevationAtTile(int xCoord, int yCoord, float elevation)
    {
        _elevationMap[xCoord, yCoord] = elevation;
    }

    public void ModifyPopulationAtTile(int xCoord, int yCoord, float populationChange)
    {
        _populationMap[xCoord, yCoord] += populationChange;
    }

    public void ModifyTrafficAtTile(int xCoord, int yCoord, float trafficChange)
    {
        _trafficMap[xCoord, yCoord] += trafficChange;
    }

    internal void EnforceDeepWaterWithWaterAsNeighbor(int xCoord, int yCoord)
    {
        if (xCoord + 1 < _tileDimension)
        {
            _elevationMap[xCoord + 1, yCoord] = Mathf.Clamp(
                _elevationMap[xCoord + 1, yCoord],
                0, TileStatsRenderer.Instance.WaterThreshold);
        }

        if (xCoord - 1 >= 0)
        {
            _elevationMap[xCoord - 1, yCoord] = Mathf.Clamp(
                _elevationMap[xCoord - 1, yCoord],
                0, TileStatsRenderer.Instance.WaterThreshold);
        }

        if (yCoord + 1 < _tileDimension)
        {
            _elevationMap[xCoord, yCoord + 1] = Mathf.Clamp(
                _elevationMap[xCoord, yCoord + 1],
                0, TileStatsRenderer.Instance.WaterThreshold);
        }

        if (yCoord - 1 >= 0)
        {
            _elevationMap[xCoord, yCoord - 1] = Mathf.Clamp(
                _elevationMap[xCoord, yCoord - 1],
                0, TileStatsRenderer.Instance.WaterThreshold);
        }

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
                xCoord, yCoord);
            float lowElev = _elevationMap[lowCoords.x, lowCoords.y];
            float splitElev = (currentElev + currentElev + lowElev) / 3f;
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
        td.Population = _populationMap[xCoord, yCoord];
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

    public (float, float) GetPrimaryStatsAtCoord(int xCoord, int yCoord)
    {
        (float, float) stats;
        stats.Item1 = _temperatureMap[xCoord, yCoord];
        stats.Item2 = _moistureMap[xCoord, yCoord];

        return stats;
    }

    public float GetWaterVolumeAtCoord(int xCoord, int yCoord)
    {
        return _waterMap[xCoord, yCoord];
    }

    public (Vector2Int, float)[] GetNeighborCellsWithElevationAndZeroWater(Vector2Int sourceCoord)
    {
        int orthoCount = 4;
        (Vector2Int, float)[] nce = new (Vector2Int, float)[orthoCount];
        Vector2Int[] nays = GridSearch.GetNeighboringCellCoordinates(
            _elevationMap, sourceCoord);

        for (int i = 0; i < orthoCount; i++)
        {
            Vector2Int coord = nays[i];
            nce[i].Item1 = coord;
            if (coord.x <0 || coord.y < 0 ||
                _waterMap[coord.x, coord.y] > 0)
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

    #endregion

    #region Finds
    public Vector2Int FindRandomBeachCoord()
    {
        float targetValue = TileStatsRenderer.Instance.WaterThreshold;
        int row;
        int col;
        if (!GridSearch.SpiralSearch_ClosestToMinValue(_elevationMap, targetValue, 0.03f,
            out row, out col))
        {
            GridSearch.SpiralSearch_ClosestToMinValue(_elevationMap, targetValue, 0.09f,
            out row, out col);
        }

        Debug.Log($"found a central water spot at {col},{row}");
        return new Vector2Int(col, row); //col = x coord, row = y coord
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

    public Vector2Int FindNeighborCoordsWithGreatestElevationDecrease(int xCoord, int yCoord)
    {
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
        if (yCoord - 1 > 0 )
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

        return lowestNeighbor;
    }


    internal Vector2Int FindHighestCellWithinWaterGrid(Vector2Int origin, Vector2Int farCorner)
    {
        int xWidth = farCorner.x - origin.x + 1;
        int yHeight = farCorner.y - origin.y + 1;

        //Debug.Log($"Passing array of size {xWidth},{yHeight}");
        float[,] arr = GridSearch.ExtractSubArray(_elevationMap,
            origin.x, origin.y, xWidth, yHeight);

        return origin + GridSearch.FindCellWithHighestValue(arr);
    }


    internal float FindTotalMoistureWithinWaterGrid(Vector2Int origin, Vector2Int farCorner)
    {
        int xWidth = farCorner.x - origin.x;
        int yHeight = farCorner.y - origin.y;
        float[,] arr = GridSearch.ExtractSubArray(_moistureMap,
            origin.x, origin.y, xWidth, yHeight);
        return GridSearch.FindSumValueWithinGrid(arr);
    }

    #endregion
}
