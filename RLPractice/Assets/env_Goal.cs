using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class env_Goal : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            SceneManager.LoadScene(2);
        }
    }
}
