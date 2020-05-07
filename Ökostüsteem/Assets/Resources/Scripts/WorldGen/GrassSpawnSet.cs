//Ökosüsteemi loomine tehisintellekti abil
//@autor Ralf Brait Lehepuu
//
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Spawnib randomilt patche plantidega

public class GrassSpawnSet : MonoBehaviour
{

    GameObject plant;
    bool spawning;
    public Vector3[] grassMap;
    int mapWidth;
    int mapHeigth;
    public int plantCount;
    int[] taken;
    float waterLevel;
    Vector3 currentSpawn;


    public void StartGen(Vector3[] spawnMap, int mapX, int mapY, float WaterLevel)
    {
        plant = new GameObject("plant1");
        plant.transform.tag = "PlantGrass";
        plant.AddComponent<BoxCollider>();
        plant.AddComponent<Plant>();
        plant.layer = 11;
        plant.transform.position = new Vector3(0,-50,0);
        grassMap = spawnMap;
        mapHeigth = mapY;
        mapWidth = mapX;
        waterLevel = WaterLevel;

        while (gameObject.GetComponent<GlobalNumbers>().plantCount <= 600)
        {
            SpawnPoint();
        }
    }

    private void FixedUpdate()
    {
        if (!spawning && gameObject.GetComponent<GlobalNumbers>().plantCount <= 600)
        {
            spawning = true;
            Invoke("SpawnPoint", Random.Range(1,2));
        }
    }

    public void SpawnPoint()
    {
        for (int l = 0; l < 50; l++) {
            int randomX = Random.Range(7, mapWidth - 7);
            int randomY = Random.Range(7, mapHeigth - 7);
            currentSpawn = grassMap[randomX * randomY];
            while (grassMap[randomX * mapWidth + randomY].y <= waterLevel)
            {
                randomX = Random.Range(7, mapWidth - 7);
                randomY = Random.Range(7, mapHeigth - 7);
            }
            currentSpawn = grassMap[randomX * randomY];

            SpawnPatch(randomX, randomY);
        }
    }

    void SpawnPatch(int x, int y)
    {
        GameObject parentSetTemplate = new GameObject("PlantSet");
        GameObject parentSet = Instantiate(parentSetTemplate);
        parentSet.transform.tag = "PlantsObject";
        parentSet.transform.parent = GameObject.FindWithTag("GenObject").transform;
        Destroy(parentSetTemplate);

        taken = new int[plantCount];

        for (int i = 0; i < plantCount; i++)
        {
            int spotID = PickSpot(x, y);
            if (spotID <= mapHeigth*mapWidth && grassMap[spotID].y >= waterLevel)
            {
                plant.GetComponent<Plant>().ID = spotID;
                plant.GetComponent<Plant>().parentScript = GetComponent<GrassSpawnSet>();
                GameObject currentPlant = Instantiate(plant, grassMap[spotID], new Quaternion(0,0,0,0));
                currentPlant.transform.SetParent(parentSet.transform);
                gameObject.GetComponent<GlobalNumbers>().plantCount += 1;
                grassMap[spotID].y -= 50;
            }
        }
        if(parentSet.transform.childCount == 0)
        {
            Destroy(parentSet);
        }

        spawning = false;
    }

    int PickSpot(int x, int y)
    {
        int spotID = Random.Range(1, 22);
        while (ArrayContains(taken, spotID)) {
            if (spotID == 21)
            {
                spotID = 1;
            }
            else
            {
                spotID++;
            }
        }
        for (int h = 0; h < taken.Length; h++)
        {
            if (taken[h] == 0)
            {
                taken[h] = spotID;
                break;
            }
        }

        switch (spotID)
        {
            case 1:
                return (x + 2) * mapWidth + (y - 1);
            case 2:
                return (x + 2) * mapWidth + y;
            case 3:
                return (x + 2) * mapWidth + (y + 1);
            case 4:
                return (x + 1) * mapWidth + (y - 2);
            case 5:
                return (x + 1) * mapWidth + (y - 1);
            case 6:
                return (x + 1) * mapWidth + y;
            case 7:
                return (x + 1) * mapWidth + (y + 1);
            case 8:
                return (x + 1) * mapWidth + (y + 2);
            case 9:
                return x * mapWidth + (y - 2);
            case 10:
                return x * mapWidth + (y - 1);
            case 11:
                return x * mapWidth + y;
            case 12:
                return x * mapWidth + (y + 1);
            case 13:
                return x * mapWidth + (y + 2);
            case 14:
                return (x - 1) * mapWidth + (y - 2);
            case 15:
                return (x - 1) * mapWidth + (y - 1);
            case 16:
                return (x - 1) * mapWidth + y;
            case 17:
                return (x - 1) * mapWidth + (y + 1);
            case 18:
                return (x - 1) * mapWidth + (y + 2);
            case 19:
                return (x - 2) * mapWidth + (y - 1);
            case 20:
                return (x - 2) * mapWidth + y;
            case 21:
                return (x - 2) * mapWidth + (y + 1);
        }
        return -1;
    }
    bool ArrayContains(int[] array, int item)
    {
        bool contains = false;

        for (int j = 0; j < array.Length; j++)
        {
            if (array[j] == item)
            {
                contains = true;
                break;
            }
        }

        return contains;
    }
}
