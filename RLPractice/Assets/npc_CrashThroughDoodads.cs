using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_CrashThroughDoodads : MonoBehaviour {

    public bool isChasing;
	
	void OnCollisionEnter(Collision col)
    {
        Debug.Log(col.gameObject.name + " " + col.gameObject.layer);
        if(col.gameObject.layer == 10)
        {
            Destroy(col.gameObject);
        }
    }
}
