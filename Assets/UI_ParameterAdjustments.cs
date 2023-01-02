using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_ParameterAdjustments : MonoBehaviour
{
    public static UI_ParameterAdjustments Instance;

    [SerializeField] TextMeshProUGUI _moistureAdjustmentTMP = null;
    [SerializeField] TextMeshProUGUI _temperatureAdjustmentTMP = null;

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
}
