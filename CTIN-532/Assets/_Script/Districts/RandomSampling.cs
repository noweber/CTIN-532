using System.Collections.Generic;
using UnityEngine;

public class RandomSampling
{
    public static List<Vector2Int> GetRandomPointsFromGrid(bool[,] grid, int minPointsToReturn, int maxPointsToReturn, int maxRetries = 100)
    {
        List<Vector2Int> points = new List<Vector2Int>();
        int retries = 0;
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        while (points.Count < maxPointsToReturn && retries < maxRetries)
        {
            Vector2Int point = new Vector2Int(Random.Range(0, width), Random.Range(0, height));

            if (grid[point.x, point.y])
            {
                points.Add(point);
            }

            retries++;
        }

        if (points.Count < minPointsToReturn)
        {
            return null;
        }

        return points;
    }

}
