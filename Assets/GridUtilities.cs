using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridUtilities
{
    static Vector2Int _north = new Vector2Int(0, 1);
    static Vector2Int _south = new Vector2Int(0, -1);
    static Vector2Int _east = new Vector2Int(1, 0);
    static Vector2Int _west = new Vector2Int(-1, 0);


    /// <summary>
    /// Attempts to provide a coordinate for a cell that is under the target
    /// value, with a low-side tolerance. 
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="targetValue"></param>
    /// <param name="tolerance"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    public static bool SpiralSearch_ClosestToMinValue(float[,] grid, float targetValue, float tolerance, out int row, out int col)
    {
        row = col = (grid.GetLength(0) - 1) / 2; // Start at the center of the grid

        int dirRow = 0, dirCol = 1; // Start moving right
        int stepsInDir = 1, stepsTaken = 0; // Keep track of steps taken in current direction and total steps taken

        while (row >= 0 && row < grid.GetLength(0) && col >= 0 && col < grid.GetLength(1))
        {
            if (grid[row, col] < targetValue &&
                grid[row, col] > targetValue - tolerance)
            {
                return true; // Found the target value
            }

            row += dirRow;
            col += dirCol;
            stepsTaken++;

            if (stepsTaken == stepsInDir)
            {
                stepsTaken = 0;

                // Change direction (rotate counterclockwise)
                int temp = dirRow;
                dirRow = -dirCol;
                dirCol = temp;

                if (dirCol == 0) // Need to increase steps in the new direction
                {
                    stepsInDir++;
                }
            }
        }

        Debug.Log("Couldn't find within search parameters");
        return false; // Target value not found
    }


    public static Vector2Int GetSpiralCoordinateAtIndex(float[,] arr, Vector2Int startCoord, int index)
    {
        int m = arr.GetLength(0); // number of rows
        int n = arr.GetLength(1); // number of columns

        int row = startCoord.y;
        int col = startCoord.x;
        int direction = 0; // 0 = right, 1 = down, 2 = left, 3 = up
        int steps = 1;
        int stepCount = 0;

        for (int i = 0; i < index; i++)
        {
            if (direction == 0)
            { // right
                if (stepCount < steps)
                {
                    col++;
                    stepCount++;
                }
                else
                {
                    direction = 1; // go down
                    stepCount = 0;
                    //steps++;
                    //row--;
                }
            }
            else if (direction == 1)
            { // down
                if (stepCount < steps)
                {
                    row--;
                    stepCount++;
                }
                else
                {
                    direction = 2; // go left
                    stepCount = 0;
                    steps++;
                    //col--;
                }
            }
            else if (direction == 2)
            { // left
                if (stepCount < steps)
                {
                    col--;
                    stepCount++;
                }
                else
                {
                    direction = 3; // go up
                    stepCount = 0;
                    //steps++;
                    //row++;
                }
            }
            else if (direction == 3)
            { // up
                if (stepCount < steps)
                {
                    row++;
                    stepCount++;
                }
                else
                {
                    direction = 0; // go right
                    stepCount = 0;
                    steps++;
                    //col++;
                }
            }

            // check if index is out of bounds
            if (row < 0 || row >= m || col < 0 || col >= n)
            {
                Debug.Log("Invalid parameters");
                return new Vector2Int(-1,-1);
            }
        }

        return new Vector2Int(col, row);  
    }

    public static Vector2Int FindCellWithHighestValue (float[,] arr)
    {
        // get the dimensions of the array
        int maxY = arr.GetLength(0);
        int maxX = arr.GetLength(1);

        // initialize variables to store the maximum value and its coordinates
        float maxValue = float.MinValue;
        Vector2Int maxCoord = Vector2Int.zero;

        // loop through each cell in the array
        for (int y = 0; y < maxY; y++)
        {
            for (int x = 0; x < maxX; x++)
            {
                // if the current cell's value is greater than the current maximum value,
                // update the maximum value and its coordinates
                //Debug.Log($"X: {x}/{maxX}, Y: {y}/{maxY}");
                if (arr[y, x] > maxValue)
                {
                    maxValue = arr[y, x];
                    maxCoord.x = x;
                    maxCoord.y = y;
                }
            }
        }
        return maxCoord;
    }

    public static Vector2Int FindCellWithLowestValue(float[,] arr)
    {
        // get the dimensions of the array
        int maxY = arr.GetLength(0);
        int maxX = arr.GetLength(1);

        // initialize variables to store the maximum value and its coordinates
        float minValue = float.MaxValue;
        Vector2Int minCoord = Vector2Int.zero;

        // loop through each cell in the array
        for (int y = 0; y < maxY; y++)
        {
            for (int x = 0; x < maxX; x++)
            {
                // if the current cell's value is greater than the current maximum value,
                // update the maximum value and its coordinates
                //Debug.Log($"X: {x}/{maxX}, Y: {y}/{maxY}");
                if (arr[y, x] < minValue)
                {
                    minValue = arr[y, x];
                    minCoord.x = x;
                    minCoord.y = y;
                }
            }
        }
        return minCoord;
    }

    public static float FindSumValueWithinGrid(float[,] arr)
    {
        // get the dimensions of the array
        int maxY = arr.GetLength(0);
        int maxX = arr.GetLength(1);

        // initialize variables to store the maximum value and its coordinates
        float sum = 0;
        Vector2Int maxCoord = Vector2Int.zero;

        // loop through each cell in the array
        for (int y = 0; y < maxY; y++)
        {
            for (int x = 0; x < maxX; x++)
            {
                sum += arr[y, x];
            }
        }
        return sum;
    }

    public static float[,] ExtractSubArray(float[,] sourceArray, int startX, int startY, int subArrayWidth, int subArrayHeight)
    {
        float[,] subArray = new float[subArrayHeight, subArrayWidth];

        for (int y = 0; y < subArrayHeight; y++)
        {
            for (int x = 0; x < subArrayWidth; x++)
            {
                subArray[y, x] = sourceArray[startY + y, startX + x];
            }
        }

        return subArray;
    }


    /// <summary>
    /// Takes in an array and returns the coordinates for the source coord's
    /// 4 orthogonal neighbors, arranged North, East, South, West.
    /// Neighbor coordinates that are outside of the bounds of the array
    /// will return as -1,-1.
    /// </summary>

    public static Vector2Int[] GetNeighboringCellCoordinates(float[,] array, Vector2Int startCoord)
    {
        if (array.GetLength(0) != array.GetLength(1))
        {
            Debug.LogWarning("Array must be square.");
            return null;
        }

        //N,E,S,W
        Vector2Int[] neighborCoords = new Vector2Int[4]
        {
            new Vector2Int(-1,-1),
            new Vector2Int(-1,-1),
            new Vector2Int(-1,-1),
            new Vector2Int(-1,-1),
        };

        if (startCoord.y +1 < array.GetLength(0))
        {
            neighborCoords[0] = startCoord + _north;
        }

        if (startCoord.x + 1 < array.GetLength(0))
        {
            neighborCoords[1] = startCoord + _east;
        }
        if (startCoord.y - 1 < array.GetLength(0))
        {
            neighborCoords[2] = startCoord + _south;
        }
        if (startCoord.x - 1 < array.GetLength(0))
        {
            neighborCoords[3] = startCoord + _west;
        }

        return neighborCoords;
    }
}

public class ParameterGrid
{
    public Vector2Int Origin;
    public Vector2Int FarCorner;
    public Vector2Int SignificantCoord;
    public float TotalValue;

    public ParameterGrid(Vector2Int origin, Vector2Int farCorner, Vector2Int highPoint, float volume)
    {
        //Debug.Log($"new WaterGrid start: {origin.x},{origin.y}. Extends: {farCorner.x}, {farCorner.y}. Volume: {volume} " +
        //    $"High: {highPoint.x},{highPoint.y} ");
        this.Origin = origin;
        this.FarCorner = farCorner;
        this.SignificantCoord = highPoint;
        this.TotalValue = volume;
    }
}


