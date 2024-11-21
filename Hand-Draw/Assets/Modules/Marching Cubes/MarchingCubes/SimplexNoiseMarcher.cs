using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplexNoiseMarcher : MonoBehaviour
{
    private MeshCollider meshCollider;
    public float isoLevel = 0.5f, size = 1.0f, speed = 2.0f;
    public Mesh mesh;
    SimplexNoise noiseGenerator = new SimplexNoise();
    List<float> pointValues = new List<float>();
    float offsetTime = 0;
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    private Vector3 tempVector3;
    public int dimension = 10;

    void Start()
    {
        meshCollider = GetComponent<MeshCollider>();
        mesh = GetComponent<MeshFilter>().mesh;

        for (int i = 0; i < 8; i++)
        {
            pointValues.Add(noiseGenerator.noise(tbl.points[i]));
            //Debug.Log(points[i]);
        }
        StartCoroutine("GenerateNewField");
    }

    public IEnumerator GenerateNewField()
    {
        yield return new WaitForSeconds(0.05f);
        //noiseGenerator = new SimplexNoise();
        vertices.Clear();
        triangles.Clear();
        mesh.Clear();


        offsetTime += Time.deltaTime * speed;

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
                        pointValues[i] = noiseGenerator.noise(0.1f * (tbl.points[i] + Vector3.one * offsetTime + tempVector3));
                        //March();
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
