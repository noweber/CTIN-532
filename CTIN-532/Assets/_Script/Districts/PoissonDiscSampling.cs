using System.Collections.Generic;
using UnityEngine;

public class PoissonDiscSampling
{
    public static List<Vector2> GeneratePoints(bool[,] grid, int radius, int k = 30, int maxTries = 10)
    {
        // Get the dimensions of the grid
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        // Calculate the cell size based on the radius
        float cellSize = radius / Mathf.Sqrt(2);

        // Create a grid of cells
        int[,] cells = new int[Mathf.CeilToInt(width / cellSize), Mathf.CeilToInt(height / cellSize)];

        // Create a list of active points and a list of points to return
        List<Vector2> activePoints = new List<Vector2>();
        List<Vector2> points = new List<Vector2>();

        // Choose a random point to start with
        Vector2 startPoint = new Vector2(Random.Range(0, width), Random.Range(0, height));
        activePoints.Add(startPoint);

        // Mark the corresponding cell as occupied
        int cellX = Mathf.FloorToInt(startPoint.x / cellSize);
        int cellY = Mathf.FloorToInt(startPoint.y / cellSize);
        cells[cellX, cellY] = activePoints.Count;

        // While there are active points
        while (activePoints.Count > 0)
        {
            // Choose a random active point
            int index = Random.Range(0, activePoints.Count);
            Vector2 point = activePoints[index];

            // Generate k random points within the radius of the active point
            bool found = false;
            for (int i = 0; i < k; i++)
            {
                Vector2 newPoint = GenerateRandomPointAround(point, radius);

                // Check if the new point is within the bounds of the grid and in an open position
                if (newPoint.x >= 0 && newPoint.x < width && newPoint.y >= 0 && newPoint.y < height && grid[(int)newPoint.x, (int)newPoint.y])
                {
                    // Check if the new point is too close to any existing point
                    int newCellX = Mathf.FloorToInt(newPoint.x / cellSize);
                    int newCellY = Mathf.FloorToInt(newPoint.y / cellSize);
                    bool isSafe = true;

                    for (int x = Mathf.Max(0, newCellX - 2); x <= Mathf.Min(cells.GetLength(0) - 1, newCellX + 2); x++)
                    {
                        for (int y = Mathf.Max(0, newCellY - 2); y <= Mathf.Min(cells.GetLength(1) - 1, newCellY + 2); y++)
                        {
                            int otherIndex = cells[x, y] - 1;
                            if (otherIndex != -1)
                            {
                                Vector2 otherPoint = activePoints[otherIndex];
                                if (Vector2.Distance(newPoint, otherPoint) < radius)
                                {
                                    isSafe = false;
                                    break;
                                }
                            }
                        }
                        if (!isSafe) break;
                    }

                    // If the new point is safe, add it to the active points and points to return
                    if (isSafe)
                    {
                        activePoints.Add(newPoint);
                        points.Add(newPoint);
                        cells[newCellX, newCellY] = activePoints.Count;
                        found = true;
                        break;
                    }
                }
            }

            // If no new point was found, remove the active point
            if (!found)
            {
                activePoints.RemoveAt(index);
            }
            else if (activePoints.Count >= maxTries)
            {
                // If we've tried too many times, remove the active point to avoid infinite loops
                activePoints.RemoveAt(index);
            }
        }

        return points;
    }

    private static Vector2 GenerateRandomPointAround(Vector2 point, float radius)
    {
        float r1 = Random.value;
        float r2 = Random.value;
        float radiusSq = (r1 * radius * radius);
        float angle = 2 * Mathf.PI * r2;
        float x = Mathf.Sqrt(radiusSq) * Mathf.Cos(angle);
        float y = Mathf.Sqrt(radiusSq) * Mathf.Sin(angle);
        var result = point + new Vector2(x, y);
        return result;
    }
}
