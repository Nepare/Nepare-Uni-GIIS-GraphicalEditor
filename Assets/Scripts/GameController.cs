using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public enum Mode
    {
        None,
        LineCDA,
        LineBresenham,
        LineWu,
        Circle,
        Parabola,
        Ellipse,
        Hyperbola,
        Hermite,
        Bezier,
        Bspline
    }
    public static Mode mode = Mode.None;

    public GameObject pixel, matrixObject; 
    private static PixelController[,] matrix;
    private static List<GameObject> selectedPixels = new List<GameObject>();

    private const int WIDTH = 192, HEIGHT = 144;

    private void Awake()
    {
        FillMatrix(WIDTH, HEIGHT);
    }

    private void FillMatrix(int width, int height)
    {   
        matrix = new PixelController[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject newPixel = Instantiate(pixel, matrixObject.transform);
                newPixel.GetComponent<PixelController>().SetCoords(i, j);
                matrix[i, j] = newPixel.GetComponent<PixelController>();
                newPixel.transform.position = new Vector3(i, j, -5);
            }
        }
    }

    public static void HandleSelectedPixel(GameObject selected_pixel)
    {
        if (mode >= Mode.LineCDA && mode <= Mode.Parabola)
        {
            if (selectedPixels.Count < 2)
            {
                selectedPixels.Add(selected_pixel);
                selected_pixel.GetComponent<PixelController>().ChangeColor(255, true);
            }
            if (selectedPixels.Count == 2)
            {
                int x1, x2, y1, y2;
                x1 = selectedPixels[0].GetComponent<PixelController>().x;
                x2 = selectedPixels[1].GetComponent<PixelController>().x;
                y1 = selectedPixels[0].GetComponent<PixelController>().y;
                y2 = selectedPixels[1].GetComponent<PixelController>().y;
                ClearSelectedPixels();
                if (mode >= Mode.LineCDA && mode <= Mode.LineWu)
                {
                    DrawLine(x1, y1, x2, y2);
                    Debug.Log("Line from " + x1.ToString() + ";" + y1.ToString() + " to " + x2.ToString() + ";" + y2.ToString());
                }
                if (mode == Mode.Circle)
                {
                    DrawCircle(x1, y1, x2, y2);
                    Debug.Log("Circle with center in " + x1.ToString() + ";" + y1.ToString() + " and radius " + FindRadius(x1, y1, x2, y2).ToString());
                }
                if (mode == Mode.Parabola)
                {
                    int a = Convert.ToInt32(y1 - y2);
                    DrawParabola(x1, y1, a);
                    Debug.Log("Parabola with center in " + x1.ToString() + ";" + y1.ToString() + " and a = " + a.ToString());
                }
                selectedPixels.Clear();
            }
        }
        if (mode >= Mode.Ellipse && mode <= Mode.Hyperbola)
        {
            if (selectedPixels.Count < 3)
            {
                selectedPixels.Add(selected_pixel);
                selected_pixel.GetComponent<PixelController>().ChangeColor(255, true);
            }
            if (selectedPixels.Count == 3)
            {
                int x1, x2, y1, y3;
                x1 = selectedPixels[0].GetComponent<PixelController>().x;
                x2 = selectedPixels[1].GetComponent<PixelController>().x;
                y1 = selectedPixels[0].GetComponent<PixelController>().y;
                y3 = selectedPixels[2].GetComponent<PixelController>().y;
                ClearSelectedPixels();
                int a = Convert.ToInt32(Mathf.Abs(x1 - x2));
                int b = Convert.ToInt32(Mathf.Abs(y1 - y3));
                if (mode == Mode.Ellipse)
                {
                    DrawEllipse(x1, y1, a, b);
                    Debug.Log("Ellipse with center in " + x1.ToString() + ";" + y1.ToString() + ", a = " + a.ToString() + ", b =" + b.ToString());
                }
                if (mode == Mode.Hyperbola)
                {
                    DrawHyperbola(x1, y1, a, b);
                    Debug.Log("Hyperbola with center in " + x1.ToString() + ";" + y1.ToString() + ", a = " + a.ToString() + ", b =" + b.ToString());
                }
                selectedPixels.Clear();
            }
        }
    }

// MAIN FUNCTIONS

    private static void DrawLine(int x1, int y1, int x2, int y2)
    {
        if (mode == Mode.LineCDA)
        {
            DrawCDA(x1, y1, x2, y2);
        }

        if (mode == Mode.LineBresenham)
        {
            DrawBresenham(x1, y1, x2, y2);
        }
        if (mode == Mode.LineWu)
        {
            DrawWu(x1, y1, x2, y2);
        }
    }

    private static void DrawCDA(int x1, int y1, int x2, int y2)
    {
        int length = Mathf.Max(Mathf.Abs(x2 - x1), Mathf.Abs(y2 - y1));
        float dx = (x2 - x1) / (float)length, dy = (y2 - y1) / (float)length;
        float x = x1 + 0.5f * Mathf.Sign(dx);
        float y = y1 + 0.5f * Mathf.Sign(dy);
        Plot(x, y);

        for (int i = 0; i <= length; i++)
        {
            x += dx; y += dy;
            Plot(x, y);
        }
    }

    private static void DrawBresenham(int x1, int y1, int x2, int y2)
    {
        float x = x1, y = y1;
        int deltaX = Mathf.Abs(x2 - x1), deltaY = Mathf.Abs(y2 - y1);
        float e = deltaY / deltaX - 0.5f;
        float changeX = (x1 < x2) ? 1f : -1f;
        float changeY = (y1 < y2) ? 1f : -1f;

        Plot(x, y);

        int i = 1;
        if (deltaX >= deltaY) 
        {
            e = 2 * deltaY - deltaX;
            while (i <= deltaX) {
                if (e >= 0) 
                {
                    y += changeY;
                    e -= 2 * deltaX;
                }
                x += changeX;
                e += 2 * deltaY;

                Plot(x, y);
                i++;
            }
        } 
        else 
        {
            e = 2 * deltaX - deltaY;
            while (i <= deltaY) 
            {
                if (e >= 0) 
                {
                    x += changeX;
                    e -= 2 * deltaY;
                }
                y += changeY;
                e += 2 * deltaX;
                Plot(x, y);
                i++;
            }
        }
    }

    private static void DrawWu(int x1, int y1, int x2, int y2)
    {
        if (x1 == x2 || y1 == y2)
            DrawBresenham(x1, y1, x2, y2);
        else
        {
            int x = x1, y = y1;
            int dx = Mathf.Abs(x2 - x1), dy = Mathf.Abs(y2 - y1);
            float e;

            int changeX = (x1 < x2) ? 1 : -1;
            int changeY = (y1 < y2) ? 1 : -1;

            Plot(x, y);
            int i = 1;
            if (dx >= dy)
            {
                e = (dy / (float)dx) - 0.5f;
                while (i <= dx) 
                {
                    if (e >= 0) 
                    {
                        y += changeY;
                        e -= 1;
                    }
                    x += changeX;
                    e += (dy / (float)dx);
                    DrawWuInternal(x, y, e, 0, changeY);
                    i++;
                }
            } 
            else 
            {
                e = (dx / (float)dy) - 0.5f;
                while (i <= dy) 
                {
                    if (e >= 0) 
                    {
                        x += changeX;
                        e -= 1;
                    }
                    y += changeY;
                    e += (dx / (float)dy);
                    DrawWuInternal(x, y, e, 0, changeY);
                    i++;
                }
            }
        }
    }

    private static void DrawWuInternal(int x, int y, float e, int changeX, int changeY)
    {
        Plot(x, y);
        float a = Mathf.Abs(e);
        if (e < 0)
            PlotAlpha(a, x - changeX, y - changeY);
        else
            PlotAlpha(a, x + changeX, y + changeY);
    }

    private static void DrawCircle(int x1, int y1, int x2, int y2)
    {
        int x = 0;
        int radius = FindRadius(x1, y1, x2, y2);
        int y = radius;
        int limit = y - radius;
        int delta = 2 - 2 * radius;
        DrawCirclePixels(x1, y1, x, y);
        int i = 0;
        while (y > limit) {
            i++;
            int dz = 2 * delta - 2 * x - 1;
            if (delta > 0 && dz > 0) {
                y--;
                delta += 1 - 2 * y;
                DrawCirclePixels(x1, y1, x, y);
                continue;
            }
            int d = 2 * delta + 2 * y - 1;
            if (delta < 0 && d <= 0) {
                x ++;
                delta += 1 + 2 * x;
                DrawCirclePixels(x1, y1, x, y);
                continue;
            }
            x++;
            y--;
            delta += 2 * x - 2 * y + 2;
            DrawCirclePixels(x1, y1, x, y);
        }
    }

    private static void DrawParabola(int x1, int y1, int a)
    {
        int x = 0;
        int y = 0;
        int sign_a = Convert.ToInt32(Mathf.Sign(a));
        a = Convert.ToInt32(Mathf.Abs(a));
        int delta = 1 - 2 * a;
        DrawParabolaPixels(x1, y1, x, sign_a * y);
        int i = 0;
        while (i < 50) {
            i++;
            int dz = 2 * delta - 2 * x - 1;
            if (delta > 0 && dz > 0) {
                y--;
                delta -= 2 * a;
                DrawParabolaPixels(x1, y1, x, sign_a * y);
                continue;
            }
            int d = 2 * delta + 2 * a;
            if (delta < 0 && d <= 0) {
                x++;
                delta += 2 * x + 1;
                DrawParabolaPixels(x1, y1, x, sign_a * y);
                continue;
            }
            x++;
            y--;
            delta += 2 * x + 1 - 2 * a;
            DrawParabolaPixels(x1, y1, x, sign_a * y);
        }
    }

    private static void DrawEllipse(int x0, int y0, int a, int b)
    {
        int aPow2 = Convert.ToInt32(Mathf.Pow(a, 2)); 
        int bPow2 = Convert.ToInt32(Mathf.Pow(b, 2)); 
        int x = 0;
        int y = b;
        int limit = y - b;
        int delta = aPow2 + bPow2 - 2 * aPow2 * b;
        DrawCirclePixels(x0, y0, x, y);
        int i = 0;
        while (y > limit) {
            i++;
            int dz = 2 * delta - 2 * x * bPow2 - 1;
            if (delta > 0 && dz > 0) {
                y--;
                delta += aPow2 - 2 * y * aPow2;
                DrawCirclePixels(x0, y0, x, y);
                continue;
            }
            int d = 2 * delta + 2 * y * aPow2 - 1;
            if (delta < 0 && d <= 0) {
                x++;
                delta += bPow2 + 2 * x * bPow2;
                DrawCirclePixels(x0, y0, x, y);
                continue;
            }
            x++;
            y--;
            delta += bPow2 * (2 * x + 1) + aPow2 * (1 - 2 * y);
            DrawCirclePixels(x0, y0, x, y);
        }
    }

    private static void DrawHyperbola(int x0, int y0, int a, int b)
    {
        int aPow2 = Convert.ToInt32(Mathf.Pow(a, 2)); 
        int bPow2 = Convert.ToInt32(Mathf.Pow(b, 2)); 
        int x = 0;
        int y = b;
        int delta = aPow2 + 2 * aPow2 * b - bPow2;
        DrawCirclePixels(x0, y0, x, y);
        int i = 0;
        while (i < 50) {
            i++;
            int dz = 2 * delta - aPow2 * (2 * y + 1);
            if (delta > 0 && dz > 0) {
                x++;
                delta -=  bPow2 * 2 * x + bPow2;
                DrawCirclePixels(x0, y0, x, y);
                continue;
            }
            int d = 2 * delta + bPow2 * (2 * x + 1);
            if (delta < 0 && d <= 0) {
                y++;
                delta += aPow2 * 2 * y + aPow2;
                DrawCirclePixels(x0, y0, x, y);
                continue;
            }
            x++;
            y++;
            delta += aPow2 * (2 * y + 1) - bPow2 * (2 * x + 1);
            DrawCirclePixels(x0, y0, x, y);
        }
    }

    private static int FindRadius(int x1, int y1, int x2, int y2)
    {
        float max = Mathf.Max(Mathf.Abs(x2 - x1), Mathf.Abs(y2 - y1));
        float min = Mathf.Min(Mathf.Abs(x2 - x1), Mathf.Abs(y2 - y1));
        return System.Convert.ToInt32(Mathf.Sqrt(Mathf.Pow(max, 2) + Mathf.Pow(min, 2)));
    }

    private static void DrawCirclePixels(int x0, int y0, int dx, int dy)
    {
        Plot(dx  + x0, dy  + y0);
        Plot(-dx + x0, -dy + y0);
        Plot(dx  + x0, -dy + y0);
        Plot(-dx + x0, dy  + y0);
    }

    private static void DrawParabolaPixels(int x0, int y0, int dx, int dy)
    {
        Plot(dx  + x0, dy  + y0);
        Plot(-dx + x0, dy + y0);
    }

    private static void DrawHermite(int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4)
    {

    }

// UTILITY FUNCTIONS

    public static void ClearSelectedPixels()
    {
        for (int i = 0; i < selectedPixels.Count; i++)
        {
            selectedPixels[i].GetComponent<PixelController>().ChangeColor(0, false);
        }
        selectedPixels.Clear();
    }

    public static void ClearScreen()
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                matrix[i, j].ChangeToWhite();
            }
        }
    }

    private static void Plot(float x, float y)
    {
        if (x >= 0 && y >= 0 && x < WIDTH && y < HEIGHT)
            matrix[System.Convert.ToInt32(Mathf.Floor(x)), System.Convert.ToInt32(Mathf.Floor(y))].ChangeColor(255, false);
    }

    private static void PlotAlpha(float alpha, float x, float y)
    {
        matrix[System.Convert.ToInt32(Mathf.Floor(x)), System.Convert.ToInt32(Mathf.Floor(y))].ChangeColor(alpha, false);
    }
}
