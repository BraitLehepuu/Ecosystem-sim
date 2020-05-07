//Ökosüsteemi loomine tehisintellekti abil
//@autor Ralf Brait Lehepuu
//
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanteaterMethods : MonoBehaviour
{

    int layerMask = 1 << 9;

    float lastEaten = 0;
    float lastDrank = 0;
    float lastMoved = 0;
    float lastIdle = 0;
    readonly float idleTime = 0.2f;
    float waterLevel = 2.51f;

    public bool dead = false;

    private int hunterLayer;
    private int peaterLayer;
    private int plantLayer;
    private int waterLayer;

    public Collider[] hunterColliders;
    public float hunterPosition;
    public Vector3 closestHunter;
    public Collider[] waterColliders;
    public float waterPosition;
    public Vector3 closestWater;
    public Collider[] plantColliders;
    public float plantPosition;
    public Vector3 closestPlant;
    public Collider[] peaterColliders;
    public float peaterPosition;
    public Vector3 closestPeater;

    PlanteaterVariables varScript;
    public PEaterAgent agentScript;

    private void Start()
    {
        varScript = GetComponent<PlanteaterVariables>();
        agentScript = GetComponent<PEaterAgent>();

        hunterLayer = 1 << 10;
        peaterLayer = 1 << 12;
        plantLayer = 1 << 11;
        waterLayer = 1 << 4;
    }

    private void FixedUpdate()
    {

        transform.GetChild(0).transform.position = new Vector3(transform.position.x, 3, transform.position.z);

        hunterColliders = Physics.OverlapSphere(transform.position, varScript.peaterVision * 20, hunterLayer);
        hunterPosition = 999;
        closestHunter = new Vector3(0, 0, 0);
        for (int i = 0; i < hunterColliders.Length; i++)
        {
            if (Vector3.Distance(transform.position, hunterColliders[i].GetComponent<Transform>().position) < hunterPosition)
            {
                hunterPosition = Vector3.Distance(transform.position, hunterColliders[i].GetComponent<Transform>().position);
                closestHunter = new Vector3(hunterColliders[i].GetComponent<Transform>().position.x - transform.position.x, 0, hunterColliders[i].GetComponent<Transform>().position.z - transform.position.z).normalized;
            }
        }

        peaterColliders = Physics.OverlapSphere(transform.position, varScript.peaterVision * 20, peaterLayer);
        peaterPosition = 999;
        closestPeater = new Vector3(0, 0, 0);
        for (int i = 0; i < peaterColliders.Length; i++)
        {
            if (Vector3.Distance(transform.position, peaterColliders[i].GetComponent<Transform>().position) < peaterPosition && peaterColliders[i].transform != transform)
            {
                peaterPosition = Vector3.Distance(transform.position, peaterColliders[i].GetComponent<Transform>().position);
                closestPeater = new Vector3(peaterColliders[i].GetComponent<Transform>().position.x - transform.position.x, 0, peaterColliders[i].GetComponent<Transform>().position.z - transform.position.z).normalized;
            }
        }

        waterColliders = Physics.OverlapSphere(transform.position, varScript.peaterVision * 20, waterLayer);
        waterPosition = 999;
        closestWater = new Vector3(0, 0, 0);
        for (int i = 0; i < waterColliders.Length; i++)
        {
            if (Vector3.Distance(transform.position, waterColliders[i].GetComponent<Transform>().position) < waterPosition)
            {
                waterPosition = Vector3.Distance(transform.position, waterColliders[i].GetComponent<Transform>().position);
                closestWater = new Vector3(waterColliders[i].GetComponent<Transform>().position.x - transform.position.x, 0, waterColliders[i].GetComponent<Transform>().position.z - transform.position.z).normalized;
            }
        }

        plantColliders = Physics.OverlapSphere(transform.position, varScript.peaterVision * 20, plantLayer);
        plantPosition = 999;
        closestPlant = new Vector3(0, 0, 0);
        for (int i = 0; i < plantColliders.Length; i++)
        {
            if (Vector3.Distance(transform.position, plantColliders[i].GetComponent<Transform>().position) < plantPosition)
            {
                plantPosition = Vector3.Distance(transform.position, plantColliders[i].GetComponent<Transform>().position);
                closestPlant = new Vector3(plantColliders[i].GetComponent<Transform>().position.x - transform.position.x, 0, plantColliders[i].GetComponent<Transform>().position.z - transform.position.z).normalized;
            }
        }
    }
    public void Die()
    {
        if (dead)
        {
            return;
        }
        dead = true;
        //gameObject.GetComponent<MeshRenderer>().enabled = false;
        //gameObject.GetComponent<SphereCollider>().enabled = false;
        //agentScript.SetReward(0f);
        agentScript.EndRun();
        //Invoke("AfterDestroy", 0.5f);
    }
    public void ResetAgent()
    {
        dead = false;
        gameObject.GetComponent<MeshRenderer>().enabled = true;
        gameObject.GetComponent<SphereCollider>().enabled = true;
    }
    void AfterDestroy()
    {
        Destroy(gameObject);
    }
    public void ResetVariables()
    {
        dead = false;
        varScript.peaterHunger = 2500;
        varScript.peaterThirst = 2500;
    }
}
