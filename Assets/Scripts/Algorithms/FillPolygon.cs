using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Unity.Mathematics;
using UnityEngine;

public class FillPolygon
{
    public static void DrawPolygonScanline(int[] x_arr, int[] y_arr)
    {
        int cornerCount = x_arr.Length;
        List<(int x, int y)> intersections = new();
        int x_mean = x_arr.Sum() / x_arr.Length, y_mean = y_arr.Sum() / y_arr.Length;

        for(int side = 0; side < cornerCount; side++)
        {
            int x1, x2, y1, y2;
            x1 = x_arr[side]; y1 = y_arr[side];
            x2 = (side == cornerCount - 1) ? x_arr[0] : x_arr[side + 1];
            y2 = (side == cornerCount - 1) ? y_arr[0] : y_arr[side + 1];

            StraightLine.DrawLine(x1, y1, x2, y2, GameController.Mode.LineBresenham);
            if (y1 == y2) continue;

            int y_base = Math.Min(y1, y2) + 1, x_base = Math.Min(x1, x2) + 1;
            int scanlinesCount = Convert.ToInt32(Math.Abs(y2 - y1));
            for (int i = 0; i < scanlinesCount; i++)
            {
                if (i == 0)
                {
                    int y_prev, y_next;
                    y_prev = (side == 0) ? y_arr[cornerCount - 1] : y_arr[side - 1];
                    y_next = (side == cornerCount - 1) ? y_arr[0] : y_arr[side + 1];

                    if (!IsLocalExtremum(y1, y_prev, y_next)) continue;
                }

                int ys = i + y_base;
                double dX = x2 - x1, dY = y2 - y1;
                if ((y1 <= ys && y2 >= ys) || (y2 <= ys && y1 >= ys))
                    intersections.Add((Convert.ToInt32(Math.Round(dX * (ys - y1) / dY)) + x_base, i + y_base));
            }
        }    

        List<(int x, int y)> resultsSorted = new();
        resultsSorted = intersections.OrderBy(i => i.x).ToList();
        resultsSorted = resultsSorted.OrderBy(i => i.y).ToList();

        for (int i = 0; i < resultsSorted.Count - 1; i++)
        {
            if (resultsSorted[i + 1].y == resultsSorted[i].y)
            {
                int x_min = Math.Min(resultsSorted[i].x, resultsSorted[i + 1].x);
                int x_max = Math.Max(resultsSorted[i].x, resultsSorted[i + 1].x);
                for (int j = x_min; j < x_max; j++)
                {
                    Floodfill(x_mean, y_mean);
                }
            }
        }
    }

    public static void DrawPolygonScanlineActive(int[] x_arr, int[] y_arr)
    {
        int cornerCount = x_arr.Length;
        List<(int x, int y)> intersections = new();
        int x_mean = x_arr.Sum() / x_arr.Length, y_mean = y_arr.Sum() / y_arr.Length;

        for(int side = 0; side < cornerCount; side++)
        {
            int x1, x2, y1, y2;
            x1 = x_arr[side]; y1 = y_arr[side];
            x2 = (side == cornerCount - 1) ? x_arr[0] : x_arr[side + 1];
            y2 = (side == cornerCount - 1) ? y_arr[0] : y_arr[side + 1];

            StraightLine.DrawLine(x1, y1, x2, y2, GameController.Mode.LineBresenham);
            if (y1 == y2) continue;

            int y_base = Math.Min(y1, y2) + 1, x_base = Math.Min(x1, x2) + 1;
            int scanlinesCount = Convert.ToInt32(Math.Abs(y2 - y1));
            for (int i = 0; i < scanlinesCount; i++)
            {
                if (i == 0)
                {
                    int y_prev, y_next;
                    y_prev = (side == 0) ? y_arr[cornerCount - 1] : y_arr[side - 1];
                    y_next = (side == cornerCount - 1) ? y_arr[0] : y_arr[side + 1];

                    if (!IsLocalExtremum(y1, y_prev, y_next)) continue;
                }

                int ys = i + y_base;
                double dX = x2 - x1, dY = y2 - y1;
                if ((y1 <= ys && y2 >= ys) || (y2 <= ys && y1 >= ys))
                    intersections.Add((Convert.ToInt32(Math.Round(dX * (ys - y1) / dY)) + x_base, i + y_base));
            }
        }    

        List<(int x, int y)> resultsSorted = new();
        resultsSorted = intersections.OrderBy(i => i.x).ToList();
        resultsSorted = resultsSorted.OrderBy(i => i.y).ToList();

        for (int i = 0; i < resultsSorted.Count - 1; i++)
        {
            if (resultsSorted[i + 1].y == resultsSorted[i].y)
            {
                int x_min = Math.Min(resultsSorted[i].x, resultsSorted[i + 1].x);
                int x_max = Math.Max(resultsSorted[i].x, resultsSorted[i + 1].x);
                for (int j = x_min; j < x_max; j++)
                {
                    Floodfill(x_mean, y_mean);
                }
            }
        }
    }

    private static bool IsLocalExtremum(int yp, int y1, int y2)
    {
        if (y1 > yp && y2 > yp) return true;
        else return false;
    }

    public static void Floodfill(int x, int y)
    {
        Stack<(int x, int y)> pixels = new();
        pixels.Push((x, y));
    
        while (pixels.Count > 0)
        {
            (int x, int y) a = pixels.Pop();
            if (a.x < GameController.WIDTH && a.x >= 0 && a.y < GameController.HEIGHT && a.y >= 0)
            {
                if (!GameController.CheckPixelFill(a.x, a.y))
                {
                    GameController.PlotAlpha(0.3f, a.x, a.y);
                    pixels.Push((a.x - 1, a.y));
                    pixels.Push((a.x + 1, a.y));
                    pixels.Push((a.x, a.y - 1));
                    pixels.Push((a.x, a.y + 1));
                }
            }
        }
        return;    
    }

    public static void FloodfillString(int x, int y)
    {
        Stack<(int x, int y)> pixels = new();
        pixels.Push((x, y));
    
        while (pixels.Count > 0)
        {
            (int x, int y) a = pixels.Pop();
            if (a.x < GameController.WIDTH && a.x >= 0 && a.y < GameController.HEIGHT && a.y >= 0)
            {
                int i = 0, j = 0;
                while (!GameController.CheckPixelFill(a.x - i, a.y))
                {
                    GameController.PlotAlpha(0.3f, a.x - i, a.y);
                    i++;
                }
                while (!GameController.CheckPixelFill(a.x + j, a.y))
                {
                    GameController.PlotAlpha(0.3f, a.x + j, a.y);
                    j++;
                }
                for (int k = a.x - i; k <= a.x + j; k++)
                {
                    if (!GameController.CheckPixelFill(k, a.y - 1)) pixels.Push((k, a.y - 1));
                    if (!GameController.CheckPixelFill(k, a.y + 1)) pixels.Push((k, a.y + 1));
                }
            }
        }
        return;    
    }
}
