using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Start kood, alustab world geni

public class DisplayNoise : MonoBehaviour
{

    public int width;
    public int heigth;
    public float scale;
    public int levels;
    public float intensity;
    public float opacity;
    public float xOffset;
    public float yOffset;
    public float opacity2;
    public float heigths;
    public int coast;
    public float waterLevel;
    public int mapRadius;

    void Start()
    {

        //Teeb world geni randomiks

        xOffset = Random.Range(0, 9999);
        yOffset = Random.Range(0, 9999);

        Test();
    }

    void Test()
    {

        //Teeb kindlaks et mapi laiused oleksid paaritud arvud

        if (width % 2 == 0)
        {
            width += 1;
        }
        if (heigth % 2 == 0)
        {
            heigth += 1;
        }

        //Genereerib noisemapi

        float[,] noiseMap = NoiseGen.GenerateNoise(width,heigth,scale,levels,intensity,opacity,xOffset,yOffset,opacity2);

        //DrawTexture drawScript = GetComponent<DrawTexture>();

        //drawScript.DrawNoise(width,heigth,noiseMap);

        MakeMesh makeMesh = GetComponent<MakeMesh>();

        makeMesh.CreateMesh(width, heigth, noiseMap, heigths, coast, waterLevel, mapRadius);
    }
}
