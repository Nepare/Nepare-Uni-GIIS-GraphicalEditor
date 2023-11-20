using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.Mathematics;
using System.Linq; 


public class CubeManipulation
{
    public struct Cube
    {
        public float theta;
        public float x, y, z;
        public (int x, int y) startCoords;
        public List<List<float>> vertices;
        public List<List<int>> edges;
        public bool isMoving, isRotating, isScaling, isPerspective, isDisplaying;
    }
    public static Cube cube;

    public static void SubscribeToRelevantEvents()
    {
        EventManager.OnCubeValues += SendValues;
        EventManager.OnRenderCube += RenderCube;
        EventManager.OnSpawnCube += SpawnCube;
    }

    private static void SpawnCube()
    {
        cube.vertices = TextFileProcessor.GetVertices();
        cube.edges = TextFileProcessor.GetEdges();
        cube.x = 0;
        cube.y = 0;
        cube.z = 0;
        cube.startCoords = (0, 0);
        cube.isMoving = false;
        cube.isRotating = false;
        cube.isScaling = false;
        cube.isPerspective = false;
        cube.isDisplaying = false;

        RenderCube();
    }

    private static List<List<float>> CompleteOperations(float theta, float x, float y, float z)
    {
        if (cube.isMoving)
        {
            for (int i = 0; i < cube.vertices.Count; i++)
            {
                cube.vertices[i][0] += x;
                cube.vertices[i][1] += y;
                cube.vertices[i][2] += z;
            }
            return cube.vertices;
        }
        else if (cube.isScaling)
        {
            float scaleFactor = 1f + theta;
            if (scaleFactor < 0.1f) scaleFactor = 0.1f;
            List<List<float>> vertices = new();
            vertices = ScaleFigure(scaleFactor);
            return vertices;
        }
        else if (cube.isRotating)
        {
            return RotateCube(theta, x, y, z);
        }
        else if (cube.isDisplaying)
        {
            cube.vertices = RotateForDisplay(theta, 1, 0, 0);
            return cube.vertices;
        }
        else return cube.vertices;
    }

    private static List<List<float>> ScaleFigure(float scaleFactor)
    {
        List<List<float>> scaledVertices = new();
        foreach (var vertex in cube.vertices)
        {
            List<float> scaledVertex = new();
            for (int i = 0; i < 3; i++)
                scaledVertex.Add(vertex[i] * scaleFactor);
            scaledVertices.Add(scaledVertex);
        }

        cube.vertices = scaledVertices;
        return scaledVertices;
    }

    private static List<List<float>> RotateCube(float theta, float x, float y, float z)
    {
        List<List<float>> centeredVertices = new();
        List<List<float>> rotatedVertices = new();
        foreach (var vertex in cube.vertices)
        {
            centeredVertices.Add(new List<float>() { vertex[0] - 25, vertex[1] - 25, vertex[2] - 25 });
        }
        foreach (var vertex in centeredVertices)
        {
            var rotatedVertex = vertex.ToArray();
            if (x != 0)
            {
                float[,] rotatedVertexMultidimensional = new float[1, 3] { { rotatedVertex[0], rotatedVertex[1], rotatedVertex[2] } };
                var rotatedVertexFloat = MatrixOps.MultiplyDoubleInverse(rotatedVertexMultidimensional, RotateX(theta));
                rotatedVertex = new float[3] { rotatedVertexFloat[0, 0], rotatedVertexFloat[0, 1], rotatedVertexFloat[0, 2] };
            }
            if (y != 0)
            {
                float[,] rotatedVertexMultidimensional = new float[1, 3] { { rotatedVertex[0], rotatedVertex[1], rotatedVertex[2] } };
                var rotatedVertexFloat = MatrixOps.MultiplyDoubleInverse(rotatedVertexMultidimensional, RotateY(theta));
                rotatedVertex = new float[3] { rotatedVertexFloat[0, 0], rotatedVertexFloat[0, 1], rotatedVertexFloat[0, 2] };
            }
            if (z != 0)
            {
                float[,] rotatedVertexMultidimensional = new float[1, 3] { { rotatedVertex[0], rotatedVertex[1], rotatedVertex[2] } };
                var rotatedVertexFloat = MatrixOps.MultiplyDoubleInverse(rotatedVertexMultidimensional, RotateZ(theta));
                rotatedVertex = new float[3] { rotatedVertexFloat[0, 0], rotatedVertexFloat[0, 1], rotatedVertexFloat[0, 2] };
            }
            rotatedVertices.Add(rotatedVertex.ToList());
        }
        List<List<float>> returnedVertices = new();
        foreach (var vertex in rotatedVertices)
        {
            returnedVertices.Add(new List<float>() { vertex[0] + 25, vertex[1] + 25, vertex[2] + 25 });
        }
        cube.vertices = returnedVertices;
        return returnedVertices;
    }

    private static float[,] RotateX(float theta)
    {
        float[,] result = new float[3, 3] 
        { 
            { 1,                0,                0},
            { 0,  math.cos(theta), -math.sin(theta)},
            { 0,  math.sin(theta),  math.cos(theta)}
        };
        return result;
    }

    private static float[,] RotateY(float theta)
    {
        float[,] result = new float[3, 3] 
        { 
            { math.cos(theta),               0,  math.sin(theta)},
            {               0,               1,                0},
            {-math.sin(theta),               0,  math.cos(theta)}
        };
        return result;
    }

    private static float[,] RotateZ(float theta)
    {
        float[,] result = new float[3, 3] 
        { 
            { math.cos(theta), -math.sin(theta),                0},
            { math.sin(theta),  math.cos(theta),                0},
            {               0,                0,                1}
        };
        return result;
    }

    private static List<List<float>> RotateForDisplay(float angle, float axisX, float axisY, float axisZ)
    {
        float angle_rad = math.radians(angle);
        float[] axis_rad = new float[3] { math.radians(axisX), math.radians(axisY), math.radians(axisZ) };
        
        float[,] rotation_matrix = new float[3, 3] {
            {math.cos(angle_rad) + math.pow(axis_rad[0], 2) * (1 - math.cos(angle_rad)),
             axis_rad[0] * axis_rad[1] * (1 - math.cos(angle_rad)) - axis_rad[2] * math.sin(angle_rad),
             axis_rad[0] * axis_rad[2] * (1 - math.cos(angle_rad)) + axis_rad[1] * math.sin(angle_rad)},

            {axis_rad[1] * axis_rad[0] * (1 - math.cos(angle_rad)) + axis_rad[2] * math.sin(angle_rad),
             math.cos(angle_rad) + math.pow(axis_rad[1], 2) * (1 - math.cos(angle_rad)),
             axis_rad[1] * axis_rad[2] * (1 - math.cos(angle_rad)) - axis_rad[0] * math.sin(angle_rad)},

            {axis_rad[2] * axis_rad[0] * (1 - math.cos(angle_rad)) - axis_rad[1] * math.sin(angle_rad),
             axis_rad[2] * axis_rad[1] * (1 - math.cos(angle_rad)) + axis_rad[0] * math.sin(angle_rad),
             math.cos(angle_rad) + math.pow(axis_rad[2], 2) * (1 - math.cos(angle_rad))}
        };
        List<List<float>> rotatedVertices = new();
        foreach (var vertex in cube.vertices)
        {
            List<float> rotatedVertex = new() {0, 0, 0};
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    rotatedVertex[i] += rotation_matrix[i, j] * vertex[j];
                }
            }
            rotatedVertices.Add(rotatedVertex);
        }
        return rotatedVertices;
    }

    private static List<List<float>> PerspectiveTransform(List<List<float>> coords, float distance)
    {
        List<List<float>> transformed_coords = new();
        foreach (var coord in coords)
        {
            float x = coord[0];
            float y = coord[1];
            float z = coord[2];
            float transformedX = x * distance / (z + distance);
            float transformedY = y * distance / (z + distance);
            transformed_coords.Add(new List<float>() { transformedX, transformedY });
        }

        return transformed_coords;
    }

    private static void RenderCube()
    {        
        if (cube.isPerspective)
        {
            cube.vertices = PerspectiveTransform(cube.vertices, 500 * math.sign(cube.x + cube.y + cube.z));
            for (int i = 0; i < 4; i++)
            {
                StraightLine.DrawLine(Convert.ToInt32(cube.vertices[i][0]), Convert.ToInt32(cube.vertices[i][1]), Convert.ToInt32(cube.vertices[(i + 1) % 4][0]), Convert.ToInt32(cube.vertices[(i + 1) % 4][1]), GameController.Mode.LineBresenham);
                StraightLine.DrawLine(Convert.ToInt32(cube.vertices[i + 4][0]), Convert.ToInt32(cube.vertices[i + 4][1]), Convert.ToInt32(cube.vertices[((i + 1) % 4) + 4][0]), Convert.ToInt32(cube.vertices[((i + 1) % 4) + 4][1]), GameController.Mode.LineBresenham);
                StraightLine.DrawLine(Convert.ToInt32(cube.vertices[i][0]), Convert.ToInt32(cube.vertices[i][1]), Convert.ToInt32(cube.vertices[i + 4][0]), Convert.ToInt32(cube.vertices[i + 4][1]), GameController.Mode.LineBresenham);
            }
        }
        else
        {
            cube.vertices = CompleteOperations(cube.theta, cube.x, cube.y, cube.z);

            foreach (var edge in cube.edges)
            {
                int x1 = Convert.ToInt32(cube.vertices[edge[0]][0]);
                int y1 = Convert.ToInt32(cube.vertices[edge[0]][1]);

                int x2 = Convert.ToInt32(cube.vertices[edge[1]][0]);
                int y2 = Convert.ToInt32(cube.vertices[edge[1]][1]);
                StraightLine.DrawLine(x1, y1, x2, y2, GameController.Mode.LineBresenham);
            }
        }
    }

    private static void SendValues(bool moving, bool rotating, bool scaling, bool perspective, bool display, int x, int y, int z)
    {
        cube.isMoving = moving;
        cube.isRotating = rotating;
        cube.isScaling = scaling;
        cube.isPerspective = perspective;
        cube.isDisplaying = display;
        cube.x = x;
        cube.y = y;
        cube.z = z;
        cube.theta = math.radians(cube.x + cube.y + cube.z);
    }
}
