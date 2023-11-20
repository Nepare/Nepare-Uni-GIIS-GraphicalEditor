using UnityEngine;

public class StraightLine
{
    public static void DrawLine(int x1, int y1, int x2, int y2, GameController.Mode mode)
    {
        if (mode == GameController.Mode.LineCDA)
        {
            DrawCDA(x1, y1, x2, y2);
        }

        if (mode == GameController.Mode.LineBresenham)
        {
            DrawBresenham(x1, y1, x2, y2);
        }
        if (mode == GameController.Mode.LineWu)
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
        GameController.Plot(x, y);

        for (int i = 0; i <= length; i++)
        {
            x += dx; y += dy;
            GameController.Plot(x, y);
        }
    }

    private static void DrawBresenham(int x1, int y1, int x2, int y2)
    {
        float x = x1, y = y1;
        int deltaX = Mathf.Abs(x2 - x1), deltaY = Mathf.Abs(y2 - y1);
        float e = 0f;
        try { e = deltaY / deltaX - 0.5f; } catch {}
        float changeX = (x1 < x2) ? 1f : -1f;
        float changeY = (y1 < y2) ? 1f : -1f;

        GameController.Plot(x, y);

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

                GameController.Plot(x, y);
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
                GameController.Plot(x, y);
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

            GameController.Plot(x, y);
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
        GameController.Plot(x, y);
        float a = Mathf.Abs(e);
        if (e < 0)
            GameController.PlotAlpha(a, x - changeX, y - changeY);
        else
            GameController.PlotAlpha(a, x + changeX, y + changeY);
    }
}
