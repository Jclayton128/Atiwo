using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_ParameterAdjustments : MonoBehaviour
{
    public static UI_ParameterAdjustments Instance;

    [SerializeField] TextMeshProUGUI _moistureAdjustmentTMP = null;
    [SerializeField] TextMeshProUGUI _temperatureAdjustmentTMP = null;
    [SerializeField] TextMeshProUGUI _populationAdjustmentTMP = null;
    [SerializeField] TextMeshProUGUI _trafficAdjustmentTMP = null;
    [SerializeField] TextMeshProUGUI _vegetationAdjustmentTMP = null;

    private void Awake()
    {
        Instance = this;
    }

    public void SetMoistureAdjustment(float moistureLevelAdjustment)
    {
        _moistureAdjustmentTMP.text = $"Moisture Change: {moistureLevelAdjustment}";
    }

    public void SetTemperatureAdjustment(float tempLevelAdjustment)
    {
        _temperatureAdjustmentTMP.text = $"Temp Change: {tempLevelAdjustment}";
    }

    public void SetPopulationAdjustment(float PopAdjustment)
    {
        _populationAdjustmentTMP.text = $"Pop Change: {PopAdjustment}";
    }
    public void SetTrafficAdjustment(float trafficLevelAdjustment)
    {
        _trafficAdjustmentTMP.text = $"Traffic Change: {trafficLevelAdjustment}";
    }
    public void SetVegetationAdjustment(float vegLevelAdjustment)
    {
        _vegetationAdjustmentTMP.text = $"Veg Change: {vegLevelAdjustment}";
    }
}
