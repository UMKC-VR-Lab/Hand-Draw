using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class HandInteractionDensityMarcher : MonoBehaviour
{
    private MeshCollider meshCollider;
    public float isoLevel = 0.5f, size = 0.1f;
    public Mesh mesh;
    List<float> pointValues = new List<float>();
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    private Vector3 tempVector3;
    public int dimension = 10;
    public DensityField densityField;
    public float updateDuration = 0.1f;

    void Start()
    {
        meshCollider = GetComponent<MeshCollider>();
        mesh = GetComponent<MeshFilter>().mesh;
        for (int i = 0; i < 8; i++)
            pointValues.Add(0.0f);
        StartCoroutine("GenerateNewField");
    }

    public IEnumerator GenerateNewField()
    {
        yield return new WaitForSeconds(updateDuration);
        vertices.Clear();
        triangles.Clear();
        mesh.Clear();

        for (int xi = 0; xi < dimension - 1; xi++)
        {
            for (int yi = 0; yi < dimension - 1; yi++)
            {
                for (int zi = 0; zi < dimension - 1; zi++)
                {
                    tempVector3.x = xi;
                    tempVector3.y = yi;
                    tempVector3.z = zi;
                    for (int i = 0; i < 8; i++)
                    {
                        pointValues[i] = densityField.densityField[
                            xi + (int)tbl.points[i].x, 
                            yi + (int)tbl.points[i].y, 
                            zi + (int)tbl.points[i].z
                            ];
                    }

                    Polygonalizer.PolygonalizeCube(isoLevel, size, transform.position + tempVector3, ref pointValues, ref vertices, ref triangles);
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        StartCoroutine("GenerateNewField");
    }
}
