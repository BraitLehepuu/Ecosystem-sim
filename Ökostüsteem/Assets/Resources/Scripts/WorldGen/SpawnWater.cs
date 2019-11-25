using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnWater : MonoBehaviour
{

    GameObject waterObject;

    public void SpawnWaterObject(int x, int y, float waterLevel, int mapWidth, int mapHeigth)
    {

        waterObject = new GameObject("Water object");
        waterObject.transform.position = new Vector3(x - mapWidth / 2, waterLevel + 0.5f, y - mapHeigth / 2);
        BoxCollider waterCollider = waterObject.AddComponent<BoxCollider>();
        waterCollider.isTrigger = true;
        waterObject.transform.SetParent(GameObject.FindWithTag("WaterObjects").transform);
        waterObject.transform.tag = "Water";

    }
}
