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
    Dictionary<Vector3Int, float> _temperatureMap = new Dictionary<Vector3Int, float>();
    Dictionary<Vector3Int, float> _moistureMap = new Dictionary<Vector3Int, float>();
    Dictionary<Vector3Int, float> _elevationMap = new Dictionary<Vector3Int, float>();

    Dictionary<Vector3Int, float> _trafficMap = new Dictionary<Vector3Int, float>();
    Dictionary<Vector3Int, float> _vegetationMap = new Dictionary<Vector3Int, float>();
    Dictionary<Vector3Int, float> _populationMap = new Dictionary<Vector3Int, float>();

    private void Awake()
    {
        Instance = this;
        _grid = GetComponent<Grid>();
    }

    private void Start()
    {
        ResetMaps();
    }

    private void ResetMaps()
    {
        _temperatureMap.Clear();
        _moistureMap.Clear();
        _elevationMap.Clear();
        _trafficMap.Clear();
        _vegetationMap.Clear();
        _populationMap.Clear();

        Vector3Int coord = new Vector3Int(0, 0,0);
        for (int x = 0; x < _tileDimension; x++)
        {
            for (int y = 0; y < _tileDimension; y++)
            {
                coord.x = x;
                coord.y = y;
                _temperatureMap.Add(coord, _startingValue);
                _moistureMap.Add(coord, _startingValue);
                _elevationMap.Add(coord, _startingValue);
                _trafficMap.Add(coord, 0); 
                _vegetationMap.Add(coord, 0);
                _populationMap.Add(coord, 0);

            }
        }
    }

    #region Modify Data Maps at Coords
    public void ModifyTemperatureAtTile(Vector3Int tileCoord, float temperatureChange)
    {
        _temperatureMap[tileCoord] += temperatureChange;
    }

    public void SetTemperatureAtTile(Vector3Int tileCoord, float temperature)
    {
        _temperatureMap[tileCoord] = temperature;
    }

    public void ModifyMoistureAtTile(Vector3Int tileCoord, float moistureChange)
    {
        _moistureMap[tileCoord] += moistureChange;
    }

    public void SetMoistureAtTile(Vector3Int tileCoord, float moisture)
    {
        _moistureMap[tileCoord] = moisture;
    }

    public void ModifyElevationAtTile(Vector3Int tileCoord, float elevationChange)
    {
        _elevationMap[tileCoord] += elevationChange;
    }

    public void SetElevationAtTile(Vector3Int tileCoord, float elevation)
    {
        _elevationMap[tileCoord] = elevation;
    }

    public void ModifyPopulationAtTile(Vector3Int tileCoord, float populationChange)
    {
        _populationMap[tileCoord] += populationChange;
    }

    public void ModifyTrafficAtTile(Vector3Int tileCoord, float trafficChange)
    {
        _trafficMap[tileCoord] += trafficChange;
    }

    internal void EnforceDeepWaterWithWaterAsNeighbor(Vector3Int coords)
    {
        if (_elevationMap.ContainsKey(coords + _north))
        {
            _elevationMap[coords + _north] = Mathf.Clamp(
                _elevationMap[coords + _north],
                0, TileStatsRenderer.Instance.WaterThreshold);
        }

        if (_elevationMap.ContainsKey(coords + _south))
        {
            _elevationMap[coords + _south] = Mathf.Clamp(
                _elevationMap[coords + _south],
                0, TileStatsRenderer.Instance.WaterThreshold);
        }

        if (_elevationMap.ContainsKey(coords + _west))
        {
            _elevationMap[coords + _west] = Mathf.Clamp(
                _elevationMap[coords + _west],
                0, TileStatsRenderer.Instance.WaterThreshold);
        }

        if (_elevationMap.ContainsKey(coords + _east))
        {
            _elevationMap[coords + _east] = Mathf.Clamp(
                _elevationMap[coords + _east],
                0, TileStatsRenderer.Instance.WaterThreshold);
        }
    }

    public void ModifyVegetationAtTile(Vector3Int tileCoord, float vegetationChange)
    {
        _vegetationMap[tileCoord] += vegetationChange;
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

    public TileStats GetTileDataAtTileCoord(Vector3Int tileCoord)
    {
        TileStats td = new TileStats();
        if (tileCoord.x >= _tileDimension
            || tileCoord.y >= _tileDimension
            || tileCoord.z >= _tileDimension)
        {
            Debug.LogWarning("Invalid tile coord");
            return td;
        }
        td.Temperature = _temperatureMap[tileCoord];
        td.Moisture = _moistureMap[tileCoord];
        td.Elevation = _elevationMap[tileCoord];
        td.Population = _populationMap[tileCoord];
        td.Traffic = _trafficMap[tileCoord];
        td.Vegetation = _vegetationMap[tileCoord];

        return td;
    }

    public float GetElevationAtCoord(Vector3Int coord)
    {
        if (_elevationMap.ContainsKey(coord)) return _elevationMap[coord];
        else return 0;
    }

    public (float, float) GetPrimaryStatsAtCoord(Vector3Int tileCoord)
    {
        (float, float) stats;
        stats.Item1 = _temperatureMap[tileCoord];
        stats.Item2 = _moistureMap[tileCoord];

        return stats;
    }
}
