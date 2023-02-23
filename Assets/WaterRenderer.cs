using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaterRenderer : MonoBehaviour
{
    public static WaterRenderer Instance;
    private Action AllSpringsHaveCompleted;

    List<ParameterGrid> _waterGrids;
    List<WaterSpring> _springs;
    List<Lake> _lakes = new List<Lake>();
    [SerializeField] Tilemap _tilemap_water = null;
    [SerializeField] TileBase _stream = null;
    [SerializeField] TileBase _water = null;
    System.Random rnd;

    //settings
    [SerializeField] int _waterGridSize = 5;

    [SerializeField] 
    [Tooltip("Affects how much river volume is lost per tile." +
        " Higher leads to shorter rivers.")]
        float _soilSoakupFactor = 0.6f;

    int _maxRiverLength = 100;
    int _maxLakeSize = 300;
    [SerializeField] float _uphillTolerance = 0.1f;
    Vector3Int _north = new Vector3Int(0, 1, 0);
    Vector3Int _south = new Vector3Int(0, -1, 0);
    Vector3Int _east = new Vector3Int(1, 0, 0);
    Vector3Int _west = new Vector3Int(-1, 0, 0);

    //state
    WaitForSeconds delay = new WaitForSeconds(.0625f);

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        rnd = new System.Random(RandomController.Instance.CurrentSeed);
        AllSpringsHaveCompleted += HandleSpringsCompleted;
    }

    private void HandleSpringsCompleted()
    {
        TileStatsRandomizer.Instance.InjectElevationNoise();
        FlowEachLake();
    }

    [ContextMenu("Create Water")]
    public void CreateWater()
    {
        _lakes.Clear();
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
        //FlowEachLake();
    }

    #region Create Water Flow
    private List<ParameterGrid> CreateWaterGrids()
    {
        List<ParameterGrid> wglist = new List<ParameterGrid>();

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

                highPoint = TileStatsHolder.Instance.FindHighestElevationCellWithinGrid(origin, farCorner);
                waterVolume = TileStatsHolder.Instance.FindTotalMoistureWithinWaterGrid(origin, farCorner);
                ParameterGrid wg = new ParameterGrid(origin, farCorner, highPoint, waterVolume);
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
            WaterSpring spring = new WaterSpring(wg.SignificantCoord, wg.TotalValue);
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
        while ( true) //volume > 0)
        {
            previousCoord = coord;
            coord = 
                TileStatsHolder.Instance.
                FindNeighborCoordsWithGreatestElevationDecrease(
                    coord.x, coord.y, _uphillTolerance);
            if (previousCoord == coord)
            {
                //Debug.Log("Local Minimum Found");
                HandleLocalMinimumFound(coord, volume);
                CompleteStreamFlow(spring);
                break;
            }

            if (!hasMetNeighbor) PlaceStreamTile(coord);

            //if (TileStatsHolder.Instance.
            //    CheckStreamStatus(coord, previousCoord))
            //{
            //    hasMetNeighbor = true;
            //}

            //PlaceStreamTile(coord);


            float moisture = TileStatsHolder.Instance.GetMoistureAtCoord(
                coord.x, coord.y);
            volume += (moisture - _soilSoakupFactor);

            /// Disabling the feature where rivers can run dry before reaching
            /// a local minimum.
            //if (volume <= 0)
            //{
            //    CompleteStreamFlow(spring);
            //    Debug.Log("River ran dry");
            //    break;
            //}

            breaker--;
            if (breaker <= 0)
            {
                CompleteStreamFlow(spring);
                Debug.Log("Max River Length Reached");
                break;
            }
            yield return delay;
        }
    }

    private void CompleteStreamFlow(WaterSpring spring)
    {
        spring.HasCompletedFlow = true;

        bool allSpringsAreCompleted = true;
        foreach (var spg in _springs)
        {
            if (spg.HasCompletedFlow == false)
            {
                allSpringsAreCompleted = false;
                break;
            }
        }

        if (allSpringsAreCompleted)
        {
            AllSpringsHaveCompleted?.Invoke();
        }
    }

    private void FlowEachLake()
    {
        foreach (var lake in _lakes)
        {
            StartCoroutine(nameof(FlowLake), lake);
        }
    }

    IEnumerator FlowLake(Lake lake)
    {
        Dictionary<Vector2Int, float> naysByElevation = new Dictionary<Vector2Int, float>();

        int breaker = _maxLakeSize;
        Vector2Int currentCoord = lake.StartCoord;
        PlaceLakeTile(currentCoord, 1f);
        AddNewCoordsNeighborsByElevation(naysByElevation, currentCoord);

        while (lake.Volume > 0)
        {
            currentCoord = SelectLowestNeighborCoord(naysByElevation);
            naysByElevation.Remove(currentCoord);
            AddNewCoordsNeighborsByElevation(naysByElevation, currentCoord);

            //turn current coord into lake;
            float volumeToDevoteHere = 10f;
            PlaceLakeTile(currentCoord, volumeToDevoteHere);

            //reduce remaining volume;
            lake.Volume -= volumeToDevoteHere;
            //Debug.Log($"{lake.LakeName} has {lake.Volume} remaining, and" +
            //    $"has {naysByElevation.Count} neighbors to check.");
            if (lake.Volume <= 0)
            {
                //Debug.Log($"{lake.LakeName} is out of volume");
                break;
            }

            breaker--;
            if (breaker <= 0)
            {
                Debug.Log("Max Lake Size Reached");
                break;
            }

            yield return delay;
        }
    }

    private Vector2Int SelectLowestNeighborCoord(Dictionary<Vector2Int, float> naysByElevation)
    {
        float lowToBeat = float.MaxValue;
        Vector2Int currentLowest = Vector2Int.zero;

        foreach (var key in naysByElevation.Keys)
        {

            if (naysByElevation[key] >= 0 && 
                naysByElevation[key] < lowToBeat)
            {
                lowToBeat = naysByElevation[key];
                currentLowest = key;
            }
        }
        return currentLowest;
    }

    private static void AddNewCoordsNeighborsByElevation(Dictionary<Vector2Int, float> naysByElevation, Vector2Int currentCoord)
    {
        (Vector2Int, float)[] nb = TileStatsHolder.Instance.
            GetNeighborCellsWithElevationAndLowWater(currentCoord);
        for (int i = 0; i < nb.Length; i++)
        {
            if (naysByElevation.ContainsKey(nb[i].Item1)) continue;
            naysByElevation.Add(nb[i].Item1, nb[i].Item2);
        }
    }
    #endregion

    #region Helpers
    private void HandleLocalMinimumFound(Vector2Int coord, float volume)
    {
        Lake possibleLake;
        if (!CheckForExistingLake(coord, out possibleLake))
        {
            CreateNewLake(coord, volume);
        }
        else
        {
            AddToCurrentLake(possibleLake, volume);
        }
    }

    private bool CheckForExistingLake(Vector2Int coord, out Lake possibleLake)
    {
        foreach (var lake in _lakes)
        {
            if (lake.StartCoord == coord)
            {
                possibleLake = lake;
                return true;
            }
        }

        possibleLake = null;
        return false;
    }

    private void CreateNewLake(Vector2Int coord, float volume)
    {
        string newName = $"Lake #{rnd.Next(100)}";
        Lake newLake = new Lake(coord, volume, newName);
        _lakes.Add(newLake);
        PlaceLakeTile(coord, volume);
        //Debug.Log($"{newName} created with {volume} water");
    }

    private void AddToCurrentLake(Lake possibleLake, float volume)
    {
        possibleLake.Volume += volume;
        //Debug.Log($"{possibleLake.LakeName} increased to {possibleLake.Volume} water");
    }

    private void PlaceStreamTile(Vector2Int coord)
    {
        TileStatsHolder.Instance.ModifyWaterStatusAtTile(
            coord.x, coord.y, 0.01f, true);

        Vector3Int newloc = (Vector3Int)coord;
        _tilemap_water.SetTile(newloc, _stream);
    }

    private void PlaceLakeTile(Vector2Int coord, float volume)
    {
        TileStatsHolder.Instance.ModifyWaterStatusAtTile(
            coord.x, coord.y, volume, true);

        if (coord.y + 1 < TileStatsHolder.Instance.TilemapDimensions)
        {
            TileStatsHolder.Instance.ModifyWaterStatusAtTile(
                coord.x, coord.y + 1, 4f, false);
        }

        if (coord.x + 1 < TileStatsHolder.Instance.TilemapDimensions)
        {
            TileStatsHolder.Instance.ModifyWaterStatusAtTile(
            coord.x + 1, coord.y, 4f, false);
        }

        if (coord.y - 1 > 0)
        {
            TileStatsHolder.Instance.ModifyWaterStatusAtTile(
                coord.x, coord.y - 1, 4f, false);
        }

        if (coord.x - 1 > 0)
        {
            TileStatsHolder.Instance.ModifyWaterStatusAtTile(
                coord.x - 1, coord.y, 4f, false);
        }

        SetLakeTile((Vector3Int)coord);
    }
    
    public void SetLakeTile(Vector3Int coord)
    {
        //float volume = TileStatsHolder.Instance.
        //    GetWaterVolumeAtCoord(coord.x, coord.y);
        if (TileStatsHolder.Instance.CheckIfWaterShouldBePresentAtCoord(coord.x, coord.y))
        {
            _tilemap_water.SetTile(coord, _water);
            _tilemap_water.SetTile(coord + _north, _water);
            _tilemap_water.SetTile(coord + _south, _water);
            _tilemap_water.SetTile(coord + _west, _water);
            _tilemap_water.SetTile(coord + _east, _water);
            _tilemap_water.SetTile(coord + _north + _east, _water);
            _tilemap_water.SetTile(coord + _south + _east, _water);
            _tilemap_water.SetTile(coord + _west + _north, _water);
            _tilemap_water.SetTile(coord + _east + _north, _water);

            //if (volume > _deepwaterThreshold)
            //{
            //    _tilemap_water.SetTile(coord, _water_deep);
            //}
            //else
            //{
            //    _tilemap_water.SetTile(coord, _water);
            //}
        }


        //if (td.Elevation <= _deepwaterThreshold)
        //{
        //    _tilemap_water.SetTile(coord, _water_deep);
        //    //return;
        //}
        //else if (td.Elevation <= _waterThreshold)
        //{
        //    switch (GetBeachCategory(coord.x, coord.y))
        //    {
        //        case BeachCategory.None:
        //            _tilemap_water.SetTile(coord, _water);
        //            break;

        //        case BeachCategory.Brown:
        //            _tilemap_water.SetTile(coord, _water);
        //            break;

        //        case BeachCategory.Sand:
        //            _tilemap_water.SetTile(coord, _water);
        //            break;
        //    }


        //    //return;
        //}
        //else
        //{
        //    _tilemap_water.SetTile(coord, null);
        //}
    }

    #endregion


    public bool CheckIfHasWaterTileAtCoord(Vector2Int coord)
    {
        return _tilemap_water.HasTile((Vector3Int)coord);
    }
}



public class WaterSpring
{
    public Vector2Int StartCoord;
    public float StartVolume;
    public bool HasCompletedFlow;

    public WaterSpring(Vector2Int startCoord,float startVolume)
    {
        this.StartCoord = startCoord;
        this.StartVolume = startVolume;
        HasCompletedFlow = false;
    }


}

public class Lake 
{
    public Vector2Int StartCoord;
    public float Volume;
    public string LakeName;

    public Lake(Vector2Int startCoord, float volume, string lakeName)
    {
        this.StartCoord = startCoord;
        this.Volume = volume;
        this.LakeName = lakeName;
    }


}


