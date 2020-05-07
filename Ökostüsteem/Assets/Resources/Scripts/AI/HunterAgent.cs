//Ökosüsteemi loomine tehisintellekti abil
//@autor Ralf Brait Lehepuu
//
//Mõningane eeskuju https://github.com/Unity-Technologies/ml-agents/tree/release-0.14.1/docs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class HunterAgent : Agent
{
    public float currentReward;
    public bool firstHunter = false;
    float lastBorn = 0;

    int layerMask = 1 << 9;
    float waterLevel = 2.51f;

    private HunterMethods hunterScript;
    private HunterVariables hunterVariables;
    private MakeMesh worldScript;


    private void Start()
    {
        hunterScript = gameObject.GetComponent<HunterMethods>();
        hunterVariables = gameObject.GetComponent<HunterVariables>();
        worldScript = GameObject.FindWithTag("GenObject").GetComponent<MakeMesh>();

    }

    public override void AgentReset()
    {
        if (firstHunter || Time.time < 5) {
            worldScript.ResetAnimal(gameObject);
            hunterScript.ResetVariables();
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

    public override void AgentAction(float[] vectorAction)
    {

        bool giveBirth = true;
        /*if (vectorAction[2] > 0)
        {
            giveBirth = true;
        }*/

        //Actions

        Move(new Vector3(vectorAction[0], 0, vectorAction[1]));

        //Idle
        if (transform.position.y <= waterLevel)
        {
            //AddReward(-10f);
            Done();
        }
        hunterVariables.hunterHunger -= 1 + hunterVariables.hunterVision * 2;
        hunterVariables.hunterThirst -= 3 + hunterVariables.hunterVision * 2;
        if (hunterVariables.hunterHunger <= 0 || hunterVariables.hunterThirst <= 0)
        {
            Done();
        }

        Eat();
        EatAnimal();
        Drink();
        Reproduce(giveBirth);
        

        AddReward(0.1f * (hunterVariables.hunterHunger/hunterVariables.hunterMaxHunger - 0.4f) * (hunterVariables.hunterThirst/hunterVariables.hunterMaxThirst/10));
    }

    public override void CollectObservations()
    {
        //How hungry and what is max hunger
        AddVectorObs(hunterVariables.hunterHunger / hunterVariables.hunterMaxHunger);

        //How thirsty and what is max thirst
        AddVectorObs(hunterVariables.hunterThirst / hunterVariables.hunterMaxThirst);

        //How high
        AddVectorObs(transform.position.y / 10);

        //Hunter position
        //AddVectorObs(hunterScript.transform.position);

        //Closest hunter
        AddVectorObs(hunterScript.closestHunter.normalized);
        AddVectorObs(Vector3.Distance(hunterScript.closestHunter, transform.position) / hunterVariables.hunterVision * 10);

        //Closest peater
        AddVectorObs(hunterScript.closestPeater.normalized);
        AddVectorObs(Vector3.Distance(hunterScript.closestPeater, transform.position) / hunterVariables.hunterVision * 10);

        //Closest water
        AddVectorObs(hunterScript.closestWater.normalized);
        AddVectorObs(Vector3.Distance(hunterScript.closestWater, transform.position) / hunterVariables.hunterVision * 10);

        //Closest plant
        AddVectorObs(hunterScript.closestPlant.normalized);
        AddVectorObs(Vector3.Distance(hunterScript.closestPlant, transform.position) / hunterVariables.hunterVision * 10);
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
        if (hunterVariables.hunterHunger > 4 + hunterVariables.hunterSpeed * 2 && hunterVariables.hunterThirst > 6 + hunterVariables.hunterSpeed * 2)
        {
            hunterVariables.hunterHunger -= 4 + hunterVariables.hunterSpeed * 2;
            hunterVariables.hunterThirst -= 6 + hunterVariables.hunterSpeed * 2;
            Vector3 movementVector = Vector3.ClampMagnitude(direction, 0.5f * hunterVariables.hunterSpeed);
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
            if (hunterVariables.hunterHunger < hunterVariables.hunterMaxHunger)
            {
                Plant plant = plants[Random.Range(0, plants.Length)].GetComponent<Plant>();
                if (plant)
                { 
                    if (hunterVariables.hunterHunger + 200 >= hunterVariables.hunterMaxHunger)
                    {
                        hunterVariables.hunterHunger = hunterVariables.hunterMaxHunger;
                    }
                    else
                    {
                        hunterVariables.hunterHunger += 200;
                    }
                    plant.getEaten();
                    //GameObject.FindWithTag("GenObject").GetComponent<GlobalNumbers>().plantCount -= 1;
                    if (hunterVariables.hunterHunger != hunterVariables.hunterMaxHunger)
                    {
                    //AddReward(0.1f);
                    }
                }
            }
            else
            {
                return;
            }

        }
    }
    public void EatAnimal()
    {
        Collider[] animals = Physics.OverlapSphere(transform.position, 2.5f, 1 << 12);
        if (animals.Length == 0)
        {
            return;
        }
        else
        {
            if (hunterVariables.hunterHunger < hunterVariables.hunterMaxHunger)
            {
                if (hunterVariables.hunterHunger + 500 >= hunterVariables.hunterMaxHunger)
                {
                    hunterVariables.hunterHunger = hunterVariables.hunterMaxHunger;
                }
                else
                {
                    hunterVariables.hunterHunger += 500;
                }
                animals[Random.Range(0, animals.Length)].GetComponent<PlanteaterMethods>().agentScript.AddReward(-0.5f);
                animals[Random.Range(0, animals.Length)].GetComponent<PlanteaterMethods>().Die();
                AddReward(2f);
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
        else if(hunterVariables.hunterThirst < hunterVariables.hunterMaxThirst - 2500)
        {
            //AddReward(1 * (hunterVariables.hunterThirst/hunterVariables.hunterMaxThirst));
            hunterVariables.hunterThirst = hunterVariables.hunterMaxThirst;
        }
    }
    public void Reproduce(bool check)
    {
        if (!check)
        {
            return;
        }
        if (hunterVariables.hunterHunger > 4000 && hunterVariables.hunterThirst > 4000 && Time.time - lastBorn > 20)
        {
            lastBorn = Time.time;

            hunterVariables.hunterHunger -= 3500;
            hunterVariables.hunterThirst -= 3500;

            Debug.Log("Hunter Born");
            GameObject child = Instantiate(gameObject, GameObject.FindWithTag("Hunters").transform);
            child.transform.position = transform.position;
            child.name = "Hunter";
            HunterVariables childVarScript = child.GetComponent<HunterVariables>();
            child.GetComponent<HunterAgent>().firstHunter = false;

            childVarScript.hunterSpeed += (float)(Random.Range(0, 201) - 100) / 1000;
            childVarScript.hunterVision += (float)(Random.Range(0, 201) - 100) / 1000;
            childVarScript.hunterSmell += (float)(Random.Range(0, 201) - 100) / 1000;
            childVarScript.hunterHunger = 500;
            childVarScript.hunterThirst = 500;

            childVarScript.CheckVariables();
            AddReward(1f);
        }
    }
}
