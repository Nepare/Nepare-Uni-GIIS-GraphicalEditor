using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangulation
{
    public static void TriangulateDelaunay(int[] x_arr, int[] y_arr)
    {
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < x_arr.Length; i++)
        {
            points.Add(new Vector3(x_arr[i], 0, y_arr[i]));
        }
        var triangles = TriangulateByFlippingEdges(points);

        for (int i = 0; i < triangles.Count; i++)
        {
            StraightLine.DrawLine(Convert.ToInt32(triangles[i].v1.GetPos2D_XZ().x), Convert.ToInt32(triangles[i].v1.GetPos2D_XZ().y), Convert.ToInt32(triangles[i].v2.GetPos2D_XZ().x), Convert.ToInt32(triangles[i].v2.GetPos2D_XZ().y), GameController.Mode.LineBresenham);
            StraightLine.DrawLine(Convert.ToInt32(triangles[i].v2.GetPos2D_XZ().x), Convert.ToInt32(triangles[i].v2.GetPos2D_XZ().y), Convert.ToInt32(triangles[i].v3.GetPos2D_XZ().x), Convert.ToInt32(triangles[i].v3.GetPos2D_XZ().y), GameController.Mode.LineBresenham);
            StraightLine.DrawLine(Convert.ToInt32(triangles[i].v3.GetPos2D_XZ().x), Convert.ToInt32(triangles[i].v3.GetPos2D_XZ().y), Convert.ToInt32(triangles[i].v1.GetPos2D_XZ().x), Convert.ToInt32(triangles[i].v1.GetPos2D_XZ().y), GameController.Mode.LineBresenham);
        }
    }

    public static void CreateVoronoiDiagram(int[] x_arr, int[] y_arr)
    {
        List<Vector2> points = new List<Vector2>();
        for (int i = 0; i < x_arr.Length; i++)
        {
            points.Add(new Vector3(x_arr[i], y_arr[i]));
        }
        List<Color> assignedColors = new();
        for (int i = 0; i < points.Count * 10; i++)
        {
            assignedColors.Add(new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1f));
        }

        for (int i = 0; i < GameController.WIDTH; i++)
        {
            for (int j = 0; j < GameController.HEIGHT; j++)
            {
                float minDistance = float.MaxValue;
                int closestPointIndex = -1;
                for (int k = 0; k < points.Count; k++)
                {
                    float currentDistance = Vector2.Distance(new Vector2(i, j), new Vector2(points[k].x, points[k].y));
                    if (minDistance > currentDistance)
                    {
                        minDistance = currentDistance;
                        closestPointIndex = k;
                    }
                }
                
                GameController.PlotColor(i, j, assignedColors[closestPointIndex]);
            }
        }

        for (int i = 0; i < points.Count; i++)
            GameController.PlotColor(points[i].x, points[i].y, Color.white);
    }

    public static List<Triangle> TriangulateConvexPolygon(List<Vertex> convexHullpoints)
    {
        List<Triangle> triangles = new List<Triangle>();

        for (int i = 2; i < convexHullpoints.Count; i++)
        {
            Vertex a = convexHullpoints[0];
            Vertex b = convexHullpoints[i - 1];
            Vertex c = convexHullpoints[i];

            triangles.Add(new Triangle(a, b, c));
        }

        return triangles;
    }

    public static List<Triangle> TriangulateByFlippingEdges(List<Vector3> sites)
    {
        List<Vertex> vertices = new List<Vertex>();

        for (int i = 0; i < sites.Count; i++)
        {
            vertices.Add(new Vertex(sites[i]));
        }

        List<Triangle> triangles = TriangulateConvexPolygon(vertices);
        List<HalfEdge> halfEdges = TransformFromTriangleToHalfEdge(triangles);

        int safety = 0;

        int flippedEdges = 0;

        while (true)
        {
            safety += 1;

            if (safety > 100000)
            {
                Debug.Log("Stuck in an endless loop :(");

                break;
            }

            bool hasFlippedEdge = false;

            for (int i = 0; i < halfEdges.Count; i++)
            {
                HalfEdge thisEdge = halfEdges[i];
                if (thisEdge.oppositeEdge == null)
                {
                    continue;
                }
                Vertex a = thisEdge.v;
                Vertex b = thisEdge.nextEdge.v;
                Vertex c = thisEdge.prevEdge.v;
                Vertex d = thisEdge.oppositeEdge.nextEdge.v;

                Vector2 aPos = a.GetPos2D_XZ();
                Vector2 bPos = b.GetPos2D_XZ();
                Vector2 cPos = c.GetPos2D_XZ();
                Vector2 dPos = d.GetPos2D_XZ();

                if (IsPointInsideOutsideOrOnCircle(aPos, bPos, cPos, dPos) < 0f)
                {
                    if (IsQuadrilateralConvex(aPos, bPos, cPos, dPos))
                    {
                        if (IsPointInsideOutsideOrOnCircle(bPos, cPos, dPos, aPos) < 0f)
                        {
                            continue;
                        }
                        flippedEdges += 1;
                        hasFlippedEdge = true;
                        FlipEdge(thisEdge);
                    }
                }
            }

            if (!hasFlippedEdge) break;
        }

        return triangles;
    }

    public static List<HalfEdge> TransformFromTriangleToHalfEdge(List<Triangle> triangles)
    {
        OrientTrianglesClockwise(triangles);

        List<HalfEdge> halfEdges = new List<HalfEdge>(triangles.Count * 3);

        for (int i = 0; i < triangles.Count; i++)
        {
            Triangle t = triangles[i];
        
            HalfEdge he1 = new HalfEdge(t.v1);
            HalfEdge he2 = new HalfEdge(t.v2);
            HalfEdge he3 = new HalfEdge(t.v3);

            he1.nextEdge = he2;
            he2.nextEdge = he3;
            he3.nextEdge = he1;

            he1.prevEdge = he3;
            he2.prevEdge = he1;
            he3.prevEdge = he2;

            he1.v.halfEdge = he2;
            he2.v.halfEdge = he3;
            he3.v.halfEdge = he1;

            t.halfEdge = he1;

            he1.t = t;
            he2.t = t;
            he3.t = t;

            halfEdges.Add(he1);
            halfEdges.Add(he2);
            halfEdges.Add(he3);
        }

        for (int i = 0; i < halfEdges.Count; i++)
        {
            HalfEdge he = halfEdges[i];

            Vertex goingToVertex = he.v;
            Vertex goingFromVertex = he.prevEdge.v;

            for (int j = 0; j < halfEdges.Count; j++)
            {
                if (i == j)
                {
                    continue;
                }

                HalfEdge heOpposite = halfEdges[j];

                if (goingFromVertex.position == heOpposite.v.position && goingToVertex.position == heOpposite.prevEdge.v.position)
                {
                    he.oppositeEdge = heOpposite;

                    break;
                }
            }
        }
        return halfEdges;
    }

    public static void OrientTrianglesClockwise(List<Triangle> triangles)
    {
        for (int i = 0; i < triangles.Count; i++)
        {
            Triangle tri = triangles[i];

            Vector2 v1 = new Vector2(tri.v1.position.x, tri.v1.position.z);
            Vector2 v2 = new Vector2(tri.v2.position.x, tri.v2.position.z);
            Vector2 v3 = new Vector2(tri.v3.position.x, tri.v3.position.z);

            if (!IsTriangleOrientedClockwise(v1, v2, v3))
            {
                tri.ChangeOrientation();
            }
        }
    }

    public static bool IsTriangleOrientedClockwise(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        bool isClockWise = true;

        float determinant = p1.x * p2.y + p3.x * p1.y + p2.x * p3.y - p1.x * p3.y - p3.x * p2.y - p2.x * p1.y;

        if (determinant > 0f)
        {
            isClockWise = false;
        }

        return isClockWise;
    }

    public static float IsPointInsideOutsideOrOnCircle(Vector2 aVec, Vector2 bVec, Vector2 cVec, Vector2 dVec)
    {
        float a = aVec.x - dVec.x;
        float d = bVec.x - dVec.x;
        float g = cVec.x - dVec.x;

        float b = aVec.y - dVec.y;
        float e = bVec.y - dVec.y;
        float h = cVec.y - dVec.y;

        float c = a * a + b * b;
        float f = d * d + e * e;
        float i = g * g + h * h;

        float determinant = (a * e * i) + (b * f * g) + (c * d * h) - (g * e * c) - (h * f * a) - (i * d * b);

        return determinant;
    }

    public static bool IsQuadrilateralConvex(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
    {
        bool isConvex = false;

        bool abc = IsTriangleOrientedClockwise(a, b, c);
        bool abd = IsTriangleOrientedClockwise(a, b, d);
        bool bcd = IsTriangleOrientedClockwise(b, c, d);
        bool cad = IsTriangleOrientedClockwise(c, a, d);

        if (abc && abd && bcd & !cad)
        {
            isConvex = true;
        }
        else if (abc && abd && !bcd & cad)
        {
            isConvex = true;
        }
        else if (abc && !abd && bcd & cad)
        {
            isConvex = true;
        }
        else if (!abc && !abd && !bcd & cad)
        {
            isConvex = true;
        }
        else if (!abc && !abd && bcd & !cad)
        {
            isConvex = true;
        }
        else if (!abc && abd && !bcd & !cad)
        {
            isConvex = true;
        }


        return isConvex;
    }

    private static void FlipEdge(HalfEdge one)
    {
        HalfEdge two = one.nextEdge;
        HalfEdge three = one.prevEdge;
        HalfEdge four = one.oppositeEdge;
        HalfEdge five = one.oppositeEdge.nextEdge;
        HalfEdge six = one.oppositeEdge.prevEdge;
        Vertex a = one.v;
        Vertex b = one.nextEdge.v;
        Vertex c = one.prevEdge.v;
        Vertex d = one.oppositeEdge.nextEdge.v;

        a.halfEdge = one.nextEdge;
        c.halfEdge = one.oppositeEdge.nextEdge;

        one.nextEdge = three;
        one.prevEdge = five;

        two.nextEdge = four;
        two.prevEdge = six;

        three.nextEdge = five;
        three.prevEdge = one;

        four.nextEdge = six;
        four.prevEdge = two;

        five.nextEdge = one;
        five.prevEdge = three;

        six.nextEdge = two;
        six.prevEdge = four;

        one.v = b;
        two.v = b;
        three.v = c;
        four.v = d;
        five.v = d;
        six.v = a;

        Triangle t1 = one.t;
        Triangle t2 = four.t;

        one.t = t1;
        three.t = t1;
        five.t = t1;

        two.t = t2;
        four.t = t2;
        six.t = t2;

        t1.v1 = b;
        t1.v2 = c;
        t1.v3 = d;

        t2.v1 = b;
        t2.v2 = d;
        t2.v3 = a;

        t1.halfEdge = three;
        t2.halfEdge = four;
    }
}

public class Vertex
{
    public Vector3 position;
    public HalfEdge halfEdge;
    public Triangle triangle;
    public Vertex prevVertex;
    public Vertex nextVertex;

    public bool isReflex; 
    public bool isConvex;
    public bool isEar;

    public Vertex(Vector3 position)
    {
        this.position = position;
    }

    public Vector2 GetPos2D_XZ()
    {
        Vector2 pos_2d_xz = new Vector2(position.x, position.z);

        return pos_2d_xz;
    }
}

public class HalfEdge
{
    public Vertex v;
    public Triangle t;
    public HalfEdge nextEdge;
    public HalfEdge prevEdge;
    public HalfEdge oppositeEdge;

    public HalfEdge(Vertex v)
    {
        this.v = v;
    }
}

public class Triangle
{
    public Vertex v1;
    public Vertex v2;
    public Vertex v3;

    public HalfEdge halfEdge;

    public Triangle(Vertex v1, Vertex v2, Vertex v3)
    {
        this.v1 = v1;
        this.v2 = v2;
        this.v3 = v3;
    }

    public Triangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        this.v1 = new Vertex(v1);
        this.v2 = new Vertex(v2);
        this.v3 = new Vertex(v3);
    }

    public Triangle(HalfEdge halfEdge)
    {
        this.halfEdge = halfEdge;
    }

    public void ChangeOrientation()
    {
        Vertex temp = this.v1;
        this.v1 = this.v2;
        this.v2 = temp;
    }
}

public class Edge
{
    public Vertex v1;
    public Vertex v2;

    public bool isIntersecting = false;

    public Edge(Vertex v1, Vertex v2)
    {
        this.v1 = v1;
        this.v2 = v2;
    }

    public Edge(Vector3 v1, Vector3 v2)
    {
        this.v1 = new Vertex(v1);
        this.v2 = new Vertex(v2);
    }

    public Vector2 GetVertex2D(Vertex v)
    {
        return new Vector2(v.position.x, v.position.z);
    }

    public void FlipEdge()
    {
        Vertex temp = v1;
        v1 = v2;
        v2 = temp;
    }
}