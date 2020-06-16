using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : SharedStats
{
    
    void Start()
    {
        CalculateStats(UnitLevel,7,3,1,2,.5f);
    }

    void Update()
    {
        if (myTurn)
        {
            int target = Random.Range(0, myEnemies.Length);
            BasicAttacks(myEnemies[target], false);
        }
    }


}
