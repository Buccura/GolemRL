using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet_Mover : MonoBehaviour {

    public float speed;
    private Rigidbody rb;

    void Start()
    {
        //rb = GetComponent<Rigidbody>();
        //rb.velocity = Vector3.forward * speed;
    }
}
