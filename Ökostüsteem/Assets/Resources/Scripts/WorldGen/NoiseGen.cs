//Ökosüsteemi loomine tehisintellekti abil
//@autor Ralf Brait Lehepuu
//
//Mõningane eeskuju https://www.youtube.com/watch?v=wbpMiKiSKm8&list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Kood mis genereerib meshi tegemiseks perlin noise

public static class NoiseGen
{

    public static float[,] GenerateNoise(int mapWidth, int mapHeigth, float mapScale, int mapLevels, float mapIntensity, float mapOpacity, float xOffset, float yOffset, float mapOpacity2)
    {

        //Genereerib perlin noise ja assignib selle väärtuse 2D arrayse
        //Väärtus arrays määrab selle vertexi kõrguse

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

                    opacity *= mapOpacity;
                    intensity *= mapIntensity;

                }
                noiseMap[x, y] = pixelValue/mapOpacity2;
            }
        }
        return noiseMap;
    }
}
