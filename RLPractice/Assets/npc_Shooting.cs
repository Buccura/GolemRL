using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_Shooting : MonoBehaviour {

    public npc_EnemyMovement eM;
	public GunController gun_script;
    

    void Start()
    {
        //eM = GetComponent<npc_EnemyMovement>();
    }

    void Update()
    {
		if(eM.playerInSight && eM.playerAlive && !eM.isDead && gun_script.FireReady() )
		{	gun_script.Fire(Vector3.zero);
        }
    }
}
