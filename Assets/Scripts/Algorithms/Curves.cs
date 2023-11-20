using System;
using UnityEngine;

public class Curves
{
    public static void DrawHermite(int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4)
    {
        int i = 0;
        float t = 0.0f;
        float step = 0.01f;

        int[,] a = new int[4, 4] 
        { 
            { 2, -2,  1,  1},
            {-3,  3, -2, -1},
            { 0,  0,  1,  0},
            { 1,  0,  0,  0}
        };
        int[,] b = new int[4, 2]
        {
            {x1, y1},
            {x4, y4},
            {x3 - x1, y3 - y1},
            {x4 - x2, y4 - y2}
        };

        int[,] c = MatrixOps.Multiply(a, b);

        while (t <= 1) {
            float[,] tMatrix = new float[1, 4] { { t * t * t, t * t, t, 1f } };
            float[,] r = MatrixOps.MultiplyDouble(tMatrix, c);
            
            float x = r[0, 0];
            float y = r[0, 1];
            GameController.Plot(Convert.ToInt32(x), Convert.ToInt32(y));
            t += step;
            i++;
        }
    }

    public static void DrawBezier(int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4)
    {
        int i = 0;
        float t = 0.0f;
        float step = 0.005f;

        int[,] a = new int[4, 4] 
        { 
            {-1,  3, -3,  1},
            { 3, -6,  3, -0},
            {-3,  3,  0,  0},
            { 1,  0,  0,  0}
        };
        int[,] b = new int[4, 2]
        {
            {x1, y1},
            {x2, y2},
            {x3, y3},
            {x4, y4}
        };

        int[,] c = MatrixOps.Multiply(a, b);

        while (t <= 1) {
            float[,] tMatrix = new float[1, 4] { { t * t * t, t * t, t, 1f } };
            float[,] r = MatrixOps.MultiplyDouble(tMatrix, c);
            
            float x = r[0, 0];
            float y = r[0, 1];
            GameController.Plot(Convert.ToInt32(x), Convert.ToInt32(y));
            t += step;
            i++;
        }
    }

    public static void DrawBSpline(int[] x_arr, int[] y_arr)
    {
        int n = x_arr.Length;
        Debug.Log(x_arr.Length);

        int k = 0;
        float step = 0.01f;

        int[,] a = new int[4, 4] 
        { 
            {-1,  3, -3,  1},
            { 3, -6,  3, -0},
            {-3,  0,  3,  0},
            { 1,  4,  1,  0}
        };

        int i = 1;
        while (i <= n-3) {
            int[,] b = new int[4, 2] 
            { 
                {x_arr[i-1],  y_arr[i-1]},
                {x_arr[i],    y_arr[i]},
                {x_arr[i+1],  y_arr[i+1]},
                {x_arr[i+2],  y_arr[i+2]}
            };

            int[,] c = MatrixOps.Multiply(a, b);
            float t = 0.0f;
            while (t <= 1) {
                float[,] tMatrix = new float[1, 4] { { t * t * t, t * t, t, 1f } };
                float[,] r = MatrixOps.MultiplyDouble(tMatrix, c);
                
                float x = r[0, 0] / 6;
                float y = r[0, 1] / 6;
                GameController.Plot(Convert.ToInt32(x), Convert.ToInt32(y));
                t += step;
                k++;
            }
            i++;
        }
    }
}
