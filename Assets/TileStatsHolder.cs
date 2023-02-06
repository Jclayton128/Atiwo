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
    
    public static TileStatsHolder Instance;
    Grid _grid;

    //settings
    [SerializeField][Range(10, 100)] int _tileDimension = 10;
    [SerializeField] float _startingValue = 0.5f;

    //state
    public int Dimension => _tileDimension;
    Dictionary<Vector3Int, float> _temperatureMap = new Dictionary<Vector3Int, float>();
    Dictionary<Vector3Int, float> _moistureMap = new Dictionary<Vector3Int, float>();

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

    public void ModifyPopulationAtTile(Vector3Int tileCoord, float populationChange)
    {
        _populationMap[tileCoord] += populationChange;
    }

    public void ModifyTrafficAtTile(Vector3Int tileCoord, float trafficChange)
    {
        _trafficMap[tileCoord] += trafficChange;
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

        td.Temperature = _temperatureMap[tileCoord];
        td.Moisture = _moistureMap[tileCoord];
        td.Population = _populationMap[tileCoord];
        td.Traffic = _trafficMap[tileCoord];
        td.Vegetation = _vegetationMap[tileCoord];

        return td;
    }
}
