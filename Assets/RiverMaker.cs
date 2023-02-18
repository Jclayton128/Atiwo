using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverMaker : MonoBehaviour
{
    System.Random rnd;
    Vector2Int _north = new Vector2Int(0, 1);
    Vector2Int _south = new Vector2Int(0, -1);
    Vector2Int _east = new Vector2Int(1, 0);
    Vector2Int _west = new Vector2Int(-1, 0);

    [SerializeField] 
    [Tooltip("Approximately how many tiles the river will go before terminating")]
    float _startingVolume = 10; 

    [SerializeField]
    [Tooltip("Likelihood of forking per tile advanced")]
    float _forkChance = 0.2f;

    [SerializeField]
    [Tooltip("Likelihood of turning per tile advanced")]
    float _turningChance = 0.2f;

    private void Start()
    {
        rnd = new System.Random(RandomController.Instance.CurrentSeed);
    }


    [ContextMenu("Start River Creation")]
    public void StartRiverCreation()
    {
        StartCoroutine(nameof(CreateRiver), _startingVolume);        
    }

    IEnumerator CreateRiver(float startingVolume)
    {
        // Find a low spot on the map that is water, and
        // that isn't too close to another river.
        // Make a river iteratively, going from the sea up to the local maxima,
        // or until the river runs out of water.

        Vector2Int currentSpot = FindStartingSpot();

        float remainingVolume = startingVolume;
        int breaker = 100;
        while (remainingVolume > 0)
        {
            Vector2Int staticTest = currentSpot;
            currentSpot = AdvanceOriginalRiverOrStream(currentSpot, remainingVolume);
            if (currentSpot == staticTest) break;

            float moisture = TileStatsHolder.Instance.GetMoistureAtCoord(
                currentSpot.x, currentSpot.y);
            remainingVolume += (moisture - .9f);
            breaker--;
            if (breaker <= 0)
            {
                Debug.LogWarning("Breaker engaged!");
                break;
            }
            yield return new WaitForSeconds(.5f);
        }
    }

    IEnumerator CreateForkedStream(Vector2Int startLocation, float startingVolume)
    {
        float remainingVolume = startingVolume;
        int breaker = 100;

        while (remainingVolume > 0)
        {
            Vector2Int staticTest = startLocation;
            startLocation = AdvanceForkedStream(startLocation, remainingVolume);
            if (startLocation == staticTest) break;
            
            float moisture = TileStatsHolder.Instance.GetMoistureAtCoord(
                startLocation.x, startLocation.y);
            remainingVolume += (moisture - .9f);
            breaker--;
            if (breaker <= 0)
            {
                Debug.LogWarning("Breaker engaged!");
                break;
            }
            yield return new WaitForSeconds(.5f);
        }
    }

    private Vector2Int AdvanceOriginalRiverOrStream(Vector2Int currentRiverLocation, float volume)
    {
        Vector2Int[] neighbors = TileStatsHolder.Instance.FindNeighborCoordsWithGreatestElevationIncrease(
            currentRiverLocation.x, currentRiverLocation.y);

        if (volume > .8f * _startingVolume)
        {
            //Create fat river, and reduce elevation here to below water.
            float waterlevel = TileStatsRenderer.Instance.WaterThreshold;
            TileStatsHolder.Instance.SetElevationAtTile(
                currentRiverLocation.x, currentRiverLocation.y,
                waterlevel);
            Vector3Int newloc = new Vector3Int(neighbors[0].x, neighbors[0].y, 0);
            TileStatsRenderer.Instance.RenderRiverTile(newloc);
        }
        else
        {
            //create stream
            TileStatsHolder.Instance.ModifyWaterStatusAtTile(
            neighbors[0].x, neighbors[0].y, .1f, true);

            Vector3Int newloc = new Vector3Int(neighbors[0].x, neighbors[0].y, 0);
            TileStatsRenderer.Instance.RenderStreamTile(newloc);

            float forkRoll = (float)rnd.NextDouble();
            if (forkRoll <= _forkChance)
            {
                //Debug.Log("stream forking!");

                float volumeModifier = Mathf.Lerp(.3f, .9f, (float)rnd.NextDouble());
                IEnumerator forkCR = CreateForkedStream(
                    currentRiverLocation, volume * volumeModifier);
                StartCoroutine(forkCR);
            }
        }

        return neighbors[0];
    }

    private Vector2Int AdvanceForkedStream(Vector2Int forkStart, float volume)
    {
        //Debug.Log($"advancing fork at {forkStart}");


        Vector2Int[] neighbors = TileStatsHolder.Instance.FindNeighborCoordsWithGreatestElevationIncrease(
            forkStart.x, forkStart.y);

        //create stream following flattest route
        TileStatsHolder.Instance.ModifyWaterStatusAtTile(
        neighbors[1].x, neighbors[1].y, .1f, true);

        Vector3Int newloc = new Vector3Int(neighbors[1].x, neighbors[1].y, 0);
        TileStatsRenderer.Instance.RenderStreamTile(newloc);

        return neighbors[1];
    }

    private Vector2Int FindStartingSpot()
    {
        //return a shallow water tile that is at least a certain distance away
        // from an existing starting spot.
        return TileStatsHolder.Instance.FindRandomBeachCoord();
    }

    [ContextMenu("Create River 2nd Method")]
    public void CreateRiver_Method2()
    {
        StartCoroutine(nameof(CreateRiver_2), _startingVolume);
    }

    IEnumerator CreateRiver_2(float startingVolume)
    {

        Vector2Int currentSpot = FindStartingSpot();

        float remainingVolume = startingVolume;

        int breaker = 100;
        float moistureInTile =0;
        Vector2Int previousSpot = currentSpot;
        Vector2Int historicalDirection = new Vector2Int(0, 0);
        Vector2Int immediateDirection = new Vector2Int(0, 0);
        while (remainingVolume > 0)
        {
            if (currentSpot == previousSpot)
            {
                Vector2Int[] dirs = TileStatsHolder.Instance.FindNeighborCoordsWithGreatestElevationIncrease(
                    currentSpot.x, currentSpot.y);
                immediateDirection = dirs[0]; //pick an immediate direction towards steepest neighbor
            }
            else
            {
                immediateDirection = currentSpot - previousSpot;
            }

            previousSpot = currentSpot;
            //iteratively build the river
            currentSpot = ChooseNextStreamSpot(currentSpot, immediateDirection, ref historicalDirection);

            TileStatsHolder.Instance.ModifyWaterStatusAtTile(
                currentSpot.x, currentSpot.y, .1f, true);
            Vector3Int newloc = (Vector3Int)currentSpot;
            TileStatsRenderer.Instance.RenderStreamTile(newloc);


            moistureInTile = TileStatsHolder.Instance.GetMoistureAtCoord(
                currentSpot.x, currentSpot.y);
            remainingVolume += (moistureInTile - .9f);

            breaker--;
            if (breaker <= 0) break;
            yield return new WaitForSeconds(.5f);
        }
    }

    private Vector2Int ChooseNextStreamSpot(Vector2Int currentSpot, Vector2Int immediateDirection, ref Vector2Int historicalDirection)
    {
        float turn = (float)rnd.NextDouble();

        //Should work historical direction in here to ensure that it isn't a completely straight run?
        if (turn <= _turningChance) // turn
        {
            Vector2Int newSpot = currentSpot;
            float turnDir = (float)rnd.NextDouble();

            if (turnDir <= 0.5f) //turn CCW
            {
                newSpot += ProcessTurn(immediateDirection, false);
            }
            else //turn CW
            {
                newSpot += ProcessTurn(immediateDirection, true);
            }

            historicalDirection = Vector2Int.zero + newSpot;
            return newSpot;
        }
        else //advance in immediate direction
        {
            historicalDirection += immediateDirection;
            return currentSpot + immediateDirection;
        }
    }

    private Vector2Int ProcessTurn(Vector2Int previousDirection, bool clockwiseTurn)
    {
        if (clockwiseTurn)
        {
            if (previousDirection == _north) return _west;
            if (previousDirection == _east) return _north;
            if (previousDirection == _south) return _east;
            if (previousDirection == _west) return _south;
        }
        else
        {
            if (previousDirection == _north) return _east;
            if (previousDirection == _east) return _south;
            if (previousDirection == _south) return _west;
            if (previousDirection == _west) return _north;
        }
        Debug.LogError("Process Turn error");
        return _north;
    }
}
