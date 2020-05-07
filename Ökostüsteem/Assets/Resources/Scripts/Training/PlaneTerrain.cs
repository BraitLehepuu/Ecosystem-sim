using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneTerrain : MonoBehaviour
{

    MeshFilter planeFilter;
    Mesh planeMesh;
    MeshCollider planeCollider;
    GameObject waterObject;
    bool mapReady;
    public bool[] takenSpots = new bool[50 * 50];

    void Start()
    {
        planeFilter = gameObject.GetComponent<MeshFilter>();
        planeMesh = new Mesh();
        planeCollider = gameObject.GetComponent<MeshCollider>();
        randomMesh();
    }

    private void FixedUpdate()
    {
        int plantCount = 50;

        if (mapReady)
        {
            while (GameObject.FindGameObjectsWithTag("PlantGrass").Length < plantCount)
            {
                int pos = Random.Range(0, 50*50 + 1);
                while (planeMesh.vertices[pos].y == 3 && takenSpots[pos] == false)
                {
                    if (pos == 50*50 - 1)
                    {
                        pos = 0;
                    }
                    else
                    {
                        pos += 1;
                    }

                }
                takenSpots[pos] = true;
                GameObject plant = Instantiate(Resources.Load<GameObject>("Models/PlantRedObject"));
                plant.transform.SetParent(transform);
                plant.AddComponent<TrainingPlant>().spot = pos;
                plant.transform.localScale = new Vector3(0.2f,0.2f,2);
                plant.transform.position = new Vector3(planeMesh.vertices[pos].x * 10, 5, planeMesh.vertices[pos].z * 10);
            }
        }
    }

    public void randomMesh()
    {
        int triangleCount = 0;

        Vector3[] verts = new Vector3[50 * 50];
        int[] triangles = new int[(50 * 50 - 2) * 3 * 2];
        int edgesCut = 5; // Random.Range(1, 4);


        for (int x = 0; x < 50; x++)
        {
            for (int y = 0; y < 50; y++)
            {
                if (x != 0)
                {
                    if (y != 0)
                    {
                        triangles[triangleCount] = x * 50 + y;
                        triangles[triangleCount + 1] = x * 50 + (y - 1);
                        triangles[triangleCount + 2] = (x - 1) * 50 + y;
                        triangleCount += 3;
                    }
                }
                if (x < 50 - 1)
                {
                    if (y != 50 - 1)
                    {
                        triangles[triangleCount] = x * 50 + y;
                        triangles[triangleCount + 1] = x * 50 + (y + 1);
                        triangles[triangleCount + 2] = (x + 1) * 50 + y;
                        triangleCount += 3;
                    }
                }
                verts[x * 50 + y] = new Vector3(x - 50 / 2, 5f, y - 50 / 2);
            }
        }

        for (int i = 0; i < edgesCut; i++)
        {
            for (int j = 0; j < 50; j++)
            {
                verts[i * 50 + j].y = 3;
            }
        }
        for (int i = 50-edgesCut; i < 50; i++)
        {
            for (int j = 0; j < 50; j++)
            {
                verts[i * 50 + j].y = 3;
            }
        }
        for (int i = edgesCut; i < 50-edgesCut; i++)
        {
            for (int j = 0; j < edgesCut; j++)
            {
                verts[i * 50 + j].y = 3;
            }
            for (int j = 50 - edgesCut; j < 50; j++)
            {
                verts[i * 50 + j].y = 3;
            }
        }

        for (int x = 0; x < 2; x++)
        {
            int[] startPoint = new int[2];
            int size = Random.Range(10, 16);
            startPoint[0] = Random.Range(edgesCut, 50 - edgesCut - size - 1); startPoint[1] = Random.Range(edgesCut, 50 - edgesCut - 5-size-1);
            for(int i = startPoint[0]; i < startPoint[0]+size; i++)
            {
                for (int j = startPoint[1]; j < startPoint[1]+size; j++)
                {
                    if (i == startPoint[0] || i == startPoint[0] + size - 1 || j == startPoint[1] || j == startPoint[1] + size - 1) {
                        waterObject = new GameObject("Water object");
                        waterObject.transform.position = new Vector3((i - 25) * 10, 4, (j - 25) * 10);
                        waterObject.transform.localScale = new Vector3(10, 10, 10);
                        BoxCollider waterCollider = waterObject.AddComponent<BoxCollider>();
                        waterCollider.isTrigger = true;
                        waterObject.transform.SetParent(transform);
                        waterObject.transform.tag = "Water";
                        waterObject.layer = 4;
                    }
                    verts[i * 50 + j].y = 3;
                }
            }
        }

        planeMesh.vertices = verts;
        planeMesh.triangles = triangles;
        planeFilter.mesh.Clear();
        planeFilter.mesh = planeMesh;
        planeCollider.sharedMesh = planeMesh;

        mapReady = true;
    }
}
