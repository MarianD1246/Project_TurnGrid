using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SharedStats : MonoBehaviour
{
    public int UnitLevel;                        //Level of Unit
    public int Speed;                            //Determines initiative
    public int MoveTile;                         //Movement 
    public int Health;                           //HP         
    public int AttackDamage;                     //DMG
    public int numInQ;                           //Place in turn queue
    public bool myTurn = false;                  //Determines who's turn it is
    public GameObject[] myEnemies;               //Enemy elements
    protected static GameObject turnManager;     //Holds turn manager GameObject
    protected static Transform attackHolder;     //Used for parenting attacking object
    protected static Animator attackingAnim;     //The attack animator 
    private float attackTimer;                   //Timer used for attack animation
    private bool inAnim, attacked;               //Bools used in the attack animation
    private GameObject attackTarget;

    //Function to Calculate Stats of unit, stats scale based of lvl
    public void CalculateStats(int Lvl, int HpBase, float HpMulti, int Move, int ABase, float AMulti )
    {
        Health = HpBase + Mathf.FloorToInt(Lvl * HpMulti);
        MoveTile = Move;
        AttackDamage = ABase + Mathf.FloorToInt(Lvl * AMulti);
    }

    //Pefrom an bassic attack on the selected target
    public void BasicAttacks(GameObject target, bool playerExepction)
    {
        if (!inAnim)
        {
            attackTarget = target;
            attackTimer = Time.time + .5f;
            EnablePointer(gameObject, true);
            EnablePointer(attackTarget, true);
            gameObject.transform.parent = attackHolder;
            attackingAnim.SetBool("Attack", true);
            print(gameObject.name + "'s turn...");
            inAnim = true;
        }

        if (!attacked)
        {
            
            if (Time.time > attackTimer)
            {
                int enemyHP = attackTarget.GetComponent<SharedStats>().Health;
                enemyHP -= AttackDamage;
                attackTarget.GetComponent<SharedStats>().Health = enemyHP;
                Debug.LogWarning(attackTarget.name + " took " + AttackDamage + " damage");

                if (enemyHP <= 0)   //killed unit?
                {
                    //to be revised ############################################################################
                    //killing myEnemies means remove myEnemy from my array
                    //remove myEnemy from ally array
                    //which means figureing if myEnemy is unit or player
                    //then removing from eUnit pUnit and UnitsinGame
                    print("Killed the player uint:" + attackTarget.name);
                    //turnManager.GetComponent<TurnManager>().KillPUnit(target);
                }

                attackTimer = Time.time + 1;
                attacked = true;
                attackingAnim.SetBool("Attack", false);
                EnablePointer(attackTarget, false);
            }
        }

        if (Time.time > attackTimer && attacked)
        {
            gameObject.transform.parent = null;
            EnablePointer(gameObject, false);
            attacked = false;
            inAnim = false;
            myTurn = false;
            if (playerExepction)
            {
                turnManager.GetComponent<TurnManager>().pCanAttack = false;
                turnManager.GetComponent<TurnManager>().pHasTraget = false;
            }
        }
    }

    public void EnablePointer(GameObject meshTarget, bool show)
    {
        MeshRenderer[] targetedMesh = meshTarget.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer part in targetedMesh)
        {
            if (part.name == "Pointer")
                part.enabled = show;
        }
    }
}
