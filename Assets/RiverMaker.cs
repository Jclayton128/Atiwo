using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverMaker : MonoBehaviour
{
    [SerializeField] 
    [Tooltip("Approximately how many tiles the river will go before terminating")]
    float _riverVolume = 10f; 

    [ContextMenu("Create River")]
    public void CreateRiver()
    {
        // Find a low spot on the map that is water, and
        // that isn't too close to another river.
        // Make a river iteratively, going from the sea up to the local maxima,
        // or until the river runs out of water.

        (int,int) startingSpot = TileStatsHolder.Instance.FindRandomWaterSpot();
    }

    private Vector3Int FindStartingSpot()
    {
        return Vector3Int.zero;
    }
}
