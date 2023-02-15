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
    float _popAdjustAmount = 0;
    float _trafficAdjustAmount = 0;
    float _vegAdjustAmount = 0;

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
            if (Mathf.Abs(_moistureAdjustAmount) >= 0.1)
            {
                TileStatsHolder.Instance.ModifyMoistureAtTile(
                    _cursorCellCoord.x,
                    _cursorCellCoord.y,
                    _moistureAdjustAmount);
            }
            if (Mathf.Abs(_tempAdjustAmount) >= 0.1)
            {
                TileStatsHolder.Instance.ModifyTemperatureAtTile(
                    _cursorCellCoord.x,
                    _cursorCellCoord.y,
                    _tempAdjustAmount);
            }
            if (Mathf.Abs(_popAdjustAmount) >= 0.1)
            {
                TileStatsHolder.Instance.ModifyPopulationAtTile(
                    _cursorCellCoord.x,
                    _cursorCellCoord.y,
                    _popAdjustAmount);
            }
            if (Mathf.Abs(_trafficAdjustAmount) >= 0.1)
            {
                TileStatsHolder.Instance.ModifyTrafficAtTile(
                    _cursorCellCoord.x,
                    _cursorCellCoord.y,
                    _trafficAdjustAmount);
            }
            if (Mathf.Abs(_vegAdjustAmount) >= 0.1)
            {
                TileStatsHolder.Instance.ModifyVegetationAtTile(
                    _cursorCellCoord.x,
                    _cursorCellCoord.y,
                    _vegAdjustAmount);
            }


            TileStatsRenderer.Instance.
                ClearBaseTilesAtCoord(_cursorCellCoord);
            TileStatsRenderer.Instance.
                RenderSingleCellByCoord(_cursorCellCoord.x,_cursorCellCoord.y);
        }
    }

    private void UpdateCursorAdjustments_Debug()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _tempAdjustAmount += _parameterAdjustStepAmount;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            _tempAdjustAmount -= _parameterAdjustStepAmount;
        }
        if (Mathf.Abs(_tempAdjustAmount) < 0.1)
        {
            _tempAdjustAmount = 0;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            _moistureAdjustAmount += _parameterAdjustStepAmount;

        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            _moistureAdjustAmount -= _parameterAdjustStepAmount;
        }
        if (Mathf.Abs(_moistureAdjustAmount) < 0.1)
        {
            _moistureAdjustAmount = 0;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            _popAdjustAmount += _parameterAdjustStepAmount;

        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            _popAdjustAmount -= _parameterAdjustStepAmount;
        }
        if (Mathf.Abs(_popAdjustAmount) < 0.1)
        {
            _popAdjustAmount = 0;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            _trafficAdjustAmount += _parameterAdjustStepAmount;

        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            _trafficAdjustAmount -= _parameterAdjustStepAmount;
        }
        if (Mathf.Abs(_trafficAdjustAmount) < 0.1)
        {
            _trafficAdjustAmount = 0;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            _vegAdjustAmount += _parameterAdjustStepAmount;

        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            _vegAdjustAmount -= _parameterAdjustStepAmount;
        }
        if (Mathf.Abs(_vegAdjustAmount) < 0.1)
        {
            _vegAdjustAmount = 0;
        }

        UI_ParameterAdjustments.Instance.SetMoistureAdjustment(_moistureAdjustAmount);
        UI_ParameterAdjustments.Instance.SetTemperatureAdjustment(_tempAdjustAmount);
        UI_ParameterAdjustments.Instance.SetPopulationAdjustment(_popAdjustAmount);
        UI_ParameterAdjustments.Instance.SetTrafficAdjustment(_trafficAdjustAmount);
        UI_ParameterAdjustments.Instance.SetVegetationAdjustment(_vegAdjustAmount);
    }

    private void UpdateCursorInspection()
    {
        _cursorWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + _offset;
        _cursorCellCoord = TileStatsHolder.Instance.GetTileCoord(_cursorWorldPos);

        UI_TileInspector.Instance.SetTileCoords(_cursorCellCoord);
        TileStats td = TileStatsHolder.Instance.
            GetTileDataAtTileCoord(_cursorCellCoord.x,_cursorCellCoord.y);

        UI_TileInspector.Instance.SetTemperature(td.Temperature);
        UI_TileInspector.Instance.SetMoisture(td.Moisture);
        UI_TileInspector.Instance.SetPopulation(td.Population);
        UI_TileInspector.Instance.SetTraffic(td.Traffic);
        UI_TileInspector.Instance.SetVegetation(td.Vegetation);
    }

}
