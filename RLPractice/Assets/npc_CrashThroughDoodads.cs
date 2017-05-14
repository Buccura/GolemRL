using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_CrashThroughDoodads : MonoBehaviour {

    public bool isChasing;
	
	void OnCollisionEnter(Collision col)
	{	GameObject obj = col.gameObject;
        Debug.Log(obj.name + " " + obj.tag);
		if (obj.tag == "Destructible")
		{	DestructableDoodadScript ds = obj.GetComponent<DestructableDoodadScript>();
			ds.Die();
		}
    }
}
