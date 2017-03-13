using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    public bool followPlayer;
    public GameObject player;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate ()
    {
	    if(Input.GetKey(KeyCode.F))
        {
            followPlayer = true;
        }

        float x = Mathf.Lerp(transform.position.x, player.transform.position.x, Time.deltaTime * 15f);
        float z = Mathf.Lerp(transform.position.z, player.transform.position.z, Time.deltaTime * 15f);
        transform.position = new Vector3(x, 0f, z);

        
	}
}
