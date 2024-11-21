using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygonalizer : MonoBehaviour
{
    public static void PolygonalizeCube(float isoLevel, float size, Vector3 position, ref List<float> pointValues, ref List<Vector3> vertices, ref List<int> triangles)
    {
        //Find the index of the triangle configuration.
        int cubeIndex = FindIndex(isoLevel, pointValues);

        //If we're not on the surface, stop.
        if (OnEdgeOfSurface(cubeIndex)) return;

        //Add vertices and triangles.
        AddVertices(cubeIndex, position, size, isoLevel, ref pointValues, ref vertices, ref triangles);
    }
    private static int FindIndex(float isoLevel, List<float> pointValues)
    {
        int cubeIndex = 0;
        for (int i = 0; i < 8; i++)
            if (pointValues[i] < isoLevel)
                cubeIndex += tbl.powsOfTwo[i];
        return cubeIndex;
    }
    private static void AddVertices(int cubeIndex, Vector3 position, float size, float isoLevel, ref List<float> pointValues, ref List<Vector3> vertices, ref List<int> triangles)
    {
        for (int i = 0; tbl.tris[cubeIndex, i] != -1; i += 3)
        {
            int a0 = tbl.cornerIndexAFromEdge[tbl.tris[cubeIndex, i]];
            int b0 = tbl.cornerIndexBFromEdge[tbl.tris[cubeIndex, i]];

            int a1 = tbl.cornerIndexAFromEdge[tbl.tris[cubeIndex, i + 1]];
            int b1 = tbl.cornerIndexBFromEdge[tbl.tris[cubeIndex, i + 1]];

            int a2 = tbl.cornerIndexAFromEdge[tbl.tris[cubeIndex, i + 2]];
            int b2 = tbl.cornerIndexBFromEdge[tbl.tris[cubeIndex, i + 2]];

            vertices.Add((VertexInterp(isoLevel, tbl.points[a0], tbl.points[b0], pointValues[a0], pointValues[b0]) + position) * size);
            vertices.Add((VertexInterp(isoLevel, tbl.points[a1], tbl.points[b1], pointValues[a1], pointValues[b1]) + position) * size);
            vertices.Add((VertexInterp(isoLevel, tbl.points[a2], tbl.points[b2], pointValues[a2], pointValues[b2]) + position) * size);
            triangles.Add(triangles.Count);
            triangles.Add(triangles.Count);
            triangles.Add(triangles.Count);
        }
    }
    private static bool OnEdgeOfSurface(int cubeIndex)
    {
        return cubeIndex == 0 || cubeIndex == 255;
    }
    private static Vector3 VertexInterp(float isoLevel, Vector3 p1, Vector3 p2, float valp1, float valp2)
    {
        if (Mathf.Abs(isoLevel - valp1) < 0.00001f) return (p1);
        if (Mathf.Abs(isoLevel - valp2) < 0.00001f) return (p2);
        if (Mathf.Abs(valp1 - valp2) < 0.00001f) return (p1);
        float mu = (isoLevel - valp1) / (valp2 - valp1);
        Vector3 p;
        p.x = p1.x + mu * (p2.x - p1.x);
        p.y = p1.y + mu * (p2.y - p1.y);
        p.z = p1.z + mu * (p2.z - p1.z);
        return p;
    }
}
