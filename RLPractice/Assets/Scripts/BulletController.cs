﻿using UnityEngine;

public class BulletController : MonoBehaviour {
	public Vector3 bullet_vel;
	public float lifetime = 4.0f; //Time to live in seconds

	void Start()
	{	
	}
	
	void Update()
	{	lifetime -= Time.deltaTime;
		if (lifetime < 0.0f)
		{	Destroy(gameObject);
		}
		transform.Translate(bullet_vel * Time.deltaTime);
	}
}
