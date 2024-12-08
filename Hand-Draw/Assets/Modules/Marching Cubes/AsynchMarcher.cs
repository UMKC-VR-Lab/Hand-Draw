using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using MarchingCubes;
using UnityEngine;

[RequireComponent(typeof(DensityManager))]
public class AsynchMarcher : MonoBehaviour
{
    // References
    public DensityManager densityManager;
    public GameObject meshPrefab;

    public float isoLevel = 0.5f, updateDuration = 0.1f;
    private float scale = 100.0f;
    public int size = 1;

    private Dictionary<Vector3Int, GameObject> chunkMeshes = new Dictionary<Vector3Int, GameObject>();
    private Queue<GameObject> meshPool = new Queue<GameObject>();

    void Start()
    {
        scale = densityManager.scale;
        StartCoroutine(UpdateDirtyChunks());
    }

    private IEnumerator UpdateDirtyChunks()
    {
        yield return new WaitForSeconds(updateDuration);

        var dirtyChunksSnapshot = new List<Vector3Int>(densityManager.dirtyChunks);

        // Process dirty chunks
        foreach (var chunkKey in dirtyChunksSnapshot)
        {
            if (chunkMeshes.ContainsKey(chunkKey))
            {
                UpdateChunkMesh(chunkKey);
            }
            else
            {
                CreateChunkMesh(chunkKey);
            }
        }

        // Clear dirty chunks after processing
        densityManager.dirtyChunks.Clear();

        StartCoroutine(UpdateDirtyChunks());
    }

    private async void CreateChunkMesh(Vector3Int chunkKey)
    {
        transform.localScale = Vector3.one;
        GameObject chunkObject = GetFromPool();
        chunkObject.transform.position = chunkKey * densityManager.chunkSize * size;
        chunkObject.SetActive(true);
        chunkMeshes[chunkKey] = chunkObject;

        // Generate and assign the mesh asynchronously
        Mesh chunkMesh = await GenerateMeshAsync(chunkKey);
        ApplyMeshToGameObject(chunkObject, chunkMesh);
        transform.localScale = Vector3.one / scale;
    }

    private async void UpdateChunkMesh(Vector3Int chunkKey)
    {
        GameObject chunkObject = chunkMeshes[chunkKey];
        Mesh chunkMesh = await GenerateMeshAsync(chunkKey);
        ApplyMeshToGameObject(chunkObject, chunkMesh);
    }

    private async Task<Mesh> GenerateMeshAsync(Vector3Int chunkKey)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<float> pointValues = new List<float>(8);
        int dimension = densityManager.chunkSize + 1;

        for (int i = 0; i < 8; i++)
            pointValues.Add(0.0f);

        await Task.Run(() =>
        {
            lock (densityManager)
            {
                // Iterate through all points in the chunk
                for (int xi = 0; xi < dimension - 1; xi++)
                {
                    for (int yi = 0; yi < dimension - 1; yi++)
                    {
                        for (int zi = 0; zi < dimension - 1; zi++)
                        {
                            Vector3 tempVector3 = new Vector3(xi, yi, zi) * size;

                            // Retrieve density values for the cube corners
                            for (int i = 0; i < 8; i++)
                            {
                                Vector3Int globalCoord = chunkKey * densityManager.chunkSize + new Vector3Int(xi, yi, zi) +
                                                         new Vector3Int((int)tbl.points[i].x, (int)tbl.points[i].y, (int)tbl.points[i].z);
                                pointValues[i] = densityManager.GetDensity(globalCoord.x, globalCoord.y, globalCoord.z);
                            }

                            // Polygonalize the cube
                            Polygonalizer.PolygonalizeCube(isoLevel, 1.0f, tempVector3, ref pointValues, ref vertices, ref triangles);
                        }
                    }
                }
            }
        });

        // Assign vertices and triangles to the mesh
        if(mesh != null)
        {
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
        }

        return mesh;
    }

    private void ApplyMeshToGameObject(GameObject chunkObject, Mesh mesh)
    {
        if(gameObject == null) return;
        var meshFilter = chunkObject.GetComponent<MeshFilter>();
        if(gameObject == null) return;
        var meshCollider = chunkObject.GetComponent<MeshCollider>();

        if (meshFilter != null)
            meshFilter.mesh = mesh;
                else
            Debug.LogWarning($"MeshFilter missing on chunk object: {chunkObject.name}");

        if (meshCollider != null)
            meshCollider.sharedMesh = mesh;
        else
            Debug.LogWarning($"MeshCollider missing on chunk object: {chunkObject.name}");
    }

    private GameObject GetFromPool()
    {
        if (meshPool.Count > 0)
        {
            return meshPool.Dequeue();
        }
        else
        {
            return Instantiate(meshPrefab, transform);
        }
    }

    private void ReturnToPool(GameObject chunkObject)
    {
        chunkObject.SetActive(false);
        var meshFilter = chunkObject.GetComponent<MeshFilter>();
        if (meshFilter != null)
            meshFilter.mesh = null;

        var meshCollider = chunkObject.GetComponent<MeshCollider>();
        if (meshCollider != null)
            meshCollider.sharedMesh = null;

        meshPool.Enqueue(chunkObject);
    }
}
