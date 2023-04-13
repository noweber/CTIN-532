using UnityEngine;

public static class DebugUtils
{
    public static void PrintBoolArray(bool[,] array)
    {
        int width = array.GetLength(0);
        int height = array.GetLength(1);

        for (int y = 0; y < height; y++)
        {
            string output = "";
            for (int x = 0; x < width; x++)
            {
                output += array[x, y] ? "1 " : "0 ";
            }
            Debug.Log(output);
        }
    }
}
