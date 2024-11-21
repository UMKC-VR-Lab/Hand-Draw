using UnityEngine;

public class DensityField : MonoBehaviour
{
    public int dimension = 10;
    public float[,,] densityField;
    public GameObject[,,] indicatorSpheres;
    public GameObject sphere;

    private void Start()
    {
        densityField = new float[dimension, dimension, dimension];
        if (debugSpheres)
        {
            indicatorSpheres = new GameObject[dimension, dimension, dimension];
            for (int xi = 0; xi < dimension; xi++)
            {
                for (int yi = 0; yi < dimension; yi++)
                {
                    for (int zi = 0; zi < dimension; zi++)
                    {
                        indicatorSpheres[xi, yi, zi] = Instantiate(sphere);
                        indicatorSpheres[xi, yi, zi].transform.position = new Vector3(xi, yi, zi) * 0.1f;
                        indicatorSpheres[xi, yi, zi].transform.parent = transform;
                        densityField[xi, yi, zi] = 0.0f;
                    }
                }
            }
        }
    }

    public bool debugSpheres = false;
    private void FixedUpdate()
    {
        if(debugSpheres)
            for(int xi = 0; xi < dimension; xi++)
            {
                for(int yi = 0; yi < dimension; yi++)
                {
                    for(int zi = 0; zi < dimension; zi++)
                    {
                        indicatorSpheres[xi, yi, zi].transform.localScale = densityField[xi, yi, zi] * Vector3.one;
                    }
                }
            }
    }

    public void SetDensity(float value, int x, int y, int z)
    {
        if(value < 0.0f)
            densityField[x, y, z] = 0.0f;
        else if(value > 1.0f) 
            densityField[x, y, z] = 1.0f;
        else
            densityField[x, y, z] = value;
    }
    public void AddToDensity(float value, int x, int y, int z)
    {
        if (value + densityField[x, y, z] < 0)
            densityField[x, y, z] = 0.0f;
        else if (value + densityField[x, y, z] > 1.0f)
            densityField[x, y, z] = 1.0f;
        else
            densityField[x, y, z] += value;

    }
}
