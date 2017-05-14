using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class npc_EnemyMovement : MonoBehaviour {

	public NavMeshAgent navAgent;
	public GameObject player;
    public GameObject child;
    public LayerMask maskIgnoreDoodads;
    public LayerMask maskWithDoodads;
    public bool sawPlayer;
    public bool playerInSight;
	public bool playerAlive;
    public float spotDistance;

    public float speed;

    public float angerTimerSet;
    private float angerTimer = 0;
    public bool isAngry;

	void Start()
	{
        spotDistance = 30f;
		player = GameObject.FindGameObjectWithTag("Player");
		navAgent = GetComponent<NavMeshAgent>();		
	}
	
	void Update()
	{
        Vector3 raycastDir = player.transform.position - transform.position;
        RaycastHit hit;

		playerAlive = player.GetComponent<PlayerController>().player_alive;
        if(Physics.Raycast(transform.position,raycastDir,out hit, 100f,maskIgnoreDoodads))
        {
            if (hit.collider.gameObject.tag == "Player" && hit.distance <= spotDistance && !sawPlayer)
            {
                sawPlayer = true;
            }

        }

		if (sawPlayer && playerAlive)
        {
            if(angerTimer <= angerTimerSet && !isAngry)
            {
                angerTimer += Time.deltaTime;
            }
            else if(angerTimer > angerTimerSet && !isAngry)
            {
                isAngry = true;
                maskWithDoodads = maskIgnoreDoodads;
            }
            RaycastHit hit2;
            
            if (Physics.Raycast(transform.position, raycastDir, out hit2,100f, maskWithDoodads))
            {
                if(hit2.collider.gameObject.tag != "Player")
                {
                    playerInSight = false;
                    navAgent.speed = speed;
                    navAgent.destination = player.transform.position;
                }
                else
                {
                    playerInSight = true;
                }
            }
        }


	}

    void LateUpdate()
    {
        if (playerInSight && sawPlayer)
        {
            navAgent.speed = 0f;
            Vector3 target = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            Quaternion rot = Quaternion.LookRotation(target);
            Vector3 newDir = Vector3.RotateTowards(child.transform.forward, target - transform.position, 10f * Time.deltaTime, 0.0f);
            child.transform.rotation = Quaternion.LookRotation(newDir);
        }
        else
        {
            // child.transform.rotation = transform.rotation;
            Vector3 newDir = Vector3.RotateTowards(child.transform.forward, transform.forward, 15f * Time.deltaTime, 0.0f);
            child.transform.rotation = Quaternion.LookRotation(newDir);
        }

    }

}
