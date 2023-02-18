using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_TileInspector : MonoBehaviour
{
    public static UI_TileInspector Instance;

    [SerializeField] TextMeshProUGUI _tileCoords = null;
    [SerializeField] TextMeshProUGUI _temperatureTMP = null;
    [SerializeField] TextMeshProUGUI _moistureTMP = null;
    [SerializeField] TextMeshProUGUI _populationTMP = null;
    [SerializeField] TextMeshProUGUI _trafficTMP = null;
    [SerializeField] TextMeshProUGUI _vegetationTMP = null;

    private void Awake()
    {
        Instance = this;
    }

    public void SetTileCoords(Vector3Int tileCoords)
    {
        _tileCoords.text = $"Tile Coord: {tileCoords.x}, {tileCoords.y}";
    }

    public void SetTemperature(float temperature)
    {
        _temperatureTMP.text = $"Temp: {temperature}";
    }

    public void SetMoisture(float moisture)
    {
        _moistureTMP.text = $"Moisture: {moisture}";
    }

    public void SetPopulation(float population)
    {
        _populationTMP.text = $"Pop: {population}";
    }

    public void SetTraffic(float traffic)
    {
        _trafficTMP.text = $"Traffic: {traffic}";
    }

    public void SetVegetation(float vegetation)
    {
        _vegetationTMP.text = $"Vegetation: {vegetation}";
    }

    public void SetElevation(float elevation)
    {
        _temperatureTMP.text = $"Elev: {elevation}";
    }

    public void SetWaterVolume(float volume)
    {
        _moistureTMP.text = $"H20: {volume}";
    }

}
