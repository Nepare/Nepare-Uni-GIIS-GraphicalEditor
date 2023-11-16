using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

public class TextFileProcessor
{
    private static string path = "Assets/Resources/cube.txt";

    public static List<List<int>> GetVertices()
    {
        List<List<int>> vertices = new List<List<int>>();
        StreamReader reader = new StreamReader(path);
        for (int i = 0; i < 8; i++)
        {
            List<int> vertex = new List<int>();
            vertex = reader.ReadLine().Split(" ").Select(s => int.Parse(s)).ToList();
            vertices.Add(vertex);
        }
        reader.Close();
        return vertices;
    }

    public static List<List<int>> GetEdges()
    {
        List<List<int>> edges = new List<List<int>>();
        StreamReader reader = new StreamReader(path);
        for (int i = 0; i < 21; i++)
        {
            if (i < 9) { reader.ReadLine(); continue; }
            List<int> edge = new List<int>();
            edge = reader.ReadLine().Split(" ").Select(s => int.Parse(s)).ToList();
            edges.Add(edge);
        }
        reader.Close();
        return edges;
    }
}
