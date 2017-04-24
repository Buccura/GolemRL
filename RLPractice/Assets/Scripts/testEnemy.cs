using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class testEnemy : MonoBehaviour {

    public NavMeshAgent navAgent;
    public GameObject player;

	// Use this for initialization
	void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
    }


    // Update is called once per frame
    void Update()
    {

        
    }
}
