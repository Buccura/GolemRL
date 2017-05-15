using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioControl_Brock : MonoBehaviour {

    public AudioSource step;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Step()
    {
        step.Play();
    }
}
