using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_Shooting : MonoBehaviour {

    public npc_EnemyMovement eM;
    public GameObject shootPoint;
    public GameObject bullet;
    public float fireRateSet;
    private float fireRate;

    void Start()
    {
     //   eM = GetComponent<npc_EnemyMovement>();
    }

    void Update()
    {

        if(eM.playerInSight && fireRate <= 0)
        {
            Instantiate(bullet, shootPoint.transform.position, transform.localRotation);
            fireRate = fireRateSet;
        }
        if (fireRate > 0)
        {
            fireRate -= Time.deltaTime;
        }

    }
}
