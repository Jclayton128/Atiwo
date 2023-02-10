using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileStatsRenderer : MonoBehaviour
{
    /// <summary>
    /// This performs a one-way conversion on the tile data for each grid cell
    /// into a visual depiction.
    /// </summary>
    /// 

    private enum MoistureCategory { Dry, MidWet, Wet, River, Water}
    private enum TemperatureCategory { Cold, MidTemp, Hot}


    public static TileStatsRenderer Instance;

    [SerializeField] Tilemap _tilemap_darkland = null;
    [SerializeField] Tilemap _tilemap_pack = null;
    [SerializeField] Tilemap _tilemap_sand = null;
    [SerializeField] Tilemap _tilemap_grass = null;
    [SerializeField] Tilemap _tilemap_grass_light = null;
    [SerializeField] Tilemap _tilemap_swamp = null;
    [SerializeField] Tilemap _tilemap_swamp_light = null;
    [SerializeField] Tilemap _tilemap_snow = null;
    [SerializeField] Tilemap _tilemap_snow_light = null;
    [SerializeField] Tilemap _tilemap_river = null;
    [SerializeField] Tilemap _tilemap_water = null;

    [SerializeField] Tilemap _tilemap_population_light = null;
    [SerializeField] Tilemap _tilemap_population_heavy = null;
    [SerializeField] Tilemap _tilemap_traffic = null;
    [SerializeField] Tilemap _tilemap_vegetation = null;

    //settings
    [Header("Base Tile Examples")]
    [SerializeField] TileBase _darkland = null;
    [SerializeField] TileBase _pack = null;
    [SerializeField] TileBase _sand = null;
    [SerializeField] TileBase _grass = null;
    [SerializeField] TileBase _grass_light = null;
    [SerializeField] TileBase _swamp = null;
    [SerializeField] TileBase _swamp_light = null;
    [SerializeField] TileBase _snow = null;
    [SerializeField] TileBase _snow_light = null;


    [Header("Temperature/Moisture Thresholds")]
    [SerializeField] float _dryThreshold = 0.3f;
    [SerializeField] float _wetThreshold = 0.7f;
    [SerializeField] float _coldThreshold = 0.3f;
    [SerializeField] float _hotThreshold = 0.7f;
    [SerializeField] float _thresholdTolerance = 0.1f;

    [Header("Population Tile Examples")]
    [SerializeField] TileBase _pavers_middle = null;
    [SerializeField] TileBase _pavers_dense = null;

    [Header("Population Thresholds")]
    [SerializeField] float _sparseThreshold = 0.3f;
    [SerializeField] float _denseThreshold = 0.7f;

    [Header("Path Tile Examples")]
    [SerializeField] TileBase _path_brown = null;

    Vector3Int _north = new Vector3Int(0, 1,0);
    Vector3Int _south = new Vector3Int(0, -1,0);
    Vector3Int _east = new Vector3Int(1, 0,0);
    Vector3Int _west = new Vector3Int(-1, 0,0);

    private void Awake()
    {
        Instance = this;
    }

    [ContextMenu("Render All Cells")]
    private void RenderAllCells()
    {
        Vector3Int coord = new Vector3Int(0, 0, 0);
        for (int x = 0; x < TileStatsHolder.Instance.Dimension; x++)
        {
            for (int y = 0; y < TileStatsHolder.Instance.Dimension; y++)
            {
                coord.x = x;
                coord.y = y;
                RenderSingleCellByCoord(coord);
            }
        }
    }


    public void RenderSingleCellByCoord(Vector3Int coord)
    {
        TileStats td = TileStatsHolder.Instance.GetTileDataAtTileCoord(coord);

        RenderBaseTile(coord, td);
        RenderPopulationTile(coord, td);
        RenderTrafficTile(coord, td);
        RenderVegetationTile(coord, td);
    }

    private void RenderBaseTile(Vector3Int coord, TileStats td)
    {
        MoistureCategory moistureCategory = ConvertMoistureIntoMoistureCat(td.Moisture);
        TemperatureCategory tempCat = ConvertTemperatureIntoTempCat(td.Temperature);
        TileBase tile = _grass;
        switch (moistureCategory)
        {
            case MoistureCategory.Dry:
                switch (tempCat)
                {
                    case TemperatureCategory.Cold:
                        tile = _darkland;
                        _tilemap_darkland.SetTile(coord, tile);
                        _tilemap_darkland.SetTile(coord + _north, tile);
                        _tilemap_darkland.SetTile(coord + _south, tile);
                        _tilemap_darkland.SetTile(coord + _east, tile);
                        _tilemap_darkland.SetTile(coord+_west, tile);
                        break;
                    case TemperatureCategory.MidTemp:
                        tile = _pack;
                        _tilemap_pack.SetTile(coord, tile);
                        _tilemap_pack.SetTile(coord + _north, tile);
                        _tilemap_pack.SetTile(coord + _south, tile);
                        _tilemap_pack.SetTile(coord + _east, tile);
                        _tilemap_pack.SetTile(coord + _west, tile);
                        break;
                    case TemperatureCategory.Hot:
                        tile = _sand;
                        _tilemap_sand.SetTile(coord, tile);
                        _tilemap_sand.SetTile(coord + _north, tile);
                        _tilemap_sand.SetTile(coord + _south, tile);
                        _tilemap_sand.SetTile(coord + _east, tile);
                        _tilemap_sand.SetTile(coord + _west, tile);
                        break;
                }
                break;

            case MoistureCategory.MidWet:
                switch (tempCat)
                {
                    case TemperatureCategory.Cold:
                        tile = _snow;
                        _tilemap_snow.SetTile(coord, tile);
                        _tilemap_snow.SetTile(coord + _north, tile);
                        _tilemap_snow.SetTile(coord + _south, tile);
                        _tilemap_snow.SetTile(coord + _east, tile);
                        _tilemap_snow.SetTile(coord + _west, tile);
                        break;
                    case TemperatureCategory.MidTemp:
                        tile = _grass;
                        _tilemap_grass.SetTile(coord, tile);
                        _tilemap_grass.SetTile(coord + _north, tile);
                        _tilemap_grass.SetTile(coord + _south, tile);
                        _tilemap_grass.SetTile(coord + _east, tile);
                        _tilemap_grass.SetTile(coord + _west, tile);
                        break;
                    case TemperatureCategory.Hot:
                        tile = _grass_light;
                        _tilemap_grass_light.SetTile(coord, tile);
                        _tilemap_grass_light.SetTile(coord + _north, tile);
                        _tilemap_grass_light.SetTile(coord + _south, tile);
                        _tilemap_grass_light.SetTile(coord + _east, tile);
                        _tilemap_grass_light.SetTile(coord + _west, tile);
                        break;
                }
                break;

            case MoistureCategory.Wet:
                switch (tempCat)
                {
                    case TemperatureCategory.Cold:
                        tile = _snow_light;
                        _tilemap_snow_light.SetTile(coord, tile);
                        _tilemap_snow_light.SetTile(coord + _north, tile);
                        _tilemap_snow_light.SetTile(coord + _south, tile);
                        _tilemap_snow_light.SetTile(coord + _east, tile);
                        _tilemap_snow_light.SetTile(coord + _west, tile);
                        break;
                    case TemperatureCategory.MidTemp:
                        tile = _swamp;
                        _tilemap_swamp.SetTile(coord, tile);
                        _tilemap_swamp.SetTile(coord + _north, tile);
                        _tilemap_swamp.SetTile(coord + _south, tile);
                        _tilemap_swamp.SetTile(coord + _east, tile);
                        _tilemap_swamp.SetTile(coord + _west, tile);
                        break;
                    case TemperatureCategory.Hot:
                        tile = _swamp_light;
                        _tilemap_swamp_light.SetTile(coord, tile);
                        _tilemap_swamp_light.SetTile(coord + _north, tile);
                        _tilemap_swamp_light.SetTile(coord + _south, tile);
                        _tilemap_swamp_light.SetTile(coord + _east, tile);
                        _tilemap_swamp_light.SetTile(coord + _west, tile);
                        break;
                }
                break;
        }

    }

    public void ClearAllBaseTiles()
    {
        _tilemap_darkland.ClearAllTiles();
        _tilemap_pack.ClearAllTiles();
        _tilemap_sand.ClearAllTiles();
        _tilemap_grass.ClearAllTiles();
        _tilemap_grass_light.ClearAllTiles();
        _tilemap_swamp.ClearAllTiles();
        _tilemap_swamp_light.ClearAllTiles();
        _tilemap_snow.ClearAllTiles();
        _tilemap_snow_light.ClearAllTiles();
    }

    public void ClearBaseTilesAtCoord(Vector3Int coord)
    {
        _tilemap_darkland.SetTile(coord, null);
        _tilemap_pack.SetTile(coord, null);
        _tilemap_sand.SetTile(coord, null);
        _tilemap_grass.SetTile(coord, null);
        _tilemap_grass_light.SetTile(coord, null);
        _tilemap_swamp.SetTile(coord, null);
        _tilemap_swamp_light.SetTile(coord, null);
        _tilemap_snow.SetTile(coord, null);
        _tilemap_snow_light.SetTile(coord, null);
    }

    private void RenderPopulationTile(Vector3Int coord, TileStats td)
    {
        if (td.Population > _denseThreshold)
        {
            _tilemap_population_heavy.SetTile(coord, _pavers_dense);
            _tilemap_population_light.SetTile(coord, _pavers_middle);
        }
        else if (td.Population >= _sparseThreshold)
        {
            _tilemap_population_heavy.SetTile(coord, null);
            _tilemap_population_light.SetTile(coord, _pavers_middle);
        }
        else
        {
            _tilemap_population_heavy.SetTile(coord, null);
            _tilemap_population_light.SetTile(coord, null);
        }

    }

    private void RenderTrafficTile(Vector3Int coord, TileStats td)
    {
        if (td.Traffic >= 0.5f)
        {
            _tilemap_traffic.SetTile(coord, _path_brown);
        }
        else
        {
            _tilemap_traffic.SetTile(coord, null);
        }

    }

    private void RenderVegetationTile(Vector3Int coord, TileStats td)
    {
        _tilemap_vegetation.SetTile(coord, null);
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
}
