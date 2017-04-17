using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraWallSeeThru : MonoBehaviour {


    void Start()
    {

    }


    void Update()
    {

    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Wall")
        {
            col.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Wall")
        {
            col.gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
    }
}
