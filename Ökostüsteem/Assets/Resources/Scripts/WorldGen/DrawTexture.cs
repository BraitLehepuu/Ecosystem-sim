﻿//Ökosüsteemi loomine tehisintellekti abil
//@autor Ralf Brait Lehepuu
//
//Mõningane eeskuju https://www.youtube.com/watch?v=wbpMiKiSKm8&list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Teeb mustvalge pildi noisist (Pole vaja lõplikuks programmiks)

public class DrawTexture : MonoBehaviour
{

    public Renderer textureRenderer;

    public void DrawNoise(int textWidth, int textHeigth, float[,] noiseMap)
    {
        Texture2D noiseTexture = new Texture2D(textWidth, textHeigth);

        Color[] noiseColor = new Color[textWidth * textHeigth];

        for (int y = 0; y < textHeigth; y++)
        {
            for (int x = 0; x < textWidth; x++)
            {
                noiseColor[y * textHeigth + x] = Color.Lerp(Color.white, Color.black, noiseMap[x,y]);
            }
        }

        noiseTexture.SetPixels(noiseColor);
        noiseTexture.Apply();

        textureRenderer.sharedMaterial.mainTexture = noiseTexture;
        textureRenderer.transform.localScale = new Vector3(textWidth, 1, textHeigth);
    }
}
