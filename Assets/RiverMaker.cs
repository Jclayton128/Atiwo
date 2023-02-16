using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverMaker : MonoBehaviour
{
    [SerializeField] 
    [Tooltip("Approximately how many tiles the river will go before terminating")]
    float _startingVolume = 10; 

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
            currentSpot = AdvanceRiver(currentSpot, remainingVolume);
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


    private (int, int) AdvanceRiver((int, int) currentRiverLocation, float volume)
    {
        (int,int) neighbor = TileStatsHolder.Instance.FindNeighborCoordWithGreatestElevationIncrease(
            currentRiverLocation.Item1, currentRiverLocation.Item2);

        if (volume > .8f * _startingVolume)
        {
            //Create fat river, and reduce elevation here to below water.
            float waterlevel = TileStatsRenderer.Instance.WaterThreshold;
            TileStatsHolder.Instance.SetElevationAtTile(
                currentRiverLocation.Item1, currentRiverLocation.Item2,
                waterlevel);
            Vector3Int newloc = new Vector3Int(neighbor.Item1, neighbor.Item2, 0);
            TileStatsRenderer.Instance.RenderRiverTile(newloc);
        }
        else
        {
            //create stream
            TileStatsHolder.Instance.ModifyStreamStatusAtTile(
            neighbor.Item1, neighbor.Item2, true);
            Vector3Int newloc = new Vector3Int(neighbor.Item1, neighbor.Item2, 0);
            TileStatsRenderer.Instance.RenderStreamTile(newloc);
        }
        
        return neighbor;
    }

    private (int,int) FindStartingSpot()
    {
        //return a shallow water tile that is at least a certain distance away
        // from an existing starting spot.
        return TileStatsHolder.Instance.FindRandomBeachCoord();
    }
}
