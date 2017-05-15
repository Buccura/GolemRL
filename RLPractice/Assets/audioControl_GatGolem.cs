using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioControl_GatGolem : MonoBehaviour {

    public AudioSource stepSound;
    public AudioSource ded;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Step()
    {
        stepSound.Play();
    }

    public void Ded()
    {
        ded.Play();
    }

}
