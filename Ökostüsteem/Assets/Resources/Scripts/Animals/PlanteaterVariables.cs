//Ökosüsteemi loomine tehisintellekti abil
//@autor Ralf Brait Lehepuu
//
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanteaterVariables : MonoBehaviour
{
    public float peaterSpeed;
    public float peaterMaxHunger;
    public float peaterHunger;
    public float peaterMaxThirst;
    public float peaterThirst;
    public float peaterVision;
    public float peaterSmell;

    private float peaterEnergyRegenVariety = 1;
    private float peaterSpeedVariety = 1;
    private float peaterVisionVariety = 1;
    private float peaterSmellVariety = 1;

    public void CheckVariables()
    {
        if (peaterSpeed > 1 + peaterSpeedVariety)
        {
            peaterSpeed = 1 + peaterSpeedVariety;
        }
        if (peaterSpeed < 1 - peaterSpeedVariety)
        {
            peaterSpeed = 1 - peaterSpeedVariety;
        }

        if (peaterVision > 1 + peaterVisionVariety)
        {
            peaterVision = 1 + peaterVisionVariety;
        }
        if (peaterVision < 1 - peaterVisionVariety)
        {
            peaterVision = 1 - peaterVisionVariety;
        }

        if (peaterSmell > 1 + peaterSmellVariety)
        {
            peaterSmell = 1 + peaterSmellVariety;
        }
        if (peaterSmell < 1 - peaterSmellVariety)
        {
            peaterSmell = 1 - peaterSmellVariety;
        }
    }
}
