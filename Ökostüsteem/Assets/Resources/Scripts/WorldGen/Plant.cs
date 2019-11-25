using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{

    public int ID;

    void Start()
    {
        GameObject model = Resources.Load<GameObject>("Models/PlantRedObject");
        model.transform.position = new Vector3(Random.Range(-0.4f, 0.4f), Random.Range(-0.5f, -0.3f), Random.Range(-0.4f, 0.4f));
        if (transform.childCount == 1) {
            Destroy(transform.GetChild(0).gameObject);
        }
        Instantiate(Resources.Load<GameObject>("Models/PlantRedObject"), transform);
    }

}
