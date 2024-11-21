using UnityEngine;
using System.Collections;
public class SimplexNoise
{
    int[] A = new int[3];
    int[] T;
    int i, j, k;
    float s, u, v, w, aThird = 0.333333333f, aSixth = 0.166666667f;

    public SimplexNoise()
    {
        if (T == null)
        {
            System.Random rand = new System.Random();
            T = new int[8];
            for (int q = 0; q < 8; q++)
                T[q] = rand.Next();
        }
    }

    public SimplexNoise(string seed)
    {
        T = new int[8];
        string[] seed_parts = seed.Split(new char[] { ' ' });

        for (int q = 0; q < 8; q++)
        {
            int b;
            try
            {
                b = int.Parse(seed_parts[q]);
            }
            catch
            {
                b = 0x0;
            }
            T[q] = b;
        }
    }

    public string GetSeed()
    {
        string seed = "";
        for (int q = 0; q < 8; q++)
        {
            seed += T[q].ToString();
            if (q < 7)
                seed += " ";
        }
        return seed;
    }

    public float coherentNoise(float x, float y, float z, int octaves = 1, int multiplier = 25, float amplitude = 0.5f, float lacunarity = 2, float persistence = 0.9f)
    {
        Vector3 v3 = new Vector3(x, y, z) / multiplier;
        float val = 0;
        for (int n = 0; n < octaves; n++)
        {
            val += noise(v3.x, v3.y, v3.z) * amplitude;
            v3 *= lacunarity;
            amplitude *= persistence;
        }
        return val;
    }

    public int getDensity(Vector3 loc)
    {
        float val = coherentNoise(loc.x, loc.y, loc.z);
        return (int)Mathf.Lerp(0, 255, val);
    }

    public float noise(float x, float y, float z)
    {
        x *= 100;
        y *= 100;
        z *= 100;
        s = (x + y + z) * aThird;
        i = (x + s) > 0 ? (int)(x + s) : (int)(x + s) - 1;
        j = (y + s) > 0 ? (int)(y + s) : (int)(y + s) - 1;
        k = (z + s) > 0 ? (int)(z + s) : (int)(z + s) - 1;
        s = (i + j + k) * aSixth;
        u = x - i + s;
        v = y - j + s;
        w = z - k + s;
        A[0] = 0; A[1] = 0; A[2] = 0;
        int hi = u >= w ? u >= v ? 0 : 1 : v >= w ? 1 : 2;
        int lo = u < w ? u < v ? 0 : 1 : v < w ? 1 : 2;
        return kay(hi) + kay(3 - hi - lo) + kay(lo) + kay(0);
    }

    public float noise(Vector3 pos)
    {
        s = (pos.x + pos.y + pos.z) * aThird;
        i = (pos.x + s) > 0 ? (int)(pos.x + s) : (int)(pos.x + s) - 1;
        j = (pos.y + s) > 0 ? (int)(pos.y + s) : (int)(pos.y + s) - 1;
        k = (pos.z + s) > 0 ? (int)(pos.z + s) : (int)(pos.z + s) - 1;
        s = (i + j + k) * aSixth;
        u = pos.x - i + s;
        v = pos.y - j + s;
        w = pos.z - k + s;
        A[0] = 0; A[1] = 0; A[2] = 0;
        int hi = u >= w ? u >= v ? 0 : 1 : v >= w ? 1 : 2;
        int lo = u < w ? u < v ? 0 : 1 : v < w ? 1 : 2;
        return (kay(hi) + kay(3 - hi - lo) + kay(lo) + kay(0)) + 0.5f;
    }

    float kay(int a)
    {
        s = (A[0] + A[1] + A[2]) * aSixth;
        float x = u - A[0] + s;
        float y = v - A[1] + s;
        float z = w - A[2] + s;
        float t = 0.6f - x * x - y * y - z * z;
        int h = shuffle(i + A[0], j + A[1], k + A[2]);
        A[a]++;
        if (t < 0) return 0;
        int b5 = h >> 5 & 1;
        int b4 = h >> 4 & 1;
        int b3 = h >> 3 & 1;
        int b2 = h >> 2 & 1;
        int b1 = h & 3;

        float p = b1 == 1 ? x : b1 == 2 ? y : z;
        float q = b1 == 1 ? y : b1 == 2 ? z : x;
        float r = b1 == 1 ? z : b1 == 2 ? x : y;

        p = b5 == b3 ? -p : p;
        q = b5 == b4 ? -q : q;
        r = b5 != (b4 ^ b3) ? -r : r;
        t *= t;
        return 8 * t * t * (p + (b1 == 0 ? q + r : b2 == 0 ? q : r));
    }

    int shuffle(int i, int j, int k)
    {
        return b(i, j, k, 0) + b(j, k, i, 1) + b(k, i, j, 2) + b(i, j, k, 3) + b(j, k, i, 4) + b(k, i, j, 5) + b(i, j, k, 6) + b(j, k, i, 7);
    }

    int b(int i, int j, int k, int B)
    {
        return T[b(i, B) << 2 | b(j, B) << 1 | b(k, B)];
    }

    int b(int N, int B)
    {
        return N >> B & 1;
    }

    int fastfloor(float n)
    {
        return n > 0 ? (int)n : (int)n - 1;
    }
}