using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
* The density manager handles the density within space.
* It currently uses a hashmap from a position in space to a 3D matrix of floats representing density.
*   (It may be better to try octrees, or other data structures eventually)
* It tracks which chunks have been updated to allow marchers and visualizers to pull from instead of iterating all chunks.
*   (This is currently a hashset of chunk keys, but it may be better to store only each individual group of 8 or more cubes that need to be updated)
* Functions
*   Get Density
*   Set Dentisy
*   Add to Density
*
*   Get Chunk Key
*   Get Get Local Index
*/
public class DensityManager : MonoBehaviour
{
    // The edge dimensions of a chunk
    public int chunkSize = 10;

    // The density field
    public Dictionary<Vector3Int, float[,,]> densityChunks = new Dictionary<Vector3Int, float[,,]>();
    
    // All chunks which need to be polygonalized
    public HashSet<Vector3Int> dirtyChunks = new HashSet<Vector3Int>();
    
    public UnityEvent onChunksBecameDirty;
    public int scale = 100;
    // Returns the density of x,y,z, and 0 if there is no entry.
    public float GetDensity(int x, int y, int z)
    {
        Vector3Int chunkKey = GetChunkKey(x, y, z);
        Vector3Int localIndex = GetLocalIndex(x, y, z);

        if (densityChunks.TryGetValue(chunkKey, out float[,,] chunk))
            return chunk[localIndex.x, localIndex.y, localIndex.z];
        
        // Default value if the chunk doesn't exist
        return 0.0f;
    }

    // density at x,y,z = value, then clamped to [0, 1]
    public void SetDensity(float value, int x, int y, int z)
    {
        Vector3Int chunkKey = GetChunkKey(x, y, z);
        Vector3Int localIndex = GetLocalIndex(x, y, z);

        if (!densityChunks.TryGetValue(chunkKey, out float[,,] chunk))
        {
            // Allocate a new chunk if it doesn't exist
            chunk = new float[chunkSize, chunkSize, chunkSize];
            densityChunks[chunkKey] = chunk;
        }

        chunk[localIndex.x, localIndex.y, localIndex.z] = Mathf.Clamp(value, 0.0f, 1.0f);
        dirtyChunks.Add(chunkKey);
    }

    // density at x,y,z += value, then clamped within [0, 1]
    public void AddToDensity(float value, int x, int y, int z)
    {
        Vector3Int chunkKey = GetChunkKey(x, y, z);
        Vector3Int localIndex = GetLocalIndex(x, y, z);
        
        if (!densityChunks.TryGetValue(chunkKey, out float[,,] chunk))
        {
            // Allocate a new chunk if it doesn't exist
            chunk = new float[chunkSize, chunkSize, chunkSize];
            densityChunks[chunkKey] = chunk;
        }

        chunk[localIndex.x, localIndex.y, localIndex.z] = Mathf.Clamp(
            chunk[localIndex.x, localIndex.y, localIndex.z] + value,
            0.0f, 1.0f
        );

        // If this chunk is not already marked dirty, do so
        if(!dirtyChunks.Contains(chunkKey))
            dirtyChunks.Add(chunkKey);
    }

    // Get the key to a chunk from a point in space within its bounds
    private Vector3Int GetChunkKey(int x, int y, int z)
    {
        return new Vector3Int(
            Mathf.FloorToInt(x / (float)chunkSize),
            Mathf.FloorToInt(y / (float)chunkSize),
            Mathf.FloorToInt(z / (float)chunkSize)
        );
    }

    // Get the index of a point within its chunk
    private Vector3Int GetLocalIndex(int x, int y, int z)
    {
        return new Vector3Int(
            (x % chunkSize + chunkSize) % chunkSize,
            (y % chunkSize + chunkSize) % chunkSize,
            (z % chunkSize + chunkSize) % chunkSize
        );
    }
    /*private Vector3Int GetLocalIndex(int x, int y, int z)
    {
        Vector3Int result = new Vector3Int(
            x % chunkSize,
            y % chunkSize,
            z % chunkSize
        );
        if(x < 0)
            result.x = chunkSize - Mathf.Abs(x % chunkSize) - 1;
        if (z < 0)
            result.z = chunkSize - Mathf.Abs(z % chunkSize) - 1;
        return result;
    }*/
}
