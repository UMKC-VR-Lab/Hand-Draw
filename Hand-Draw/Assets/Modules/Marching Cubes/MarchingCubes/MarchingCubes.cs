using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingCubes : MonoBehaviour
{
    //PolygonalizeCube(float isoLevel, float size, Vector3 position, ref List<float> pointValues, ref List<Vector3> vertices, ref List<int> triangles)
    public static GameObject GenerateChunk(float isoLevel, float size, Vector3 position, int dimension, List<List<List<float>>> densityValues, Material material)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        Vector3 tempVector3;
        //Find the densities for the points of this cube
        List<float> tempDensities = new List<float>(8);
        for (int i = 0; i < 8; i++)
            tempDensities.Add(0);
        for (int xi = 0; xi < dimension; xi++)
        {
            for (int yi = 0; yi < dimension; yi++)
            {
                for (int zi = 0; zi < dimension; zi++)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        tempDensities[i] = densityValues
                            [xi + (int)tbl.points[i].x][yi + (int)tbl.points[i].y][zi + (int)tbl.points[i].z];
                        Debug.Log(
                              (xi + (int)tbl.points[i].x) + " "
                            + (yi + (int)tbl.points[i].y) + " "
                            + (zi + (int)tbl.points[i].z) + ": " + tempDensities[i]);
                    }
                    tempVector3.x = xi;
                    tempVector3.y = yi;
                    tempVector3.z = zi;
                    Polygonalizer.PolygonalizeCube(isoLevel, size, (position + tempVector3), 
                        ref tempDensities, ref vertices, ref triangles);
                }
            }
        }
        GameObject returnGameObject = new GameObject();
        returnGameObject.AddComponent<MeshRenderer>().material = material;
        returnGameObject.AddComponent<MeshFilter>();
        Mesh mesh = returnGameObject.GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        return returnGameObject;
    }
}
