using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_CheckCurrentTile : MonoBehaviour {

    public LayerMask mask;
    public BoardManager bMan; // Reference to the board manager script

    void Start()
    {
        bMan = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BoardManager>();
    }


    void Update()
    {
        RaycastHit hit;
        Quaternion rot = new Quaternion(90f, 0f, 0f, 0f);
        if (Physics.Raycast(transform.position,-Vector3.up,out hit, mask))
        {
            bMan.playerX = hit.collider.gameObject.GetComponent<env_floorTile>().x;
            bMan.playerY = hit.collider.gameObject.GetComponent<env_floorTile>().y;
        }

    }
}
