using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TurnManager : MonoBehaviour
{
    public GameObject[] UnitsInGame;                                                                                           //Used for All units (ai & player) in level
    public int UnitIndex;                                                                                                      //Used as an index number used as the index of UnitsInGame
    public bool pCanAttack, pHasTraget;

    private GameObject[] pUnits;                                                                                               //Used for Player units
    private GameObject[] eUnits;                                                                                               //Used for Enemy units
    private GameObject target;                                                                                                 //Target is used in basic attacks as the unit targeted
    private GameObject[] targets;                                                                                              //Targets is used in abiliteis or aoe attacks as units targeted by set attack or ability
    private Camera cam;                                                                                                        //Used to store Main camera component
    private bool turnsSet;                                                                                                     //Used to check if the game has been set
                                                                                                                               

    void Start()
    {
        pUnits = GameObject.FindGameObjectsWithTag("Player");                                                                  //Getting all of player's units
        eUnits = GameObject.FindGameObjectsWithTag("Unit");                                                                    //Getting all the enemy units
        AssignEnemies(pUnits, eUnits);                                                                                         //Assigning the eUnits as enemy units to player units
        AssignEnemies(eUnits, pUnits);                                                                                         //Assigning the pUnits as enemy units to enemy units
        UnitsInGame = pUnits.Concat(eUnits).ToArray();                                                                         //Units in game is the combination of pUnit & eUnits
        turnsSet = false;                                                                                                      //Turn hasn't been set
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();                                           //Setting up cam as Main camera

        ArrangeTurns();                                                                                                        //Arrange units' turn order 
    }

    void Update()
    {
        TurnQueue();                                                                                                           //
        if (pCanAttack)                                                                                                        //
        {
            HeroAttack();
            print("player is attacking");
        }
        if (pHasTraget)
            UnitsInGame[UnitIndex].GetComponent<SharedStats>().BasicAttacks(target, true);
    }

    //Arrange the turn order of all the units in level
    private void ArrangeTurns()
    {
        while (!turnsSet)                                                                                                      //while the turns havn't been set
        {
            bool bSwitch = false;                                                                                              //set a bool to check if a switch in the bubble sort has happend
            for (int i = 0; i < UnitsInGame.Length - 1; i++)                                                                   //Go throught each unit -1 becasue inside the loop we check current unit and next unit 
            {
                if (UnitsInGame[i].GetComponent<SharedStats>().Speed < UnitsInGame[i + 1].GetComponent<SharedStats>().Speed)   //Compare speed of current unit to that of next unit
                {
                    GameObject temp = UnitsInGame[i];                                                                          //temp temporarily stores current unit
                    UnitsInGame[i] = UnitsInGame[i + 1];                                                                       //Current unit in array is now next unit
                    UnitsInGame[i + 1] = temp;                                                                                 //Next unit in array is now current unit 
                    bSwitch = true;                                                                                            //A switch has occured in the array a unit got bubbled up by the bubble sort
                }
            }
            if (!bSwitch)                                                                                                      //If a switch hasn't occured then bubble sort is finished
            {
                turnsSet = true;                                                                                               //The turn order has been set
                UnitIndex = 0;                                                                                                 //Resetting the index of UnitsInGame to 0
                UnitsInGame[UnitIndex].GetComponent<SharedStats>().myTurn = true;                                              //Tells the fastes unit in UnitsInGame it's its turn
                int i = 1;                                                                                                     //i is used to set numInQ starting with 1
                foreach (GameObject Unit in UnitsInGame)
                {
                    Unit.GetComponent<SharedStats>().numInQ = i;                                                               //Set the numInQ for each unit in UnitsInGame
                    //print(Unit.name + " is "+i+" in queue.");
                    i++;
                }
            }
        }
    }


    private void TurnQueue()
    {
        //print(UnitsInGame[UnitIndex].name + "'s turn");
        if (UnitsInGame[UnitIndex].GetComponent<SharedStats>().myTurn == false)
        {
            UnitIndex++;
            if (UnitIndex > UnitsInGame.Length - 1)
            {
                UnitIndex = 0;
            }
            UnitsInGame[UnitIndex].GetComponent<SharedStats>().myTurn = true;
            //if (UnitsInGame[UnitIndex].tag == "Player")
            //{
                
            //}
        }
    } 

    private void AssignEnemies(GameObject[] myTeam, GameObject[] enemyTeam)
    {
        foreach (GameObject unit in myTeam)
        {
            unit.GetComponent<SharedStats>().myEnemies = enemyTeam;
        }
    }

    public void KillPUnit( int target)
    {
        foreach(GameObject unit in eUnits)
        {
            RemoveAt(ref unit.GetComponent<SharedStats>().myEnemies, target);
        }
        if (pUnits[target].GetComponent<SharedStats>().numInQ < UnitIndex )
        {
            UnitIndex--;
        }
        RemoveAt(ref UnitsInGame, pUnits[target].GetComponent<SharedStats>().numInQ - 1);
        pUnits[target].SetActive(false);

        //CheckForGameOver ################################################################
    }

    //Highlights enemy in game field
    public void HighlightEnemies(bool show)
    {
        foreach (GameObject unit in eUnits)
        {
            MeshRenderer[] comp = unit.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer part in comp)
            {
                if (part.name == "Pointer")
                    part.enabled = show;
            }   
        }
        pCanAttack = show;
        
    }

    //Hero's BasicAttack
    public void HeroAttack()
    {
          //print(UnitsInGame[UnitIndex].name);
        if (Input.GetMouseButtonDown(0) && UnitsInGame[UnitIndex].tag == "Player" && UnitsInGame[UnitIndex].GetComponent<SharedStats>().myTurn == true)
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out hit);
            if (hit.collider && hit.collider.tag == "Unit")
            {
                target = hit.collider.gameObject;
                print(target.name);
                HighlightEnemies(false);
                pHasTraget = true;
            }
        }
    }

    //Remove from array and rearange, fount at https://answers.unity.com/questions/1074164/remove-element-from-array.html
    public static void RemoveAt<T>(ref T[] arr, int index)
    {
        for (int a = index; a < arr.Length - 1; a++)
        {
            arr[a] = arr[a + 1];
        }
        System.Array.Resize(ref arr, arr.Length - 1);
    }  
}