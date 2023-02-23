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
    //[SerializeField] Tilemap _tilemap_mountains = null;
    [SerializeField] Tilemap _tilemap_water = null;

    //[SerializeField] Tilemap _tilemap_population_light = null;
    //[SerializeField] Tilemap _tilemap_population_heavy = null;
    [SerializeField] Tilemap _tilemap_traffic = null;
    //[SerializeField] Tilemap _tilemap_vegetation = null;

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
    //[SerializeField] TileBase _mountain_odd = null;
    //[SerializeField] TileBase _mountain_even = null;
    //[SerializeField] TileBase _mountain_solitairy = null;
    //[SerializeField] TileBase _mountain_bottom_even = null;
    //[SerializeField] TileBase _mountain_bottom_odd = null;
    //[SerializeField] TileBase _water = null;
    //[SerializeField] TileBase _water_deep = null;
    //[SerializeField] TileBase _stream_slow = null;    

    [SerializeField] float _thresholdTolerance = 0.1f;
    [SerializeField] float _deepwaterThreshold = 0.15f;
    [SerializeField] float _waterThreshold = 0.3f;

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


    public void RenderAllCells()
    {
        Debug.Log("rendering all cells");
        ClearAllBaseTiles();
        for (int x = 0; x < TileStatsHolder.Instance.Dimension; x++)
        {
            for (int y = 0; y < TileStatsHolder.Instance.Dimension; y++)
            {
                Vector3Int coord = new Vector3Int(x, y, 0);
                RenderBaseTile(coord);
            }
        }
    }

    


    public void RenderSingleCellByCoord(int xCoord, int yCoord)
    {
        //TileStats td = TileStatsHolder.Instance.GetTileDataAtTileCoord(xCoord, yCoord);

        //Water is controlled by TileWaterMaker
        //RenderWaterTile(coord, td); 
        
        //RenderPopulationTile(coord, td);
        //RenderTrafficTile(coord, td);
        //RenderVegetationTile(coord, td);
    }

    private void RenderBaseTile(Vector3Int coord)
    {
        TileBase tile = _grass;
        TileStatsHolder.BiomeCategory bc = TileStatsHolder.Instance.GetBiomeCategoryAtCoord(coord.x, coord.y);
        switch (bc)
        {
        case TileStatsHolder.BiomeCategory.ColdDry:
            tile = _darkland;
            _tilemap_darkland.SetTile(coord, tile);
            _tilemap_darkland.SetTile(coord + _north, tile);
            _tilemap_darkland.SetTile(coord + _south, tile);
            _tilemap_darkland.SetTile(coord + _east, tile);
            _tilemap_darkland.SetTile(coord+_west, tile);
            break;

        case TileStatsHolder.BiomeCategory.MidtempDry:
                tile = _pack;
            _tilemap_pack.SetTile(coord, tile);
            _tilemap_pack.SetTile(coord + _north, tile);
            _tilemap_pack.SetTile(coord + _south, tile);
            _tilemap_pack.SetTile(coord + _east, tile);
            _tilemap_pack.SetTile(coord + _west, tile);
            break;

        case TileStatsHolder.BiomeCategory.HotDry:
                tile = _sand;
            _tilemap_sand.SetTile(coord, tile);
            _tilemap_sand.SetTile(coord + _north, tile);
            _tilemap_sand.SetTile(coord + _south, tile);
            _tilemap_sand.SetTile(coord + _east, tile);
            _tilemap_sand.SetTile(coord + _west, tile);
            break;

        case TileStatsHolder.BiomeCategory.ColdMidwet:
                tile = _snow;
            _tilemap_snow.SetTile(coord, tile);
            _tilemap_snow.SetTile(coord + _north, tile);
            _tilemap_snow.SetTile(coord + _south, tile);
            _tilemap_snow.SetTile(coord + _east, tile);
            _tilemap_snow.SetTile(coord + _west, tile);
            break;

        case TileStatsHolder.BiomeCategory.MidtempMidwet:
                tile = _grass;
            _tilemap_grass.SetTile(coord, tile);
            _tilemap_grass.SetTile(coord + _north, tile);
            _tilemap_grass.SetTile(coord + _south, tile);
            _tilemap_grass.SetTile(coord + _east, tile);
            _tilemap_grass.SetTile(coord + _west, tile);
            break;

        case TileStatsHolder.BiomeCategory.HotMidwet:
                tile = _swamp;
                _tilemap_swamp.SetTile(coord, tile);
                _tilemap_swamp.SetTile(coord + _north, tile);
                _tilemap_swamp.SetTile(coord + _south, tile);
                _tilemap_swamp.SetTile(coord + _east, tile);
                _tilemap_swamp.SetTile(coord + _west, tile);
                break;
                
        case TileStatsHolder.BiomeCategory.ColdWet:
                tile = _snow_light;
            _tilemap_snow_light.SetTile(coord, tile);
            _tilemap_snow_light.SetTile(coord + _north, tile);
            _tilemap_snow_light.SetTile(coord + _south, tile);
            _tilemap_snow_light.SetTile(coord + _east, tile);
            _tilemap_snow_light.SetTile(coord + _west, tile);
            break;

        case TileStatsHolder.BiomeCategory.MidtempWet:
                tile = _grass_light;
                _tilemap_grass_light.SetTile(coord, tile);
                _tilemap_grass_light.SetTile(coord + _north, tile);
                _tilemap_grass_light.SetTile(coord + _south, tile);
                _tilemap_grass_light.SetTile(coord + _east, tile);
                _tilemap_grass_light.SetTile(coord + _west, tile);

            break;

        case TileStatsHolder.BiomeCategory.HotWet:
                tile = _swamp_light;
            _tilemap_swamp_light.SetTile(coord, tile);
            _tilemap_swamp_light.SetTile(coord + _north, tile);
            _tilemap_swamp_light.SetTile(coord + _south, tile);
            _tilemap_swamp_light.SetTile(coord + _east, tile);
            _tilemap_swamp_light.SetTile(coord + _west, tile);
            break;
        }

    }

    

    //private BeachCategory GetBeachCategory(int xCoord, int yCoord)
    //{
    //    if ((TileStatsHolder.Instance.GetElevationAtCoord(xCoord +1, yCoord) <= _waterThreshold) ||
    //        (TileStatsHolder.Instance.GetElevationAtCoord(xCoord - 1, yCoord) <= _waterThreshold) ||
    //        (TileStatsHolder.Instance.GetElevationAtCoord(xCoord, yCoord + 1) <= _waterThreshold) ||
    //        (TileStatsHolder.Instance.GetElevationAtCoord(xCoord, yCoord - 1) <= _waterThreshold))
    //    {
    //        return BeachCategory.None;
    //    }

    //    return BeachCategory.None;
 
    //}

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
        MountainRenderer.Instance.ClearMountainTilemap();
        _tilemap_water.ClearAllTiles();
        TreeRenderer.Instance.ClearVegetationTilemap();

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

    //public void RenderRiverTile(Vector3Int coord)
    //{
    //    _tilemap_water.SetTile(coord, _water);
    //    _tilemap_water.SetTile(coord+_north, _water);
    //    _tilemap_water.SetTile(coord + _south, _water);
    //    _tilemap_water.SetTile(coord + _west, _water);
    //    _tilemap_water.SetTile(coord+_east, _water);
    //}

    public void RenderStreamTile(Vector3Int coord)
    {
 
    }

    //private void RenderPopulationTile(Vector3Int coord, TileStats td)
    //{
    //    if (td.Population > _denseThreshold)
    //    {
    //        _tilemap_population_heavy.SetTile(coord, _pavers_dense);
    //        _tilemap_population_light.SetTile(coord, _pavers_middle);
    //    }
    //    else if (td.Population >= _sparseThreshold)
    //    {
    //        _tilemap_population_heavy.SetTile(coord, null);
    //        _tilemap_population_light.SetTile(coord, _pavers_middle);
    //    }
    //    else
    //    {
    //        _tilemap_population_heavy.SetTile(coord, null);
    //        _tilemap_population_light.SetTile(coord, null);
    //    }

    //}

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


    
}
