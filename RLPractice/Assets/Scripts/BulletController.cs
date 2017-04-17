//GolemRL Bullet Object Script
using UnityEngine;

public class BulletController : MonoBehaviour {
	public float lifetime = 4.0f; //Time to live in seconds
	public int damage = 0; //Damage in hitpoints
	public Vector3 bullet_vel;

	//private Rigidbody rb;

	void Start()
	{	//rb = GetComponent<Rigidbody> ();
	}
	
	void Update()
	{	lifetime -= Time.deltaTime;
		if (lifetime < 0.0f)
		{	Destroy(gameObject);
		}
		transform.Translate(bullet_vel*Time.deltaTime, Space.World);
	}

	/*void FixedUpdate()
	{	rb.velocity = bullet_vel*Time.deltaTime;
	}*/
}
