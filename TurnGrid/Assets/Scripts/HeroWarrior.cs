using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroWarrior : SharedStats
{
    private bool showMyPointer = false;
    
    void Start()
    {
        CalculateStats(UnitLevel, 10, 3, 1, 2, 1f);
    }


    void Update()
    {
        if (myTurn)
        {
            if (!showMyPointer)
            {
                showMyPointer = true;
                EnablePointer(gameObject, showMyPointer);
            }
        }

        if(!myTurn && showMyPointer)
        {
            showMyPointer = false;
            EnablePointer(gameObject, showMyPointer);
        }
    }
}
