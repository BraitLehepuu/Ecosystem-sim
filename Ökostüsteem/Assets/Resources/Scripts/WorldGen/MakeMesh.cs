//Ökosüsteemi loomine tehisintellekti abil
//@autor Ralf Brait Lehepuu
//
//Mõningane eeskuju https://www.youtube.com/watch?v=wbpMiKiSKm8&list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Teeb noisemapist meshi ja värvib selle kõrguste järgi (vesi ja muru)

public class MakeMesh : MonoBehaviour
{

    int AmapWidth;
    float AwaterLevel;
    int AanimalSpawnDist;
    Vector3[] AgrassSpawns;
    Vector3[] grassSpawns;
    int grassMapWidth;
    int grassMapHeight;
    float grassMapWaterLevel;

    private void Update()
    {
        /*if (Input.GetKeyDown("return"))
        {
            for (int i = 0; i < GameObject.FindGameObjectsWithTag("AnimalHunter").Length; i++)
            {
                Destroy(GameObject.FindWithTag("AnimalHunter"));
            }
            for (int i = 0; i < GameObject.FindGameObjectsWithTag("AnimalPlanteater").Length; i++)
            {
                Destroy(GameObject.FindWithTag("AnimalPlanteater"));
            }
            ResetArea();
        }
        if ((GameObject.FindGameObjectsWithTag("AnimalHunter").Length <= 1 || GameObject.FindGameObjectsWithTag("AnimalPlanteater").Length <= 1) && Time.time > 5)
        {
            if (GameObject.FindGameObjectsWithTag("AnimalHunter").Length == 1)
            {
                GameObject.FindGameObjectWithTag("AnimalHunter").GetComponent<HunterAgent>().Done();
            }
            if (GameObject.FindGameObjectsWithTag("AnimalPlanteater").Length == 1)
            {
                GameObject.FindGameObjectWithTag("AnimalPlanteater").GetComponent<HunterAgent>().Done();
            }
            else
            {
                ResetArea();
            }
        }*/
    }

    public void CreateMesh(int mapWidth, int mapHeigth, float[,] noiseMap, float heigths, int coast, float waterLevel, int mapRadius, int animalSpawnDist)
    {
        int triangleCount = 0;

        Mesh terrainMesh = new Mesh();

        Vector3[] verts = new Vector3[mapWidth * mapHeigth];
        int[] triangles = new int[(mapWidth * mapHeigth -2) * 3*2];
        Color32[] colors = new Color32[verts.Length];
        grassSpawns = new Vector3[mapWidth * mapHeigth];

        grassMapWidth = mapWidth;
        grassMapHeight = mapWidth;
        grassMapWaterLevel = waterLevel;

        //Loob kõik vertexid ning annab neile kõrguse noisemapi järgi
        //Loob meshi triangled

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeigth; y++)
            {
                if (x != 0)
                {
                    if (y != 0)
                    {
                        triangles[triangleCount] = x * mapWidth + y;
                        triangles[triangleCount + 1] = x * mapWidth + (y - 1);
                        triangles[triangleCount + 2] = (x - 1) * mapWidth + y;
                        triangleCount += 3;
                    }
                }
                if (x < mapWidth - 1)
                {
                    if (y != mapHeigth-1)
                    {
                        triangles[triangleCount] = x * mapWidth + y;
                        triangles[triangleCount + 1] = x * mapWidth + (y + 1);
                        triangles[triangleCount + 2] = (x + 1) * mapWidth + y;
                        triangleCount += 3;
                    }
                }
                verts[x * mapWidth + y] = new Vector3(x- mapWidth/2, noiseMap[x, y] * heigths, y - mapWidth / 2);
                grassSpawns[x * mapWidth + y] = new Vector3(x - mapWidth / 2, noiseMap[x, y] * heigths, y - mapWidth / 2);
                colors[x * mapWidth + y] = Color.green;

                //Teeb kindlaks, et mapi ääred ei oleks kõrgemad kui vesi
                if (Vector3.Distance(new Vector3(verts[(mapWidth+1)/2 * mapWidth + (mapHeigth+1)/2].x, 0, verts[(mapWidth + 1) / 2 * mapWidth + (mapHeigth + 1) / 2].z), new Vector3(x - mapWidth / 2, 0, y - mapWidth / 2)) > (mapWidth-mapRadius)/2)
                {
                    float distance = Vector3.Distance(new Vector3(verts[(mapWidth + 1) / 2 * mapWidth + (mapHeigth + 1) / 2].x, 0, verts[(mapWidth + 1) / 2 * mapWidth + (mapHeigth + 1) / 2].z), new Vector3(x - mapWidth / 2, 0, y - mapWidth / 2)) - (mapWidth - mapRadius) / 2;
                    verts[x * mapWidth + y] = new Vector3(verts[x * mapWidth + y].x, verts[x * mapWidth + y].y - distance/coast, verts[x * mapWidth + y].z);
                    grassSpawns[x * mapWidth + y] = verts[x * mapWidth + y];
                    if (Vector3.Distance(new Vector3(verts[(mapWidth + 1) / 2 * mapWidth + (mapHeigth + 1) / 2].x, 0, verts[(mapWidth + 1) / 2 * mapWidth + (mapHeigth + 1) / 2].z), new Vector3(x - mapWidth / 2, 0, y - mapWidth / 2)) > (mapWidth - mapRadius) / 2)
                    {
                        verts[x * mapWidth + y] = new Vector3(verts[x * mapWidth + y].x, verts[x * mapWidth + y].y - distance / (coast*2), verts[x * mapWidth + y].z);
                        grassSpawns[x * mapWidth + y] = verts[x * mapWidth + y];
                    }
                }

                //Tõstab vertexid mis on alla vee leveli vee levelile ning värvib need siniseks
                if (verts[x * mapWidth + y].y < waterLevel)
                {
                    verts[x * mapWidth + y].y = waterLevel;
                    grassSpawns[x * mapWidth + y] = Vector3.zero;
                    colors[x * mapWidth + y] = Color.blue;
                }
            }
        }

        //Teeb vertexi kollaseks kui lähedal on vesi ning loob vee kallastele vee objekti
        for (int x = 3; x < mapWidth-3; x++)
        {
            for (int y = 3; y < mapHeigth-3; y++)
            {
                if (verts[x * mapWidth + y].y > waterLevel && WaterCloseBy(x, y, verts, mapWidth, waterLevel))
                {
                    colors[x * mapWidth + y] = Color.yellow;
                }
                else if(verts[x * mapWidth + y].y == waterLevel && LandCloseBy(x, y, verts, mapWidth, waterLevel))
                {
                    GetComponent<SpawnWater>().SpawnWaterObject(x,y, waterLevel, mapWidth, mapHeigth);
                }
            }
        }

        terrainMesh.vertices = verts;
        verts = null;
        terrainMesh.triangles = triangles;
        triangles = null;
        terrainMesh.colors32 = colors;
        colors = null;
        //terrainMesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh.Clear();
        GetComponent<MeshFilter>().mesh = terrainMesh;

        gameObject.AddComponent<MeshCollider>().sharedMesh = terrainMesh;
        GetComponent<GrassSpawnSet>().StartGen(grassSpawns, mapWidth, mapHeigth, waterLevel); //Alustab plant generationi

        AgrassSpawns = grassSpawns;
        AmapWidth = mapWidth;
        AanimalSpawnDist = animalSpawnDist;
        AwaterLevel = waterLevel;

        Invoke("SpawnAnimals", 1f);
        return;
    }

    //Checkib kas vett on lähedal

    bool WaterCloseBy(int x, int y, Vector3[] verts, int mapWidth, float waterLevel)
    {
        if (verts[(x + 1) * mapWidth + y].y <= waterLevel ||
            verts[(x + 2) * mapWidth + y].y <= waterLevel ||
            verts[(x - 1) * mapWidth + y].y <= waterLevel ||
            verts[(x - 2) * mapWidth + y].y <= waterLevel ||
            verts[x * mapWidth + (y + 1)].y <= waterLevel ||
            verts[x * mapWidth + (y + 2)].y <= waterLevel ||
            verts[x * mapWidth + (y + 3)].y <= waterLevel ||
            verts[x * mapWidth + (y - 1)].y <= waterLevel ||
            verts[x * mapWidth + (y - 2)].y <= waterLevel ||
            verts[x * mapWidth + (y - 3)].y <= waterLevel ||
            verts[(x + 1) * mapWidth + (y + 1)].y <= waterLevel ||
            verts[(x + 2) * mapWidth + (y + 1)].y <= waterLevel ||
            verts[(x + 1) * mapWidth + (y - 1)].y <= waterLevel ||
            verts[(x + 2) * mapWidth + (y - 1)].y <= waterLevel ||
            verts[(x - 1) * mapWidth + (y + 1)].y <= waterLevel ||
            verts[(x - 2) * mapWidth + (y + 1)].y <= waterLevel ||
            verts[(x - 1) * mapWidth + (y - 1)].y <= waterLevel ||
            verts[(x - 2) * mapWidth + (y - 1)].y <= waterLevel ||
            verts[(x + 1) * mapWidth + (y + 2)].y <= waterLevel ||
            verts[(x + 1) * mapWidth + (y - 2)].y <= waterLevel ||
            verts[(x - 1) * mapWidth + (y + 2)].y <= waterLevel ||
            verts[(x - 1) * mapWidth + (y - 2)].y <= waterLevel)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    bool LandCloseBy(int x, int y, Vector3[] verts, int mapWidth, float waterLevel)
    {
        if (verts[(x + 1) * mapWidth + y].y > waterLevel ||
            verts[(x - 1) * mapWidth + y].y > waterLevel ||
            verts[x * mapWidth + (y + 1)].y > waterLevel ||
            verts[x * mapWidth + (y - 1)].y > waterLevel)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void SpawnAnimals()
    {

        int randomY = Random.Range(50, AmapWidth - 50);
        int randomX = Random.Range(50, AmapWidth - 50);

        while (AgrassSpawns[randomX * AmapWidth + randomY].y <= AwaterLevel)
        {
            randomY = Random.Range(50, AmapWidth - 50);
            randomX = Random.Range(50, AmapWidth - 50);
        }

        for (int i = 0; i < 5; i++)
        {
            if (AgrassSpawns[randomX * AmapWidth + randomY].y > AwaterLevel)
            {
                Instantiate(Resources.Load<GameObject>("Models/Planteater"), GameObject.FindWithTag("Planteaters").transform).transform.position = AgrassSpawns[(randomX + i) * AmapWidth + (randomY + i)];
            }
        }

        int randomX2 = Random.Range(0, AanimalSpawnDist * 2 + 1) - AanimalSpawnDist;
        int randomY2 = Random.Range(0, AanimalSpawnDist * 2 + 1) - AanimalSpawnDist;
        while (AgrassSpawns[(randomX + randomX2) * AmapWidth + (randomY + randomY2)].y <= AwaterLevel)
        {
            randomX2 = Random.Range(0, AanimalSpawnDist * 2 + 1) - AanimalSpawnDist;
            randomY2 = Random.Range(0, AanimalSpawnDist * 2 + 1) - AanimalSpawnDist;
        }
        for (int i = 0; i < 5; i++)
        {
            if (AgrassSpawns[(randomX + randomX2 + i) * AmapWidth + (randomY + randomY2 + 1)].y > AwaterLevel)
            {
                Instantiate(Resources.Load<GameObject>("Models/Hunter"), GameObject.FindWithTag("Hunters").transform).transform.position = AgrassSpawns[(randomX + randomX2 + i) * AmapWidth + (randomY + randomY2 + i)];
            }
        }
        return;
    }
    public void ResetAnimal(GameObject animal)
    {
        int randomY = Random.Range(20, AmapWidth - 20);
        int randomX = Random.Range(20, AmapWidth - 20);

        while (AgrassSpawns[randomX * AmapWidth + randomY].y <= AwaterLevel)
        {
            randomY = Random.Range(20, AmapWidth - 20);
            randomX = Random.Range(20, AmapWidth - 20);
        }

        animal.transform.position = AgrassSpawns[(randomX) * AmapWidth + (randomY)];
    }
    /*public void ResetArea()
    {
        GameObject[] Hunters = GameObject.FindGameObjectsWithTag("AnimalHunter");
        GameObject[] Planteaters = GameObject.FindGameObjectsWithTag("AnimalPlanteater");
        GameObject[] Plants = GameObject.FindGameObjectsWithTag("PlantsObject");
        GameObject[] PlantScripts = GameObject.FindGameObjectsWithTag("PlantGrass");

        for (int i = 0; i < Hunters.Length; i++){
            Hunters[i].transform.position = new Vector3(999,0,999);
            Destroy(Hunters[i]);
        }
        for (int i = 0; i < Planteaters.Length; i++)
        {
            Planteaters[i].transform.position = new Vector3(999, 0, 999);
            Destroy(Planteaters[i]);
        }
        for (int i = 0; i < PlantScripts.Length; i++)
        {
            if (PlantScripts[i].GetComponent<Plant>()) {
                PlantScripts[i].GetComponent<Plant>().setBack();
            }
            if(PlantScripts[i].transform.parent == null)
            {
                Destroy(PlantScripts[i]);
            }
        }
        for (int i = 0; i < Plants.Length; i++)
        {
            Destroy(Plants[i]);
        }
        gameObject.GetComponent<GlobalNumbers>().plantCount = 0;
        GetComponent<GrassSpawnSet>().StartGen(grassSpawns, grassMapWidth, grassMapHeight, grassMapWaterLevel);
        SpawnAnimals();
    }*/
}
