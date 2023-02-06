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

    private enum MoistureCategory { Dry, Damp, Wet}
    private enum TemperatureCategory { Cold, Warm, Hot}


    public static TileStatsRenderer Instance;

    [SerializeField] Tilemap _tilemap_base = null;
    [SerializeField] Tilemap _tilemap_population_light = null;
    [SerializeField] Tilemap _tilemap_population_heavy = null;
    [SerializeField] Tilemap _tilemap_traffic = null;
    [SerializeField] Tilemap _tilemap_vegetation = null;

    //settings
    [Header("Base Tile Examples")]
    [SerializeField] TileBase _dryCold_base = null;
    [SerializeField] TileBase _dryWarm_base = null;
    [SerializeField] TileBase _dryHot_base = null;

    [SerializeField] TileBase _dampCold_base = null;
    [SerializeField] TileBase _dampWarm_base = null;
    [SerializeField] TileBase _dampHot_base = null;

    [SerializeField] TileBase _wetCold_base = null;
    [SerializeField] TileBase _wetWarm_base = null;
    [SerializeField] TileBase _wetHot_base = null;

    [Header("Temperature/Moisture Thresholds")]
    [SerializeField] float _dryThreshold = 0.3f;
    [SerializeField] float _wetThreshold = 0.7f;
    [SerializeField] float _coldThreshold = 0.3f;
    [SerializeField] float _hotThreshold = 0.7f;

    [Header("Population Tile Examples")]
    [SerializeField] TileBase _pavers_middle = null;
    [SerializeField] TileBase _pavers_dense = null;

    [Header("Population Thresholds")]
    [SerializeField] float _sparseThreshold = 0.3f;
    [SerializeField] float _denseThreshold = 0.7f;

    [Header("Path Tile Examples")]
    [SerializeField] TileBase _path_brown = null;

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
        TileBase tile = _dryCold_base;
        switch (moistureCategory)
        {
            case MoistureCategory.Dry:
                switch (tempCat)
                {
                    case TemperatureCategory.Cold:
                        tile = _dryCold_base;
                        break;
                    case TemperatureCategory.Warm:
                        tile = _dryWarm_base;
                        break;
                    case TemperatureCategory.Hot:
                        tile = _dryHot_base;
                        break;
                }
                break;

            case MoistureCategory.Damp:
                switch (tempCat)
                {
                    case TemperatureCategory.Cold:
                        tile = _dampCold_base;
                        break;
                    case TemperatureCategory.Warm:
                        tile = _dampWarm_base;
                        break;
                    case TemperatureCategory.Hot:
                        tile = _dampHot_base;
                        break;
                }
                break;

            case MoistureCategory.Wet:
                switch (tempCat)
                {
                    case TemperatureCategory.Cold:
                        tile = _wetCold_base;
                        break;
                    case TemperatureCategory.Warm:
                        tile = _wetWarm_base;
                        break;
                    case TemperatureCategory.Hot:
                        tile = _wetHot_base;
                        break;
                }
                break;
        }
        _tilemap_base.SetTile(coord, tile);
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
        else return MoistureCategory.Damp;
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
        else return TemperatureCategory.Warm;
    }
}
