using System;
using UnityEngine;

public class SecondOrderLine
{
    public static void DrawCircle(int x1, int y1, int x2, int y2)
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

    public static void DrawParabola(int x1, int y1, int a)
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

    public static void DrawEllipse(int x0, int y0, int a, int b)
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

    public static void DrawHyperbola(int x0, int y0, int a, int b)
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

    private static void DrawCirclePixels(int x0, int y0, int dx, int dy)
    {
        GameController.Plot(dx  + x0, dy  + y0);
        GameController.Plot(-dx + x0, -dy + y0);
        GameController.Plot(dx  + x0, -dy + y0);
        GameController.Plot(-dx + x0, dy  + y0);
    }

    private static void DrawParabolaPixels(int x0, int y0, int dx, int dy)
    {
        GameController.Plot(dx  + x0, dy  + y0);
        GameController.Plot(-dx + x0, dy + y0);
    }

    public static int FindRadius(int x1, int y1, int x2, int y2)
    {
        float max = Mathf.Max(Mathf.Abs(x2 - x1), Mathf.Abs(y2 - y1));
        float min = Mathf.Min(Mathf.Abs(x2 - x1), Mathf.Abs(y2 - y1));
        return Convert.ToInt32(Mathf.Sqrt(Mathf.Pow(max, 2) + Mathf.Pow(min, 2)));
    }
}
