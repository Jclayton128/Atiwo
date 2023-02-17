using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileWaterMaker : MonoBehaviour
{
    List<WaterGrid> _waterGrids;
    List<WaterSpring> _springs;

    //settings
    [SerializeField] int _waterGridSize = 5;

    [SerializeField] 
    [Tooltip("Affects how much river volume is lost per tile." +
        " Higher leads to shorter rivers.")]
        float _soilSoakupFactor = 0.6f;

    int _maxRiverLength = 100;


    [ContextMenu("Create Water")]
    public void CreateWater()
    {
        // Gridify entire tilemap
        _waterGrids = CreateWaterGrids();
        // Place one spring in the highest point within each subgrid
        _springs = CreateSpringsWithinWaterGrids();

        // foreach spring, route it until it reaches a local minimum.
        FlowEachSpring();
        // Check for a Lake at the local minimum.
            // If there isn't a Lake there, make one.
            //If there is a Lake there, then add remaining volume to it.
        // foreach Lake, grow it tile-by-tile until its volume is used up.
    }

    private List<WaterGrid> CreateWaterGrids()
    {
        List<WaterGrid> wglist = new List<WaterGrid>();

        int size = TileStatsHolder.Instance.TilemapDimensions;
        Vector2Int origin = Vector2Int.zero;
        Vector2Int farCorner = Vector2Int.zero;
        Vector2Int highPoint = Vector2Int.zero;
        float waterVolume = 0;
        for (int x = 0; x < size; x += _waterGridSize)
        {
            for (int y = 0; y < size; y += _waterGridSize)
            {
                origin.x = x;
                origin.y = y;

                if (origin.x + _waterGridSize -1 < size)
                {
                    farCorner.x = origin.x + _waterGridSize-1;
                }
                else
                {
                    farCorner.x = size - 1;
                }
                if (origin.y + _waterGridSize -1< size)
                {
                    farCorner.y = origin.y + _waterGridSize-1;
                }
                else
                {
                    farCorner.y = size - 1;
                }

                highPoint = TileStatsHolder.Instance.FindHighestCellWithinWaterGrid(origin, farCorner);
                waterVolume = TileStatsHolder.Instance.FindWaterVolumeWithinWaterGrid(origin, farCorner);
                WaterGrid wg = new WaterGrid(origin, farCorner, highPoint, waterVolume);
                wglist.Add(wg);
            }
        }

        return wglist;
    }

    private List<WaterSpring> CreateSpringsWithinWaterGrids()
    {
        List<WaterSpring> wslist = new List<WaterSpring>();
        foreach (var wg in _waterGrids)
        {
            WaterSpring spring = new WaterSpring(wg.HighPoint, wg.WaterVolume);
            wslist.Add(spring);
        }
        return wslist;
    }

    private void FlowEachSpring()
    {
        foreach (var spring in _springs)
        {
            StartCoroutine(nameof(FlowSpring), spring);
        }
    }

    IEnumerator FlowSpring(WaterSpring spring)
    {
        bool hasMetNeighbor = false;
        float volume = spring.StartVolume;
        Vector2Int coord = spring.StartCoord;
        Vector2Int previousCoord = new Vector2Int(-9999, -9999);

        PlaceStreamTile(coord);

        int breaker = _maxRiverLength;
        while (volume > 0)
        {
            previousCoord = coord;
            coord = 
                TileStatsHolder.Instance.
                FindNeighborCoordsWithGreatestElevationDecrease(
                    coord.x, coord.y);
            if (previousCoord == coord)
            {
                Debug.Log("Local Minimum Found");
                break;
            }

            //if (!hasMetNeighbor) PlaceStreamTile(coord);

            //if (!TileStatsHolder.Instance.CheckStreamStatus(coord))
            //{
            //    hasMetNeighbor = true;
            //}

            PlaceStreamTile(coord);

            float moisture = TileStatsHolder.Instance.GetPrimaryStatsAtCoord(
                coord.x, coord.y).Item2;
            volume += (moisture - _soilSoakupFactor);

            if (volume <= 0)
            {
                Debug.Log("River ran dry");
            }

            breaker--;
            if (breaker <= 0)
            {
                Debug.LogWarning("Breaker engaged!");
                break;
            }
            yield return new WaitForSeconds(.5f);
        }
    }

    private void PlaceStreamTile(Vector2Int coord)
    {
        TileStatsHolder.Instance.ModifyStreamStatusAtTile(
            coord.x, coord.y, true);

        Vector3Int newloc = (Vector3Int)coord;
        TileStatsRenderer.Instance.RenderStreamTile(newloc);
    }


}

public class WaterGrid
{
    public Vector2Int Origin;
    public Vector2Int FarCorner;
    public Vector2Int HighPoint;
    public float WaterVolume;

    public WaterGrid(Vector2Int origin, Vector2Int farCorner, Vector2Int highPoint, float volume)
    {
        //Debug.Log($"new WaterGrid start: {origin.x},{origin.y}. Extends: {farCorner.x}, {farCorner.y}. Volume: {volume} " +
        //    $"High: {highPoint.x},{highPoint.y} ");
        this.Origin = origin;
        this.FarCorner = farCorner;
        this.HighPoint = highPoint;
        this.WaterVolume = volume;
    }
}

public class WaterSpring
{
    public Vector2Int StartCoord;
    public float StartVolume;

    public WaterSpring(Vector2Int startCoord,float startVolume)
    {
        this.StartCoord = startCoord;
        this.StartVolume = startVolume;
    }
}


