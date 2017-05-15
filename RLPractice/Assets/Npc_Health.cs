using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc_Health : MonoBehaviour {

    public float HP;
    public Animator anim;
    bool already_dead = false;
    public npc_EnemyMovement eM;
    public GunController gC;

    void Start()
    {
      //  eM = GetComponent<npc_EnemyMovement>();
    }

    void Update ()
    {
        if (HP <= 0 && already_dead == false)
        {
            anim.SetBool("Dead", true);
            eM.isDead = true;
            gC.isDead = true;
            GetComponent<BoxCollider>().enabled = false;
            foreach(BoxCollider bC in GetComponentsInChildren<BoxCollider>())
            {
                bC.enabled = false;
            }
            already_dead = true;
           // Destroy(gameObject);
        }
        else
        {
             

        }
    }



}
