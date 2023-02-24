using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileStatsRandomizer : MonoBehaviour
{
    public static TileStatsRandomizer Instance;
    System.Random rnd;


    [SerializeField] float _noiseScale_macro = 1f;
    [SerializeField] float _noiseScale_micro = 1f;
    [SerializeField] float _noiseScale_elevation = 1f;

    private void Awake()
    {
        Instance = this;
    }

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

        int size = TileStatsHolder.Instance.Dimension;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float distFromMidY = Mathf.Abs(size/2f - y);
                float temp = (.7f - distFromMidY/size);

                //float temp =
                //    Mathf.Clamp01(Mathf.PerlinNoise(
                //        ((float)x /  _noiseScale_macro) + tempOffset_1,
                //        ((float)y /  _noiseScale_macro) + tempOffset_1));
                temp +=
                    Mathf.Lerp(-.3f, .3f, (Mathf.PerlinNoise(
                        ((float)x / _noiseScale_macro) + tempOffset_1,
                        ((float)y / _noiseScale_macro) + tempOffset_1)));

                temp +=
                    Mathf.Lerp(-.1f, .1f, (Mathf.PerlinNoise(
                        ((float)x /  _noiseScale_micro) + tempOffset_1,
                        ((float)y / _noiseScale_micro) + tempOffset_1)));

                //temp = Mathf.Clamp01(temp);

                float moisture =
                    Mathf.Clamp01(Mathf.PerlinNoise(
                        ((float)x /  _noiseScale_macro) + moistOffset_1,
                        ((float)y /  _noiseScale_macro) + moistOffset_1));

                moisture +=
                    Mathf.Lerp(-.3f, .3f, (Mathf.PerlinNoise(
                        ((float)x /  _noiseScale_micro) + moistOffset_2,
                        ((float)y /  _noiseScale_micro) + moistOffset_2)));

                //moisture = Mathf.Clamp01(moisture);

                float elevation =
                    Mathf.Clamp01(Mathf.PerlinNoise(
                        ((float)x /  _noiseScale_macro) + elevationOffset_1,
                        ((float)y / _noiseScale_macro) + elevationOffset_1));

                elevation +=
                    Mathf.Lerp(-.1f, .1f, (Mathf.PerlinNoise(
                        ((float)x /  _noiseScale_elevation) + elevationOffset_2,
                        ((float)y /  _noiseScale_elevation) + elevationOffset_2)));


                TileStatsHolder.Instance.SetTemperatureAtTile(x,y, temp);
                TileStatsHolder.Instance.SetMoistureAtTile(x,y, moisture);
                TileStatsHolder.Instance.SetElevationAtTile(x,y, elevation);

                //well, population != elevation...
                TileStatsHolder.Instance.SetPopulationAtTile(x, y, elevation);

                //Chance of vegetation is highest at moderate temp, and higher moisture;
                float vegChance = (1f - (Mathf.Abs(.75f - moisture)) - (Mathf.Abs(.5f - temp)));
                TileStatsHolder.Instance.SetVegetationChanceAtTile(x, y, vegChance);

                TileStatsHolder.Instance.CategorizeTileAtCoord(x, y);
                //TileStatsRenderer.Instance.RenderSingleCellByCoord(x,y);
            }
        }
    }

    public void InjectElevationNoise()
    {
        float elevationOffset_3 = (float)rnd.NextDouble();
        float elevation;
        for (int x = 0; x < TileStatsHolder.Instance.Dimension; x++)
        {
            for (int y = 0; y < TileStatsHolder.Instance.Dimension; y++)
            {
                elevation = TileStatsHolder.Instance.GetElevationAtCoord(x, y);
                elevation +=
                    Mathf.Lerp(-.3f, .3f, (Mathf.PerlinNoise(
                        ((float)x / TileStatsHolder.Instance.Dimension * _noiseScale_elevation * 5) + elevationOffset_3,
                        ((float)y / TileStatsHolder.Instance.Dimension * _noiseScale_elevation * 5) + elevationOffset_3)));

                TileStatsHolder.Instance.SetElevationAtTile(x, y, elevation);
            }
        }
    }

}
