//GolemRL Powerup Object Script
using UnityEngine;

public class PowerupScript : MonoBehaviour {
	public bool timeout = false; //Despawns after lifetime expires
	public float lifetime = 60.0f; //Time to live in seconds
	public float spin_rate = 15.0f; //Counter-clockwise spin (deg/sec)

	void Start()
	{	
	}

	void Update()
	{	if (timeout)
		{	lifetime -= Time.deltaTime;
			if (lifetime < 0.0f)
			{	Destroy(gameObject);
			}
		}
	}

	void FixedUpdate()
	{	transform.Rotate(0.0f, spin_rate*Time.fixedDeltaTime, 0.0f);
	}

	void OnTriggerEnter(Collider other) //Trigger collision
	{	GameObject target;
		target = other.gameObject;
		if (target.name == "Player")
		{	Debug.Log("Powerup!");
			GrantPowerup(target);
			Destroy(gameObject);
		}
	}

	void GrantPowerup(GameObject player)
	{
	}
}
