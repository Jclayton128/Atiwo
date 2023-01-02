using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    //settings
    [SerializeField] Vector3 _offset = new Vector2(0.5f, 0.5f);
    float _parameterAdjustStepAmount = 0.1f;

    //state
    Vector2 _cursorWorldPos;
    Vector3Int _cursorCellCoord;

    float _tempAdjustAmount = 0;
    float _moistureAdjustAmount = 0;

    private void Update()
    {
        UpdateCursorInspection();
        UpdateCursorAdjustments_Debug();
        UpdateCursorClick();
    }

    private void UpdateCursorClick()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Mathf.Abs(_moistureAdjustAmount) > 0.1)
            {
                TileDataHolder.Instance.ModifyMoistureAtTile(_cursorCellCoord, _moistureAdjustAmount);
            }
            if (Mathf.Abs(_tempAdjustAmount) > 0.1)
            {
                TileDataHolder.Instance.ModifyTemperatureAtTile(_cursorCellCoord, _moistureAdjustAmount);
            }
            TileDataRenderer.Instance.RenderSingleCellByCoord(_cursorCellCoord);
        }
    }

    private void UpdateCursorAdjustments_Debug()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            _tempAdjustAmount += _parameterAdjustStepAmount;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            _tempAdjustAmount -= _parameterAdjustStepAmount;
        }
        if (Mathf.Abs(_tempAdjustAmount) < 0.1)
        {
            _tempAdjustAmount = 0;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            _moistureAdjustAmount += _parameterAdjustStepAmount;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            _moistureAdjustAmount -= _parameterAdjustStepAmount;
        }
        if (Mathf.Abs(_moistureAdjustAmount) < 0.1)
        {
            _moistureAdjustAmount = 0;
        }

        UI_ParameterAdjustments.Instance.SetMoistureAdjustment(_moistureAdjustAmount);
        UI_ParameterAdjustments.Instance.SetTemperatureAdjustment(_tempAdjustAmount);
    }

    private void UpdateCursorInspection()
    {
        _cursorWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + _offset;
        _cursorCellCoord = TileDataHolder.Instance.GetTileCoord(_cursorWorldPos);

        UI_TileInspector.Instance.SetTileCoords(_cursorCellCoord);
        TileData td = TileDataHolder.Instance.GetTileDataAtTileCoord(_cursorCellCoord);

        UI_TileInspector.Instance.SetTemperature(td.Temperature);
        UI_TileInspector.Instance.SetMoisture(td.Moisture);
        UI_TileInspector.Instance.SetPopulation(td.Population);
        UI_TileInspector.Instance.SetTraffic(td.Traffic);
        UI_TileInspector.Instance.SetVegetation(td.Vegetation);
    }

}
