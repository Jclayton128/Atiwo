using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridSearch
{
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

}
