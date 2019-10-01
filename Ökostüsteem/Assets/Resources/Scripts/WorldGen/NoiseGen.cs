using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseGen
{

    public static float[,] GenerateNoise(int mapWidth, int mapHeigth, float mapScale, int mapLevels, float mapIntensity, float mapOpacity, float xOffset, float yOffset, float mapOpacity2)
    {

        float[,] noiseMap = new float[mapWidth, mapHeigth];

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeigth; y++)
            {

                float opacity = 1;
                float intensity = 1;

                float pixelValue = 0;

                for (int i = 0; i < mapLevels; i++)
                {
                    float perlinX = (float)x / mapWidth / mapScale * intensity + xOffset;
                    float perlinY = (float)y / mapHeigth / mapScale * intensity + yOffset;

                    float perlinValue = Mathf.PerlinNoise(perlinX, perlinY);

                    pixelValue += perlinValue * opacity;
                    //pixelValue = Vector3.Lerp(new Vector3(pixelValue, 0, 0), new Vector3(perlinValue * opacity, 0, 0), 0.5f).x;

                    opacity *= mapOpacity;
                    intensity *= mapIntensity;

                }
                noiseMap[x, y] = pixelValue/mapOpacity2;
            }
        }
        return noiseMap;
    }
}
