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
    Vector3Int _north = new Vector3Int(0, 1, 0);
    Vector3Int _south = new Vector3Int(0, -1, 0);
    Vector3Int _east = new Vector3Int(1, 0, 0);
    Vector3Int _west = new Vector3Int(-1, 0, 0);

    public static TileStatsHolder Instance;
    Grid _grid;

    //settings
    [SerializeField][Range(10, 100)] int _tileDimension = 10;
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
    bool[,] _streamMap;

    private void Awake()
    {
        Instance = this;
        _temperatureMap = new float[_tileDimension, _tileDimension];
        _moistureMap = new float[_tileDimension, _tileDimension];
        _elevationMap = new float[_tileDimension, _tileDimension];
        _trafficMap = new float[_tileDimension, _tileDimension];
        _vegetationMap = new float[_tileDimension, _tileDimension];
        _populationMap = new float[_tileDimension, _tileDimension];
        _streamMap = new bool[_tileDimension, _tileDimension];

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
        Array.Clear(_streamMap, 0, _tileDimension);

        Vector3Int coord = new Vector3Int(0, 0,0);
        for (int x = 0; x < _tileDimension; x++)
        {
            for (int y = 0; y < _tileDimension; y++)
            {
                coord.x = x;
                coord.y = y;
                _temperatureMap[x,y] =  _startingValue;
                _moistureMap[x,y] =  _startingValue;
                _elevationMap[x,y] =  _startingValue;

                _trafficMap[x,y] =  0;
                _vegetationMap[x,y] =  0;
                _populationMap[x,y] =  0;
                _streamMap[x, y] = false;
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
        if (xCoord+1 < _tileDimension)
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

        if (yCoord -1 >= 0)
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
        _vegetationMap[xCoord,yCoord] += vegetationChange;
    }

    public void ModifyStreamStatusAtTile(int xCoord, int yCoord, bool isStream)
    {
        _streamMap[xCoord, yCoord] = isStream;
    }

    #endregion

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

    public (int,int) FindRandomBeachCoord()
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

        Debug.Log($"found a central water spot at {row},{col}");
        return (row, col);
    }

    public (int,int)[] FindNeighborCoordsWithGreatestElevationIncrease(int xCoord, int yCoord)
    {       
        (int, int)[] neighborsBySteepness = new (int, int)[2];
        neighborsBySteepness[0] = (xCoord, yCoord); //default to north if identical
        neighborsBySteepness[1] = (xCoord, yCoord);
        float delta_steepest = 0;
        float delta_flattest = 1;
        float currentElev = _elevationMap[xCoord, yCoord];
        float testElev = 0;

        //north
        if ( yCoord + 1 < _tileDimension && !_streamMap[xCoord, yCoord + 1])
        {
            testElev = _elevationMap[xCoord, yCoord + 1];

            if (testElev > currentElev + delta_steepest)
            {
                delta_steepest = testElev - currentElev;
                neighborsBySteepness[0] = (xCoord, yCoord + 1);
            }
            else if (testElev > currentElev && testElev < currentElev + delta_flattest)
            {
                delta_flattest = testElev - currentElev;
                neighborsBySteepness[1] = (xCoord, yCoord + 1);
            }
        }

        //east
        if (xCoord + 1 < _tileDimension && !_streamMap[xCoord + 1, yCoord])
        {
            testElev = _elevationMap[xCoord + 1, yCoord];
            if (testElev > currentElev + delta_steepest)
            {
                delta_steepest = testElev - currentElev;
                neighborsBySteepness[0] = (xCoord + 1, yCoord);
            }
            else if (testElev > currentElev && testElev < currentElev + delta_flattest)
            {
                delta_flattest = testElev - currentElev;
                neighborsBySteepness[1] = (xCoord + 1, yCoord);
            }
        }

        //south

        if (yCoord - 1 > 0 && !_streamMap[xCoord, yCoord - 1])
        {
            testElev = _elevationMap[xCoord, yCoord - 1];

            if (testElev > currentElev + delta_steepest)
            {
                delta_steepest = testElev - currentElev;
                neighborsBySteepness[0] = (xCoord, yCoord - 1);
            }
            else if (testElev > currentElev && testElev < currentElev + delta_flattest)
            {
                delta_flattest = testElev - currentElev;
                neighborsBySteepness[1] = (xCoord, yCoord - 1);
            }
        }

        //west
        if (xCoord - 1 > 0 && !_streamMap[xCoord - 1, yCoord])
        {
            testElev = _elevationMap[xCoord-1, yCoord];

            if (testElev > currentElev + delta_steepest)
            {
                delta_steepest = testElev - currentElev;
                neighborsBySteepness[0] = (xCoord - 1, yCoord);
            }
            else if (testElev > currentElev && testElev < currentElev + delta_flattest)
            {
                delta_flattest = testElev - currentElev;
                neighborsBySteepness[1] = (xCoord - 1, yCoord);
            }
        }

        //if (xCoord -1 >= 0 && 
        //    _elevationMap[xCoord - 1, yCoord] > currentElev + delta_steepest)
        //{
        //    delta_steepest = _elevationMap[xCoord-1, yCoord] - currentElev;
        //    nay = (xCoord - 1, yCoord);
        //}

        //north a second time for flattest check.
        if (yCoord + 1 < _tileDimension && !_streamMap[xCoord, yCoord + 1])
        {
            testElev = _elevationMap[xCoord, yCoord + 1];
            if (testElev > currentElev && testElev < currentElev + delta_flattest)
            {
                delta_flattest = testElev - currentElev;
                neighborsBySteepness[1] = (xCoord, yCoord + 1);
            }
        }

        return neighborsBySteepness;

    }
}
