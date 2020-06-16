using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSAssigner : SharedStats
{
    void Awake()
    {
        turnManager = gameObject;
        GameObject anim = GameObject.Find("AttackAnimation");
        attackHolder = anim.transform;
        attackingAnim = anim.GetComponent<Animator>();
    }

}
