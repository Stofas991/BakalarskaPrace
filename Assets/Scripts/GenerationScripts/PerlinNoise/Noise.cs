/**
 * File: noise.cs
 * Author: Kryštof Glos
 * Last Modified: 9.4.2024
 * Description: Contains methods for generating noise maps using Perlin noise.
 */

using UnityEngine;

public static class noise
{
    /**
     * Method: GenerateNoiseMap
     * Description: Generates a 2D noise map using Perlin noise algorithm.
     * Parameters:
     *   - mapWidth: Width of the noise map
     *   - mapHeight: Height of the noise map
     *   - seed: Seed value for generating random numbers
     *   - scale: Scale factor for adjusting the frequency of the noise
     *   - octaves: Number of octaves used in the noise generation
     *   - persistance: Persistance value for controlling the amplitude decrease per octave
     *   - lacunarity: Lacunarity value for controlling the frequency increase per octave
     *   - offset: Offset vector for adding randomness to the noise map
     * Returns:
     *   - float[,]: The generated noise map
     */
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        // Generate noise map
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {

                    // Calculate sample coordinates
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                    // Generate Perlin noise value and add to noise height
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    // Update amplitude and frequency for next octave
                    amplitude *= persistance;
                    frequency *= lacunarity;


                }
                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;
            }
        }

        // Normalize noise map values
        for (int y = 0; y < mapHeight; y++)
        {
            //going over width too
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }
        return noiseMap;
    }
}
