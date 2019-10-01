using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeMesh : MonoBehaviour
{
    public void CreateMesh(int mapWidth, int mapHeigth, float[,] noiseMap, float heigths, int coast, float waterLevel, int mapRadius)
    {
        int triangleCount = 0;

        Mesh terrainMesh = new Mesh();

        Vector3[] verts = new Vector3[mapWidth * mapHeigth];
        int[] triangles = new int[(mapWidth * mapHeigth -2) * 3*2];
        Vector2[] uvs = new Vector2[verts.Length];
        Color32[] colors = new Color32[verts.Length];

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
                colors[x * mapWidth + y] = Color.green;
                if (Vector3.Distance(new Vector3(verts[(mapWidth+1)/2 * mapWidth + (mapHeigth+1)/2].x, 0, verts[(mapWidth + 1) / 2 * mapWidth + (mapHeigth + 1) / 2].z), new Vector3(x - mapWidth / 2, 0, y - mapWidth / 2)) > (mapWidth-mapRadius)/2)
                {
                    float distance = Vector3.Distance(new Vector3(verts[(mapWidth + 1) / 2 * mapWidth + (mapHeigth + 1) / 2].x, 0, verts[(mapWidth + 1) / 2 * mapWidth + (mapHeigth + 1) / 2].z), new Vector3(x - mapWidth / 2, 0, y - mapWidth / 2)) - (mapWidth - mapRadius) / 2;
                    verts[x * mapWidth + y] = new Vector3(verts[x * mapWidth + y].x, verts[x * mapWidth + y].y - distance/coast, verts[x * mapWidth + y].z);
                    if (Vector3.Distance(new Vector3(verts[(mapWidth + 1) / 2 * mapWidth + (mapHeigth + 1) / 2].x, 0, verts[(mapWidth + 1) / 2 * mapWidth + (mapHeigth + 1) / 2].z), new Vector3(x - mapWidth / 2, 0, y - mapWidth / 2)) > (mapWidth - mapRadius) / 2)
                    {
                        verts[x * mapWidth + y] = new Vector3(verts[x * mapWidth + y].x, verts[x * mapWidth + y].y - distance / (coast*2), verts[x * mapWidth + y].z);
                    }
                }
                if (verts[x * mapWidth + y].y < waterLevel)
                {
                    verts[x * mapWidth + y].y = waterLevel;
                    colors[x * mapWidth + y] = Color.blue;
                }
            }
        }

        for (int x = 3; x < mapWidth-3; x++)
        {
            for (int y = 3; y < mapHeigth-3; y++)
            {
                if (WaterCloseBy(x, y, verts, mapWidth, waterLevel) && verts[x * mapWidth + y].y > waterLevel)
                {
                    colors[x * mapWidth + y] = Color.yellow;
                }
            }
        }

        for (int i = 0; i < verts.Length; i++)
        {
            uvs[i] = new Vector2(verts[i].z, verts[i].x);
        }

        terrainMesh.vertices = verts;
        terrainMesh.triangles = triangles;
        terrainMesh.uv = uvs;
        terrainMesh.colors32 = colors;
        terrainMesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh.Clear();
        GetComponent<MeshFilter>().mesh = terrainMesh;

        /*Vector3[] normals = GetComponent<MeshFilter>().mesh.normals;
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = new Vector3(normals[i].x * -1, normals[i].y * -1, normals[i].z * -1);
        }

        GetComponent<MeshFilter>().mesh.normals = normals;*/
    }

    bool WaterCloseBy(int x, int y, Vector3[] verts, int mapWidth, float waterLevel)
    {
        if (verts[(x + 1) * mapWidth + y].y <= waterLevel ||
            verts[(x + 2) * mapWidth + y].y <= waterLevel ||
            verts[(x - 1) * mapWidth + y].y <= waterLevel ||
            verts[(x - 2) * mapWidth + y].y <= waterLevel ||
            verts[x * mapWidth + y + 1].y <= waterLevel ||
            verts[x * mapWidth + y + 2].y <= waterLevel ||
            verts[x * mapWidth + y + 3].y <= waterLevel ||
            verts[x * mapWidth + y - 1].y <= waterLevel ||
            verts[x * mapWidth + y - 2].y <= waterLevel ||
            verts[x * mapWidth + y - 3].y <= waterLevel ||
            verts[(x + 1) * mapWidth + y + 1].y <= waterLevel ||
            verts[(x + 2) * mapWidth + y + 1].y <= waterLevel ||
            verts[(x + 1) * mapWidth + y - 1].y <= waterLevel ||
            verts[(x + 2) * mapWidth + y - 1].y <= waterLevel ||
            verts[(x - 1) * mapWidth + y + 1].y <= waterLevel ||
            verts[(x - 2) * mapWidth + y + 1].y <= waterLevel ||
            verts[(x - 1) * mapWidth + y - 1].y <= waterLevel ||
            verts[(x - 2) * mapWidth + y - 1].y <= waterLevel ||
            verts[(x + 1) * mapWidth + y + 2].y <= waterLevel ||
            verts[(x + 1) * mapWidth + y - 2].y <= waterLevel ||
            verts[(x - 1) * mapWidth + y + 2].y <= waterLevel ||
            verts[(x - 1) * mapWidth + y - 2].y <= waterLevel)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
