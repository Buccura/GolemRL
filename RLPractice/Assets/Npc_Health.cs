using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc_Health : MonoBehaviour {

    public float HP;
    public Animator anim;

    public npc_EnemyMovement eM;
    public GunController gC;

    void Start()
    {
      //  eM = GetComponent<npc_EnemyMovement>();
    }

    void Update ()
    {
        if (HP <= 0)
        {
            anim.SetBool("Dead", true);
            eM.isDead = true;
            gC.isDead = true;
           // Destroy(gameObject);
        }
    }



}
