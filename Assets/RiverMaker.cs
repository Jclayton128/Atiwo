using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverMaker : MonoBehaviour
{
    System.Random rnd;

    [SerializeField] 
    [Tooltip("Approximately how many tiles the river will go before terminating")]
    float _startingVolume = 10; 

    [SerializeField]
    [Tooltip("Likelihood of forking per tile advanced")]
    float _forkChance = 0.2f;

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

        (int, int) currentSpot = FindStartingSpot();

        float remainingVolume = startingVolume;
        int breaker = 100;
        while (remainingVolume > 0)
        {
            (int, int) staticTest = currentSpot;
            currentSpot = AdvanceOriginalRiverOrStream(currentSpot, remainingVolume);
            if (currentSpot == staticTest) break;

            float moisture = TileStatsHolder.Instance.GetPrimaryStatsAtCoord(
                currentSpot.Item1, currentSpot.Item2).Item2;
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

    IEnumerator CreateForkedStream((int,int) startLocation, float startingVolume)
    {
        float remainingVolume = startingVolume;
        int breaker = 100;

        while (remainingVolume > 0)
        {
            (int, int) staticTest = startLocation;
            startLocation = AdvanceForkedStream(startLocation, remainingVolume);
            if (startLocation == staticTest) break;
            
            float moisture = TileStatsHolder.Instance.GetPrimaryStatsAtCoord(
                startLocation.Item1, startLocation.Item2).Item2;
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

    private (int, int) AdvanceOriginalRiverOrStream((int, int) currentRiverLocation, float volume)
    {
        (int,int)[] neighbors = TileStatsHolder.Instance.FindNeighborCoordsWithGreatestElevationIncrease(
            currentRiverLocation.Item1, currentRiverLocation.Item2);

        if (volume > .8f * _startingVolume)
        {
            //Create fat river, and reduce elevation here to below water.
            float waterlevel = TileStatsRenderer.Instance.WaterThreshold;
            TileStatsHolder.Instance.SetElevationAtTile(
                currentRiverLocation.Item1, currentRiverLocation.Item2,
                waterlevel);
            Vector3Int newloc = new Vector3Int(neighbors[0].Item1, neighbors[0].Item2, 0);
            TileStatsRenderer.Instance.RenderRiverTile(newloc);
        }
        else
        {
            //create stream
            TileStatsHolder.Instance.ModifyStreamStatusAtTile(
            neighbors[0].Item1, neighbors[0].Item2, true);

            Vector3Int newloc = new Vector3Int(neighbors[0].Item1, neighbors[0].Item2, 0);
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

    private (int,int) AdvanceForkedStream((int,int) forkStart, float volume)
    {
        //Debug.Log($"advancing fork at {forkStart}");


        (int, int)[] neighbors = TileStatsHolder.Instance.FindNeighborCoordsWithGreatestElevationIncrease(
            forkStart.Item1, forkStart.Item2);

        //create stream following flattest route
        TileStatsHolder.Instance.ModifyStreamStatusAtTile(
        neighbors[1].Item1, neighbors[1].Item2, true);

        Vector3Int newloc = new Vector3Int(neighbors[1].Item1, neighbors[1].Item2, 0);
        TileStatsRenderer.Instance.RenderStreamTile(newloc);

        return neighbors[1];
    }

    private (int,int) FindStartingSpot()
    {
        //return a shallow water tile that is at least a certain distance away
        // from an existing starting spot.
        return TileStatsHolder.Instance.FindRandomBeachCoord();
    }
}
