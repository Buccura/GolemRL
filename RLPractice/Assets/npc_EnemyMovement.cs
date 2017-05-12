using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class npc_EnemyMovement : MonoBehaviour {

    public enum behavior {shoot,suicide,}

    public behavior AIBehavior;

	public NavMeshAgent navAgent;
	public GameObject player;
    public GameObject child;
    public LayerMask maskIgnoreDoodads;
    public LayerMask maskWithDoodads;
    public bool sawPlayer;
    public bool playerInSight;
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
        if(Physics.Raycast(transform.position,raycastDir,out hit, 100f,maskIgnoreDoodads))
        {
            if (hit.collider.gameObject.tag == "Player" && hit.distance <= spotDistance && !sawPlayer)
            {
                sawPlayer = true;
            }

        }

        if (sawPlayer)
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
            if(AIBehavior == behavior.shoot)
            {
                if (Physics.Raycast(transform.position, raycastDir, out hit2, 100f, maskWithDoodads))
                {
                    if (hit2.collider.gameObject.tag != "Player")
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
            else if(AIBehavior == behavior.suicide)
            {
                navAgent.speed = speed;
                navAgent.destination = player.transform.position;
            }

        }


	}

    void LateUpdate()
    {
        if(AIBehavior == behavior.suicide)
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

    void OnTriggerEnter(Collider col)
    {
        if(AIBehavior == behavior.suicide)
        {
            if(col.gameObject.tag == "Player")
            {
                Destroy(col.gameObject);
            }
        }
    }

}
