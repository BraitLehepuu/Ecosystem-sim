//Ökosüsteemi loomine tehisintellekti abil
//@autor Ralf Brait Lehepuu
//
//Mõningane eeskuju https://github.com/Unity-Technologies/ml-agents/tree/release-0.14.1/docs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class PEaterAgent : Agent
{
    public float currentReward;
    public bool firstPeater = false;
    float lastBorn = 0;

    private PlanteaterMethods peaterScript;
    private PlanteaterVariables peaterVariables;

    int layerMask = 1 << 9;
    float waterLevel = 2.51f;

    private MakeMesh worldScript;

    void Start()
    {
        peaterScript = gameObject.GetComponent<PlanteaterMethods>();
        peaterVariables = gameObject.GetComponent<PlanteaterVariables>();
        worldScript = GameObject.FindWithTag("GenObject").GetComponent<MakeMesh>();
    }

    public override void AgentAction(float[] vectorAction)
    {
        bool giveBirth = true;
        /*if (vectorAction[2] > 0)
        {
            giveBirth = true;
        }*/

        //Actions
        Move(new Vector3(vectorAction[0], 0,vectorAction[1]));

        //Idle
        if (transform.position.y <= waterLevel)
        {
            //AddReward(-10f);
            Done();
        }
        peaterVariables.peaterHunger -= 1 + peaterVariables.peaterVision * 2;
        peaterVariables.peaterThirst -= 3 + peaterVariables.peaterSpeed * 2;
        if (peaterVariables.peaterHunger <= 0 || peaterVariables.peaterThirst <= 0)
        {
            Done();
        }

        Eat();
        Drink();
        Reproduce(giveBirth);

        AddReward(0.1f * (peaterVariables.peaterHunger / peaterVariables.peaterMaxHunger - 0.4f) * (peaterVariables.peaterThirst / peaterVariables.peaterMaxThirst/10));
    }

    public override void AgentReset()
    {
        if (firstPeater || Time.time < 5) {
            worldScript.ResetAnimal(gameObject);
            peaterScript.ResetVariables();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override float[] Heuristic()
    {
        var action = new float[5];
        if (Input.GetKey(KeyCode.RightArrow))
        {            
            action[0] = 1;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {            
            action[0] = 3;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {            
            action[1] = 1;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {            
            action[1] = 3;
        }
        action[2] = 0f;
        return action;
    }

    public override void CollectObservations()
    {
        //How hungry and what is max hunger
        AddVectorObs(peaterVariables.peaterHunger / peaterVariables.peaterMaxHunger);

        //How thirsty and what is max thirst
        AddVectorObs(peaterVariables.peaterThirst / peaterVariables.peaterMaxThirst);

        //How high
        AddVectorObs(transform.position.y / 10);

        //Self position
        //AddVectorObs(peaterScript.transform.position);

        //Closest hunter
        AddVectorObs(peaterScript.closestHunter.normalized);
        AddVectorObs(Vector3.Distance(peaterScript.closestHunter, transform.position) / peaterVariables.peaterVision * 10);

        //Closest peater
        AddVectorObs(peaterScript.closestPeater.normalized);
        AddVectorObs(Vector3.Distance(peaterScript.closestPeater, transform.position) / peaterVariables.peaterVision * 10);

        //Closest water
        AddVectorObs(peaterScript.closestWater.normalized);
        AddVectorObs(Vector3.Distance(peaterScript.closestWater, transform.position) / peaterVariables.peaterVision * 10);

        //Closest plant
        AddVectorObs(peaterScript.closestPlant.normalized);
        AddVectorObs(Vector3.Distance(peaterScript.closestPlant, transform.position) / peaterVariables.peaterVision * 10);
    }

    public void RewardAgent(float amount)
    {
        AddReward(amount);
    }
    public void EndRun()
    {
        Done();
    }
    public void Move(Vector3 direction)
    {
        if (direction == new Vector3(0, 0, 0))
        {
            return;
        }
        if (peaterVariables.peaterHunger > 4 + peaterVariables.peaterSpeed * 2 && peaterVariables.peaterThirst > 6 + peaterVariables.peaterSpeed * 2)
        {
            peaterVariables.peaterHunger -= 4 + peaterVariables.peaterSpeed * 2;
            peaterVariables.peaterThirst -= 6 + peaterVariables.peaterSpeed * 2;
            Vector3 movementVector = Vector3.ClampMagnitude(direction, 0.5f * peaterVariables.peaterSpeed);
            Vector3 posToGo = transform.position + movementVector;
            RaycastHit hit;
            Physics.Raycast(new Vector3(posToGo.x, 50, posToGo.z), new Vector3(0, -1, 0), out hit, Mathf.Infinity, layerMask);
            if (hit.point.y - 0.5f <= waterLevel)
            {
                return;
            }
            posToGo.y = hit.point.y - 0.5f;

            transform.position = posToGo;
        }
    }
    public void Eat()
    {
        Collider[] plants = Physics.OverlapSphere(transform.position, 2.5f, 1 << 11);
        if (plants.Length == 0)
        {
            return;
        }
        else
        {
            if (peaterVariables.peaterHunger < peaterVariables.peaterMaxHunger)
            {
                Plant plant = plants[Random.Range(0, plants.Length)].GetComponent<Plant>();
                if (plant)
                {
                    if (peaterVariables.peaterHunger + 400 >= peaterVariables.peaterMaxHunger)
                    {
                        peaterVariables.peaterHunger = peaterVariables.peaterMaxHunger;
                    }
                    else
                    {
                        peaterVariables.peaterHunger += 400;
                    }
                    plant.getEaten();
                    //GameObject.FindWithTag("GenObject").GetComponent<GlobalNumbers>().plantCount -= 1;
                    if (peaterVariables.peaterHunger != peaterVariables.peaterMaxHunger)
                    {
                       //AddReward(0.5f);
                    }
                }
            }
            else
            {
                return;
            }
        }
    }
    public void Drink()
    {
        Collider[] waterSpots = Physics.OverlapSphere(transform.position, 4f, 1 << 4);
        if (waterSpots.Length == 0)
        {
            return;
        }
        else if(peaterVariables.peaterThirst < peaterVariables.peaterMaxThirst - 2500)
        {
            //AddReward(1 * (peaterVariables.peaterMaxThirst/peaterVariables.peaterMaxThirst));
            peaterVariables.peaterThirst = peaterVariables.peaterMaxThirst;
        }
    }
    public void Reproduce(bool check)
    {
        if (!check)
        {
            return;
        }
        if (peaterVariables.peaterHunger > 4000 && peaterVariables.peaterThirst > 4000 && Time.time - lastBorn > 20)
        {
            lastBorn = Time.time;

            peaterVariables.peaterThirst -= 3000;
            peaterVariables.peaterHunger -= 3000;

            Debug.Log("Plant Eater Born");
            GameObject child = Instantiate(gameObject, GameObject.FindWithTag("Planteaters").transform);
            child.transform.position = transform.position;
            child.name = "Planteater";
            PlanteaterVariables childVarScript = child.GetComponent<PlanteaterVariables>();
            child.GetComponent<PEaterAgent>().firstPeater = false;

            childVarScript.peaterSpeed += (float)(Random.Range(0, 201) - 100) / 1000;
            childVarScript.peaterVision += (float)(Random.Range(0, 201) - 100) / 1000;
            childVarScript.peaterSmell += (float)(Random.Range(0, 201) - 100) / 1000;
            childVarScript.peaterHunger = 500;
            childVarScript.peaterThirst = 500;

            childVarScript.CheckVariables();
            AddReward(1f);
        }
    }
}