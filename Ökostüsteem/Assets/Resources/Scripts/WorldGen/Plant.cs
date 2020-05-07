//Ökosüsteemi loomine tehisintellekti abil
//@autor Ralf Brait Lehepuu
//
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{

    public int ID;
    public GrassSpawnSet parentScript;

    private void FixedUpdate()
    {
        transform.GetChild(0).transform.GetChild(0).transform.position = new Vector3(transform.position.x, 3, transform.position.z);
    }

    void Start()
    {
        GameObject model = Resources.Load<GameObject>("Models/PlantRedObject");
        model.transform.position = new Vector3(Random.Range(-0.4f, 0.4f), Random.Range(-0.5f, -0.3f), Random.Range(-0.4f, 0.4f));
        if (transform.childCount == 1) {
            Destroy(transform.GetChild(0).gameObject);
        }
        Instantiate(Resources.Load<GameObject>("Models/PlantRedObject"), transform);
    }

    public void getEaten()
    {
        parentScript = GameObject.FindGameObjectWithTag("GenObject").GetComponent<GrassSpawnSet>();
        if (parentScript.grassMap[ID].y < -10 && parentScript)
        {
            parentScript.grassMap[ID].y += 50;
        }
        GameObject.FindWithTag("GenObject").GetComponent<GlobalNumbers>().plantCount -= 1;
        Destroy(gameObject);
    }

    public void setBack()
    {
        if (parentScript.grassMap[ID].y < -10 && parentScript)
        {
            parentScript.grassMap[ID].y += 50;
        }
    }

}
