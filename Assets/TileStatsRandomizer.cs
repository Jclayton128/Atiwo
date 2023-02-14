using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileStatsRandomizer : MonoBehaviour
{
    System.Random rnd;

    [SerializeField] float _noiseScale_macro = 1f;
    [SerializeField] float _noiseScale_micro = 1f;

    private void Start()
    {
        rnd = new System.Random(RandomController.Instance.CurrentSeed);
    }

    [ContextMenu("Randomly Populate Map")]
    public void RandomlyPopulatePrimaryStats()
    {
        float tempOffset_1 = (float)rnd.NextDouble();
        float tempOffset_2 = (float)rnd.NextDouble();
        float moistOffset_1 = (float)rnd.NextDouble();
        float moistOffset_2 = (float)rnd.NextDouble();
        float elevationOffset_1 = (float)rnd.NextDouble();
        float elevationOffset_2 = (float)rnd.NextDouble();
        //Debug.Log($"to: {tempOffset_1}. mo: {moistOffset_1}");

        TileStatsRenderer.Instance.ClearAllBaseTiles();

        Vector3Int coords = new Vector3Int(0, 0, 0);
        for (int x = 0; x < TileStatsHolder.Instance.Dimension; x++)
        {
            for (int y = 0; y < TileStatsHolder.Instance.Dimension; y++)
            {
                coords.x = x;
                coords.y = y;

                float temp =
                    Mathf.Clamp01(Mathf.PerlinNoise(
                        ((float)x / TileStatsHolder.Instance.Dimension * _noiseScale_macro) + tempOffset_1,
                        ((float)y / TileStatsHolder.Instance.Dimension * _noiseScale_macro) + tempOffset_1));

                temp +=
                    Mathf.Lerp(-.3f, .3f, (Mathf.PerlinNoise(
                        ((float)x / TileStatsHolder.Instance.Dimension * _noiseScale_micro) + tempOffset_2,
                        ((float)y / TileStatsHolder.Instance.Dimension * _noiseScale_micro) + tempOffset_2)));

                float moisture =
                    Mathf.Clamp01(Mathf.PerlinNoise(
                        ((float)x / TileStatsHolder.Instance.Dimension * _noiseScale_macro) + moistOffset_1,
                        ((float)y / TileStatsHolder.Instance.Dimension * _noiseScale_macro) + moistOffset_1));

                moisture +=
                    Mathf.Lerp(-.3f, .3f, (Mathf.PerlinNoise(
                        ((float)x / TileStatsHolder.Instance.Dimension * _noiseScale_micro) + moistOffset_2,
                        ((float)y / TileStatsHolder.Instance.Dimension * _noiseScale_micro) + moistOffset_2)));
                
                float elevation =
                    Mathf.Clamp01(Mathf.PerlinNoise(
                        ((float)x / TileStatsHolder.Instance.Dimension * _noiseScale_macro) + elevationOffset_1,
                        ((float)y / TileStatsHolder.Instance.Dimension * _noiseScale_macro) + elevationOffset_1));

                elevation +=
                    Mathf.Lerp(-.3f, .3f, (Mathf.PerlinNoise(
                        ((float)x / TileStatsHolder.Instance.Dimension * _noiseScale_micro) + elevationOffset_2,
                        ((float)y / TileStatsHolder.Instance.Dimension * _noiseScale_micro) + elevationOffset_2)));

                if (elevation <= TileStatsRenderer.Instance.DeepwaterThreshold)
                {
                    TileStatsHolder.Instance.EnforceDeepWaterWithWaterAsNeighbor(coords);
                }

                TileStatsHolder.Instance.SetTemperatureAtTile(coords, temp);
                TileStatsHolder.Instance.SetMoistureAtTile(coords, moisture);
                TileStatsHolder.Instance.SetElevationAtTile(coords, elevation);
                TileStatsRenderer.Instance.RenderSingleCellByCoord(coords);
            }
        }
    }

}
