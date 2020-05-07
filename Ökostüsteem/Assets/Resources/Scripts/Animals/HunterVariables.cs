//Ökosüsteemi loomine tehisintellekti abil
//@autor Ralf Brait Lehepuu
//
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterVariables : MonoBehaviour
{
    public float hunterSpeed;
    public float hunterMaxHunger;
    public float hunterHunger;
    public float hunterMaxThirst;
    public float hunterThirst;
    public float hunterVision;
    public float hunterSmell;

    private float hunterEnergyRegenVariety = 1;
    private float hunterSpeedVariety = 1;
    private float hunterVisionVariety = 1;
    private float hunterSmellVariety = 1;

    public void CheckVariables()
    {

        if (hunterSpeed > 1 + hunterSpeedVariety)
        {   
            hunterSpeed = 1 + hunterSpeedVariety;
        }   
        if (hunterSpeed < 1 - hunterSpeedVariety)
        {   
            hunterSpeed = 1 - hunterSpeedVariety;
        }

        if (hunterVision > 1 + hunterVisionVariety)
        {
            hunterVision = 1 + hunterVisionVariety;
        }
        if (hunterVision < 1 - hunterVisionVariety)
        {
            hunterVision = 1 - hunterVisionVariety;
        }

        if (hunterSmell > 1 + hunterSmellVariety)
        {
            hunterSmell = 1 + hunterSmellVariety;
        }
        if (hunterSmell < 1 - hunterSmellVariety)
        {
            hunterSmell = 1 - hunterSmellVariety;
        }
    }
}
