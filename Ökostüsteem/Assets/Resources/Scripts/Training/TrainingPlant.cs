using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingPlant : MonoBehaviour
{
    public int spot;

    public void getEaten()
    {
        transform.GetComponentInParent<PlaneTerrain>().takenSpots[spot] = false;
        Destroy(gameObject);
    }
}
