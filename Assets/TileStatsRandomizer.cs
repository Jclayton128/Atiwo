using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileStatsRandomizer : MonoBehaviour
{
    System.Random rnd;

    [SerializeField] float _noiseScale = 1f;

    private void Start()
    {
        rnd = new System.Random(RandomController.Instance.CurrentSeed);
    }

    [ContextMenu("Randomly Populate Map")]
    public void RandomlyPopulateTemperatureAndMoisture()
    {
        float tempOffset = (float)rnd.NextDouble();       
        float moistOffset = (float)rnd.NextDouble();
        Debug.Log($"to: {tempOffset}. mo: {moistOffset}");

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
                        ((float)x / TileStatsHolder.Instance.Dimension * _noiseScale) + tempOffset,
                        ((float)y / TileStatsHolder.Instance.Dimension * _noiseScale) + tempOffset));

                float moisture =
                    Mathf.Clamp01(Mathf.PerlinNoise(
                        ((float)x / TileStatsHolder.Instance.Dimension * _noiseScale) + moistOffset,
                        ((float)y / TileStatsHolder.Instance.Dimension * _noiseScale) + moistOffset));

                TileStatsHolder.Instance.SetTemperatureAtTile(coords, temp);
                TileStatsHolder.Instance.SetMoistureAtTile(coords, moisture);
                TileStatsRenderer.Instance.RenderSingleCellByCoord(coords);
            }
        }
    }
}
