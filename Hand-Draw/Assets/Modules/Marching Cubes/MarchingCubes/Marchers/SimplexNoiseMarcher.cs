using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshCollider)), RequireComponent(typeof(Mesh))]
public class SimplexNoiseMarcher : MonoBehaviour
{
    // References
    private SimplexNoise noiseGenerator = new SimplexNoise();
    private MeshCollider meshCollider;
    public Mesh mesh;

    // Parameters
    public float isoLevel = 0.5f, size = 1.0f, speed = 2.0f, updateDuration = 0.05f;
    private float offsetTime = 0;
    public int dimension = 10;

    // Generation 
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    private List<float> pointValues = new List<float>(new float[8]);
    private Vector3 tempVector3;

    void Start()
    {
        // Ensure the meshCollider and mesh are referenced
        if(meshCollider == null) meshCollider = GetComponent<MeshCollider>();
        if(mesh == null) mesh = GetComponent<MeshFilter>().mesh;

        StartCoroutine("GenerateNewField");
    }

    public IEnumerator GenerateNewField()
    {
        yield return new WaitForSeconds(updateDuration);
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
