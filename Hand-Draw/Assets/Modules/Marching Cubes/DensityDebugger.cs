using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DensityManager))]
public class DensityDebugger : MonoBehaviour
{
    // References
    public DensityManager densityManager;

    // Debugging parameters
    [Range(0.01f, 0.5f)] public float sphereSpacing = 0.05f;
    [Range(0.01f, 0.5f)] public float sphereSize = 0.1f;
    [Range(10f, 500f)] public float debugViewDistance = 50f;

    // Rendering resources
    public Mesh sphereMesh;
    public Material sphereMaterial;

    // State for instanced rendering
    private List<Matrix4x4> sphereMatrices = new List<Matrix4x4>();

    private void Start()
    {
        if (densityManager == null) densityManager = GetComponent<DensityManager>();
        if (sphereMesh == null || sphereMaterial == null)
        {
            Debug.LogError("Sphere mesh and material must be assigned.");
            enabled = false;
        }
    }

    private void Update()
    {
        DrawDebugSpheres();
    }

    private void DrawDebugSpheres()
    {
        sphereMatrices.Clear();

        foreach (Vector3Int chunkKey in densityManager.densityChunks.Keys)
        {
            // Skip chunks outside the debug view distance
            if (!IsChunkInView(chunkKey))
                continue;

            AddChunkSpheresToMatrixList(chunkKey, densityManager.densityChunks[chunkKey]);
        }

        // Render all spheres in one draw call
        if (sphereMatrices.Count > 0)
        {
            Graphics.DrawMeshInstanced(sphereMesh, 0, sphereMaterial, sphereMatrices);
        }
    }

    private void AddChunkSpheresToMatrixList(Vector3Int chunkKey, float[,,] densityChunk)
    {
        for (int xi = 0; xi < densityManager.chunkSize; xi++)
        {
            for (int yi = 0; yi < densityManager.chunkSize; yi++)
            {
                for (int zi = 0; zi < densityManager.chunkSize; zi++)
                {
                    float density = densityChunk[xi, yi, zi];
                    if (density > 0f)
                    {
                        Vector3 worldPos = GetWorldPosition(chunkKey, new Vector3Int(xi, yi, zi));
                        var matrix = Matrix4x4.TRS(worldPos, Quaternion.identity, Vector3.one * density * sphereSize);
                        sphereMatrices.Add(matrix);
                    }
                }
            }
        }
    }

    private bool IsChunkInView(Vector3Int chunkKey)
    {
        Vector3 chunkWorldPos = (Vector3)chunkKey * densityManager.chunkSize * sphereSpacing;
        return Vector3.Distance(Camera.main.transform.position, chunkWorldPos) < debugViewDistance;
    }

    private Vector3 GetWorldPosition(Vector3Int chunkKey, Vector3Int localPos)
    {
        Vector3 chunkWorldPos = (Vector3)chunkKey * densityManager.chunkSize * sphereSpacing;
        Vector3 localWorldPos = (Vector3)localPos * sphereSpacing;
        return chunkWorldPos + localWorldPos;
    }
}
