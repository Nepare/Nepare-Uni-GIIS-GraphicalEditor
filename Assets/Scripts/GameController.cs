using System;
using System.Collections;
using System.Collections.Generic;
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
    private static int BSplineParameter = 4;

    private const int WIDTH = 192, HEIGHT = 144;

    private void Awake()
    {
        FillMatrix(WIDTH, HEIGHT);
        EventManager.OnBSplineParameterChanged += ChangeBSplineParameter;
        EventManager.OnClearScreen += ClearScreen;
        EventManager.OnClearSelectedPixels += ClearSelectedPixels;

        EventManager.SendBSplineParameterChanged(4);
        CubeManipulation.SubscribeToRelevantEvents();
    }

    private void ChangeBSplineParameter(int newParam)
    {
        BSplineParameter = newParam;
        Debug.Log("New BSpline parameter = " + newParam.ToString());
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
                    StraightLine.DrawLine(x1, y1, x2, y2, mode);
                    Debug.Log("Line from " + x1.ToString() + ";" + y1.ToString() + " to " + x2.ToString() + ";" + y2.ToString());
                }
                if (mode == Mode.Circle)
                {
                    SecondOrderLine.DrawCircle(x1, y1, x2, y2);
                    Debug.Log("Circle with center in " + x1.ToString() + ";" + y1.ToString() + " and radius " + SecondOrderLine.FindRadius(x1, y1, x2, y2).ToString());
                }
                if (mode == Mode.Parabola)
                {
                    int a = Convert.ToInt32(y1 - y2);
                    SecondOrderLine.DrawParabola(x1, y1, a);
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
                    SecondOrderLine.DrawEllipse(x1, y1, a, b);
                    Debug.Log("Ellipse with center in " + x1.ToString() + ";" + y1.ToString() + ", a = " + a.ToString() + ", b =" + b.ToString());
                }
                if (mode == Mode.Hyperbola)
                {
                    SecondOrderLine.DrawHyperbola(x1, y1, a, b);
                    Debug.Log("Hyperbola with center in " + x1.ToString() + ";" + y1.ToString() + ", a = " + a.ToString() + ", b =" + b.ToString());
                }
                selectedPixels.Clear();
            }
        }
        if (mode >= Mode.Hermite && mode <= Mode.Bezier)
        {
            if (selectedPixels.Count < 4)
            {
                selectedPixels.Add(selected_pixel);
                selected_pixel.GetComponent<PixelController>().ChangeColor(255, true);
            }
            if (selectedPixels.Count == 4)
            {
                int x1, x2, y1, y2, x3, y3, x4, y4;
                x1 = selectedPixels[0].GetComponent<PixelController>().x;
                x2 = selectedPixels[1].GetComponent<PixelController>().x;
                x3 = selectedPixels[2].GetComponent<PixelController>().x;
                x4 = selectedPixels[3].GetComponent<PixelController>().x;
                y1 = selectedPixels[0].GetComponent<PixelController>().y;
                y2 = selectedPixels[1].GetComponent<PixelController>().y;
                y3 = selectedPixels[2].GetComponent<PixelController>().y;
                y4 = selectedPixels[3].GetComponent<PixelController>().y;

                ClearSelectedPixels();
                if (mode == Mode.Hermite)
                {
                    Curves.DrawHermite(x1, y1, x2, y2, x3, y3, x4, y4);
                    Debug.Log($"Drawing Hermite: p1 = ({x1};{y1}); p2 = ({x2};{y2}); p3 = ({x3};{y3}); p4 = ({x4};{y4});");
                }
                if (mode == Mode.Bezier)
                {
                    Curves.DrawBezier(x1, y1, x2, y2, x3, y3, x4, y4);
                    Debug.Log($"Drawing Bezier: p1 = ({x1};{y1}); p2 = ({x2};{y2}); p3 = ({x3};{y3}); p4 = ({x4};{y4});");
                }
                selectedPixels.Clear();
            }
        }
        if (mode == Mode.Bspline)
        {
            if (selectedPixels.Count < BSplineParameter)
            {
                selectedPixels.Add(selected_pixel);
                selected_pixel.GetComponent<PixelController>().ChangeColor(255, true);
            }
            if (selectedPixels.Count == BSplineParameter)
            {
                int[] x_arr = new int[BSplineParameter];
                int[] y_arr = new int[BSplineParameter];
                for (int i = 0; i < BSplineParameter; i++)
                {
                    x_arr[i] = selectedPixels[i].GetComponent<PixelController>().x;
                    y_arr[i] = selectedPixels[i].GetComponent<PixelController>().y;
                }
                ClearSelectedPixels();
                Curves.DrawBSpline(x_arr, y_arr);
                Debug.Log("Drawing BSpline!");
                selectedPixels.Clear();
            }
        }
    }

// UTILITY FUNCTIONS

    private static void ClearSelectedPixels()
    {
        for (int i = 0; i < selectedPixels.Count; i++)
        {
            selectedPixels[i].GetComponent<PixelController>().ChangeColor(0, false);
        }
        selectedPixels.Clear();
    }

    private static void ClearScreen()
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                matrix[i, j].ChangeToWhite();
            }
        }
    }

    public static void Plot(float x, float y)
    {
        if (x >= 0 && y >= 0 && x < WIDTH && y < HEIGHT)
            matrix[Convert.ToInt32(Mathf.Floor(x)), Convert.ToInt32(Mathf.Floor(y))].ChangeColor(255, false);
    }

    public static void PlotAlpha(float alpha, float x, float y)
    {
        if (x >= 0 && y >= 0 && x < WIDTH && y < HEIGHT)
            matrix[Convert.ToInt32(Mathf.Floor(x)), Convert.ToInt32(Mathf.Floor(y))].ChangeColor(alpha, false);
    }

    public static void PlotColor(float x, float y, Color color)
    {
        if (x >= 0 && y >= 0 && x < WIDTH && y < HEIGHT)
            matrix[Convert.ToInt32(Mathf.Floor(x)), Convert.ToInt32(Mathf.Floor(y))].ChangeColor(color);
    }
}