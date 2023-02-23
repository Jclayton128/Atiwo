using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MountainRenderer : MonoBehaviour
{
    public static MountainRenderer Instance;
    [SerializeField] TileBase _mountain_odd = null;
    [SerializeField] TileBase _mountain_even = null;
    [SerializeField] TileBase _mountain_solitairy = null;
    //[SerializeField] TileBase _hill_grass_odd = null;
    [SerializeField] TileBase _hill_grass = null;
    //[SerializeField] TileBase _hill_snow_odd = null;
    [SerializeField] TileBase _hill_snow = null;
    //[SerializeField] TileBase _hill_sand_odd = null;
    [SerializeField] TileBase _hill_sand = null;
    //[SerializeField] TileBase _hill_pack_odd = null;
    [SerializeField] TileBase _hill_pack = null;
    [SerializeField] Tilemap _tilemap_mountains = null;

    TileStatsHolder _tsh;

    Vector3Int _north = new Vector3Int(0, 1, 0);
    Vector3Int _south = new Vector3Int(0, -1, 0);
    Vector3Int _east = new Vector3Int(1, 0, 0);
    Vector3Int _west = new Vector3Int(-1, 0, 0);

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        _tsh = TileStatsHolder.Instance;
    }

    public void ClearMountainTilemap()
    {
        _tilemap_mountains.ClearAllTiles();
    }

    public void RenderAllMountains()
    {
        StartCoroutine(nameof(RenderMountains));
    }

    IEnumerator RenderMountains()
    {
        for (int x = 1; x < TileStatsHolder.Instance.Dimension - 1; x++)
        {
            for (int y = 1; y < TileStatsHolder.Instance.Dimension - 1; y++)
            {
                TileStatsHolder.BiomeCategory bc;
                if (_tsh.CheckIfWaterShouldBePresentAtCoord(x, y)) continue;
                else if (_tsh.CheckIfMountainShouldBePresentAtCoord(x, y))
                {
                    Vector3Int np = new Vector3Int(x, y, 0);
                    if ((x + y) % 2 == 0)
                    {
                        if (_tsh.CheckIfWaterShouldBePresentAtCoord(np.x,np.y)) continue;

                        if (_tsh.CheckIfWaterShouldBePresentAtCoord(np.x, np.y+1)||
                            _tsh.CheckIfWaterShouldBePresentAtCoord(np.x+1, np.y) ||
                            _tsh.CheckIfWaterShouldBePresentAtCoord(np.x, np.y - 1) ||
                            _tsh.CheckIfWaterShouldBePresentAtCoord(np.x-1, np.y))
                        {
                            if (!_tilemap_mountains.HasTile(np)) _tilemap_mountains.SetTile(np, _mountain_solitairy);
                        }
                        else
                        {
                            _tilemap_mountains.SetTile(np, _mountain_even);
                            _tilemap_mountains.SetTile(np + _east, _mountain_odd);
                            _tilemap_mountains.SetTile(np + _north, _mountain_odd);
                            _tilemap_mountains.SetTile(np + _north + _east, _mountain_even);
                            _tsh.SetElevationAtTileToMountainValue(np.x, np.y);
                            _tsh.SetElevationAtTileToMountainValue(np.x + 1, np.y);

                            //TileStatsHolder.Instance.SetElevationAtTileToMountainValue(np.x, np.y+1);
                            //TileStatsHolder.Instance.SetElevationAtTileToMountainValue(np.x+1, np.y+1);
                        }

                    }
                }
                else if (TileStatsHolder.Instance.CheckIfHillShouldBePresentAtCoord(x, y, out bc))
                {
                    Vector3Int np = new Vector3Int(x, y, 0);
                    if (_tilemap_mountains.HasTile(np)) continue;
                    switch (bc)
                    {
                        case TileStatsHolder.BiomeCategory.MidtempDry: //pack
                            _tilemap_mountains.SetTile(np, _hill_pack);
                            //if ((x + y) % 2 == 0) _tilemap_mountains.SetTile(np, _hill_pack_even);
                            //else _tilemap_mountains.SetTile(np, _hill_pack_odd);
                            break;

                        case TileStatsHolder.BiomeCategory.HotDry: //sand
                            _tilemap_mountains.SetTile(np, _hill_sand);
                            //if ((x + y) % 2 == 0) _tilemap_mountains.SetTile(np, _hill_sand_even);
                            //else _tilemap_mountains.SetTile(np, _hill_sand_odd);
                            break;

                        case TileStatsHolder.BiomeCategory.MidtempMidwet: //grass
                            _tilemap_mountains.SetTile(np, _hill_grass);
                            //if ((x + y) % 2 == 0) _tilemap_mountains.SetTile(np, _hill_grass_even);
                            //else _tilemap_mountains.SetTile(np, _hill_grass_odd);
                            break;

                        case TileStatsHolder.BiomeCategory.ColdMidwet: //snow
                            _tilemap_mountains.SetTile(np, _hill_snow);
                            //if ((x + y) % 2 == 0) _tilemap_mountains.SetTile(np, _hill_snow_even);
                            //else _tilemap_mountains.SetTile(np, _hill_snow_odd);
                            break;

                    }
                }

            }
            yield return new WaitForEndOfFrame();
        }

    }

    public bool CheckForMountainsOrHillsAtCoord(Vector2Int coord)
    {
        return _tilemap_mountains.HasTile((Vector3Int)coord);
    }
}
