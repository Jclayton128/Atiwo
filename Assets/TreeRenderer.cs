using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TreeRenderer : MonoBehaviour
{
    public static TreeRenderer Instance;
    TileStatsHolder _tsh;
    System.Random _rnd;

    [SerializeField] Tilemap _tilemap_vegetation = null;

    // For (midtemp, midwet) (hot, midwet)
    [SerializeField] TileBase _tree_temperate = null;

    // For (cold, dry)
    [SerializeField] TileBase _tree_evergreen = null;
    
    // for (midtep, dry) (hot, dry)
    [SerializeField] TileBase _tree_desert = null;
    
    // for (cold, midwet) (cold, wet)
    [SerializeField] TileBase _tree_snowy = null;
    
    // for (midtemp, wet) (hot, wet)
    [SerializeField] TileBase _tree_mangrove = null;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _tsh = TileStatsHolder.Instance;        
    }

    public void ClearVegetationTilemap()
    {
        _tilemap_vegetation.ClearAllTiles();
    }

    public void RenderAllTrees()
    {
        _rnd = new System.Random(RandomController.Instance.CurrentSeed);
        StartCoroutine(nameof(RenderTrees));
    }

    IEnumerator RenderTrees()
    {
        for (int x = 1; x < TileStatsHolder.Instance.Dimension - 1; x++)
        {
            for (int y = 1; y < TileStatsHolder.Instance.Dimension - 1; y++)
            {
                Vector2Int np = new Vector2Int(x, y);
                if (WaterRenderer.Instance.CheckIfHasWaterTileAtCoord(np)) continue;
                if (MountainRenderer.Instance.CheckForMountainsOrHillsAtCoord(np)) continue;
                if (!_tsh.CheckIfNeighborsAreSameBiomeCategory(x, y,
                    _tsh.GetBiomeCategoryAtCoord(x, y))) continue;

                float chance = _tsh.GetVegetationChanceAtCoord(x, y);
                if (_rnd.NextDouble() <= chance)
                {
                    PlaceTreeAtCoord(x, y);
                } 
            }
            yield return new WaitForEndOfFrame();
        }

    }

    private void PlaceTreeAtCoord(int x, int y)
    {
        Vector3Int np = new Vector3Int(x, y, 0);
        TileStatsHolder.BiomeCategory bc = _tsh.GetBiomeCategoryAtCoord(x, y);
        switch (bc)
        {
            case TileStatsHolder.BiomeCategory.ColdDry:
                _tilemap_vegetation.SetTile(np, _tree_evergreen);
                break;

            case TileStatsHolder.BiomeCategory.MidtempMidwet:
                _tilemap_vegetation.SetTile(np, _tree_temperate);
                break;

            case TileStatsHolder.BiomeCategory.HotMidwet:
                _tilemap_vegetation.SetTile(np, _tree_mangrove);
                break;

            case TileStatsHolder.BiomeCategory.MidtempDry:
                _tilemap_vegetation.SetTile(np, _tree_desert);
                break;

            case TileStatsHolder.BiomeCategory.HotDry:
                _tilemap_vegetation.SetTile(np, _tree_desert);
                break;

            case TileStatsHolder.BiomeCategory.ColdMidwet:
                _tilemap_vegetation.SetTile(np, _tree_snowy);
                break;

            case TileStatsHolder.BiomeCategory.ColdWet:
                _tilemap_vegetation.SetTile(np, _tree_snowy);
                break;

            case TileStatsHolder.BiomeCategory.MidtempWet:
                _tilemap_vegetation.SetTile(np, _tree_temperate);
                break;

            case TileStatsHolder.BiomeCategory.HotWet:
                _tilemap_vegetation.SetTile(np, _tree_mangrove);
                break;
        }


    }

    private void RenderVegetationTile(Vector3Int coord, TileStats td)
    {
        _tilemap_vegetation.SetTile(coord, null);
    }

    public bool CheckForTreesAtCoord(Vector2Int coord)
    {
        return _tilemap_vegetation.HasTile((Vector3Int)coord);
    }


}
