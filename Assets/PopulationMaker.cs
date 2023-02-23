using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PopulationMaker : MonoBehaviour
{
    System.Random _rnd;
    Action AllCitiesHaveBeenFounded;
    TileStatsHolder _tsh;

    [SerializeField] Tilemap _tilemap_population = null;

    [SerializeField] TileBase _house_blueroof = null;
    [SerializeField] TileBase _house_tanroof = null;
    [SerializeField] TileBase _house_desert = null;
    [SerializeField] TileBase _house_snowy = null;
    [SerializeField] TileBase _house_swamp = null;

    //settings
    [SerializeField] int _populationGridSize = 5;
    [SerializeField] int _maxTribeMigrationDistance = 40;
    [SerializeField] int _maxCitySize = 40;

    //state
    List<ParameterGrid> _popGrids;
    List<Tribe> _tribes;
    List<City> _cities = new List<City>();
    WaitForSeconds delay = new WaitForSeconds(.0625f);

    private void Start()
    {
        _rnd = new System.Random(RandomController.Instance.CurrentSeed);
        AllCitiesHaveBeenFounded += GrowAllCities;
        _tsh = TileStatsHolder.Instance;
    }

    public void Populate()
    {
        Debug.Log("Populating");
        _cities.Clear();
        _tilemap_population.ClearAllTiles();
        _popGrids = CreatePopulationGrids();
        _tribes = CreateTribesWithinPopulationGrids();
        MigrateEachTribe();

    }

    private List<ParameterGrid> CreatePopulationGrids()
    {
      
        List<ParameterGrid> wglist = new List<ParameterGrid>();

        int size = TileStatsHolder.Instance.TilemapDimensions;
        Vector2Int origin = Vector2Int.zero;
        Vector2Int farCorner = Vector2Int.zero;
        Vector2Int highPoint = Vector2Int.zero;
        float population = 0;
        for (int x = 0; x < size; x += _populationGridSize)
        {
            for (int y = 0; y < size; y += _populationGridSize)
            {
                origin.x = x;
                origin.y = y;

                if (origin.x + _populationGridSize - 1 < size)
                {
                    farCorner.x = origin.x + _populationGridSize - 1;
                }
                else
                {
                    farCorner.x = size - 1;
                }
                if (origin.y + _populationGridSize - 1 < size)
                {
                    farCorner.y = origin.y + _populationGridSize - 1;
                }
                else
                {
                    farCorner.y = size - 1;
                }

                highPoint = TileStatsHolder.Instance.FindLeastPopulatedCellsWithinGrid(origin, farCorner);
                population = TileStatsHolder.Instance.FindTotalPopulationWithinPopulationGrid(origin, farCorner);
                ParameterGrid wg = new ParameterGrid(origin, farCorner, highPoint, population);
                wglist.Add(wg);
            }
        }

        return wglist;

    }

    private List<Tribe> CreateTribesWithinPopulationGrids()
    {
        List<Tribe> tlist = new List<Tribe>();
        foreach (var popgrid in _popGrids)
        {
            Tribe tribe = new Tribe(popgrid.TotalValue, popgrid.SignificantCoord);
            tlist.Add(tribe);
        }
        return tlist;
    }
    
    private void MigrateEachTribe()
    {
        foreach (var tribe in _tribes)
        {
            StartCoroutine(nameof(MigrateTribe), tribe);
        }
    }

    IEnumerator MigrateTribe(Tribe tribe)
    {
        float volume = tribe.Population;
        Vector2Int coord = tribe.StartCoord;
        Vector2Int previousCoord = new Vector2Int(-9999, -9999);

        //Depict tribe here?

        int breaker = _maxTribeMigrationDistance; ;
        while (true) //volume > 0)
        {
            breaker--;
            if (breaker <= 0)
            {
                Debug.Log("Max Migration Reached");
                break;
            }

            previousCoord = coord;
            coord =
                TileStatsHolder.Instance.
                FindNeighborCellWithHigherPopulationValue(
                    coord.x, coord.y);
            if (previousCoord == coord)
            {
                Debug.Log("Local Minimum Found");
                HandleLocalMinimumFound(coord, volume);
                CompleteMigration(tribe);
                //FoundCityAtLocalMaxima(coord, volume);
                //CompleteStreamFlow(spring);
                break;
            }
        }

        yield return delay;
    }

    private void HandleLocalMinimumFound(Vector2Int coord, float volume)
    {
        City possibleCity;
        if (!CheckForExistingCity(coord, out possibleCity))
        {
            FoundNewCity(coord, volume);
        }
        else
        {
            AddToExistingCity(possibleCity, volume);
        }
    }

    private void GrowAllCities()
    {
        foreach (var city in _cities)
        {
            StartCoroutine(nameof(GrowCity), city);
        }
    }

    IEnumerator GrowCity(City city)
    {
        float pop = city.Population;
        Vector2Int currentSpot = city.StartCoord;
        int iteration = 0;

        while (pop > 0)
        {
            //Debug.Log($"{iteration/40f} vs {_rnd.NextDouble()}");
            Vector2Int testSpot =
                _tsh.GetCoordinatesForPopulationArrayFromSpiralCoord(city.StartCoord, iteration);
            if (_tsh.CheckIfCellIsHabitable(testSpot) &&
                (_maxCitySize - iteration)/ (float)_maxCitySize >
                (float)_rnd.NextDouble())
            {
                pop -= 20;
                currentSpot = testSpot;
                PlaceCityTile(currentSpot);
            }
            else
            {
                //Debug.Log($"{city.CityName} tried to grow and failed");
            }

            iteration++;
            if (iteration > _maxCitySize)
            {
                //Debug.Log("Max City Iterations Reached");
                break;
            }

            yield return delay;
        }
    }



    #region Helpers

    private void PlaceCityTile(Vector2Int currentSpot)
    {
        TileStatsHolder.BiomeCategory bc = _tsh.GetBiomeCategoryAtCoord(currentSpot.x, currentSpot.y);
        switch (bc)
        {
            case TileStatsHolder.BiomeCategory.ColdMidwet:
                _tilemap_population.SetTile((Vector3Int)currentSpot, _house_snowy);
                break;

            case TileStatsHolder.BiomeCategory.ColdWet:
                _tilemap_population.SetTile((Vector3Int)currentSpot, _house_snowy);
                break;

            case TileStatsHolder.BiomeCategory.HotDry:
                _tilemap_population.SetTile((Vector3Int)currentSpot, _house_desert);
                break;

            case TileStatsHolder.BiomeCategory.HotMidwet:
                _tilemap_population.SetTile((Vector3Int)currentSpot, _house_swamp);
                break;

            case TileStatsHolder.BiomeCategory.HotWet:
                _tilemap_population.SetTile((Vector3Int)currentSpot, _house_swamp);
                break;

            case TileStatsHolder.BiomeCategory.ColdDry:
                _tilemap_population.SetTile((Vector3Int)currentSpot, _house_tanroof);
                break;

            case TileStatsHolder.BiomeCategory.MidtempDry:
                _tilemap_population.SetTile((Vector3Int)currentSpot, _house_tanroof);
                break;

            default:
                _tilemap_population.SetTile((Vector3Int)currentSpot, _house_blueroof);
                break;
        }

        
    }

    private bool CheckForExistingCity(Vector2Int coord, out City possibleCity)
    {
        foreach (var city in _cities)
        {
            if (city.StartCoord == coord)
            {
                possibleCity = city;
                return true;
            }
        }

        possibleCity = null;
        return false;
    }
    private void FoundNewCity(Vector2Int coord, float population)
    {
        string newName = $"City #{_rnd.Next(100)}";
        City newCity = new City(newName, population, coord);
        _cities.Add(newCity);
        Vector3Int np = new Vector3Int(coord.x, coord.y, 0);
        if (_tsh.CheckIfCellIsHabitable(coord))
        {
            PlaceCityTile(coord);
        }

        //Debug.Log($"{newName} created with {population} people at {coord.x},{coord.y}");
    }

    private void AddToExistingCity(City possibleCity, float volume)
    {
        possibleCity.Population += volume;
        //Debug.Log($"{possibleLake.LakeName} increased to {possibleLake.Volume} water");
    }
    private void CompleteMigration(Tribe tribe)
    {
        tribe.HasCompletedMigration = true;

        bool allMigrationsAreCompleted = true;
        foreach (var tri in _tribes)
        {
            if (tri.HasCompletedMigration == false)
            {
                allMigrationsAreCompleted = false;
                break;
            }
        }

        if (allMigrationsAreCompleted)
        {
            AllCitiesHaveBeenFounded?.Invoke();
        }
    }

    #endregion

}

public class City
{
    public string CityName;
    public float Population;
    public Vector2Int StartCoord;

    public City (string cityName, float population, Vector2Int startCoord)
    {
        this.CityName = cityName;
       this.Population = population;
        this.StartCoord = startCoord;
    }
}

public class Tribe
{
    public float Population;
    public Vector2Int StartCoord;
    public bool HasCompletedMigration;

    public Tribe(float population, Vector2Int startCoord)
    {
        this.Population = population;
        this.StartCoord = startCoord;
        HasCompletedMigration = false;
    }
}
