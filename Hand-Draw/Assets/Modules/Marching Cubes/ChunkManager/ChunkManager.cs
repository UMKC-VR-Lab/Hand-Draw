using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    SimplexNoise noiseGenerator = new SimplexNoise();
    //Populate the correct densities in a three dimensional list.
    List<List<List<float>>> densityField = new List<List<List<float>>>();
    public int dimension = 10;
    public float isoLevel = 0.5f;
    public float noiseScale = 0.1f;
    public Material chunkMaterial;
    void Start()
    {
        Vector3 tempVector3;
        List<List<float>> planes = new List<List<float>>(dimension);
        List<float> lines = new List<float>(dimension);
        for(int xi = 0; xi <= dimension; xi++)
        {
            for (int yi = 0; yi <= dimension; yi++)
            {
                for (int zi = 0; zi <= dimension; zi++)
                {

                    tempVector3.x = xi;
                    tempVector3.y = yi;
                    tempVector3.z = zi;
                    lines.Add(noiseGenerator.noise(tempVector3 * noiseScale));
                    //GameObject tempSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    //tempSphere.transform.position = tempVector3;
                    //tempSphere.transform.parent = transform;
                    //tempSphere.transform.localScale = noiseGenerator.noise(tempVector3 * noiseScale) * Vector3.one;
                }
                planes.Add(lines);
            }
            densityField.Add(planes);
        }
        MarchingCubes.GenerateChunk(isoLevel, 0.1f, Vector3.zero, dimension, densityField, chunkMaterial);
    }
    //public static GameObject GenerateChunk(float isoLevel, float size, Vector3 position, int dimension, List<float> densityValues)
}
