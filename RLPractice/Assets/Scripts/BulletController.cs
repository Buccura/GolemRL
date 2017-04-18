//GolemRL Bullet Object Script
using UnityEngine;

public class BulletController : MonoBehaviour {
	public bool piercing = false; //Bullet only stops for walls
	public bool destroy_on_impact = true; //Bulled destroyed instead of stopping
	public float lifetime = 4.0f; //Time to live in seconds
	public int damage = 0; //Damage in hitpoints
	public Vector3 bullet_vel;

	//private Rigidbody bullet_rb;

	void Start()
	{	//bullet_rb = GetComponent<Rigidbody>();
	}
	
	void Update()
	{	lifetime -= Time.deltaTime;
		if (lifetime < 0.0f)
		{	Destroy(gameObject);
		}
	}

	void FixedUpdate()
	{	transform.Translate(bullet_vel*Time.fixedDeltaTime, Space.World);
		/*bullet_rb.velocity = bullet_vel;
		if (bullet_rb.isKinematic)
		{	transform.Translate(bullet_vel*Time.fixedDeltaTime, Space.World);
		}
		Debug.DrawRay(bullet_rb.transform.position,bullet_rb.velocity*Time.fixedDeltaTime,Color.green,Time.fixedDeltaTime);*/
	}

	/*void OnCollisionEnter(Collision collision) //Rigidbody collision
	{	GameObject target;
		target = collision.gameObject;
		if (target != null)
		{	Debug.Log(target);
			if (destroy_on_impact)
			{	Destroy(gameObject);
			}
			else if (!piercing)
			{	bullet_vel = Vector3.zero;
			}
		}
	}*/

	void OnTriggerEnter(Collider other) //Trigger collision
	{	GameObject target;
		target = other.gameObject;
		if (target != null && target.name != "Player" && target.name != "Bullet(Clone)")
		{	Debug.Log(target);
			if (target.name == "WallPiece(Clone)")
			{	bullet_vel = Vector3.zero;
				if (destroy_on_impact)
				{	Destroy(gameObject);
				}
			}
			else if (!piercing)
			{	bullet_vel = Vector3.zero;
				if (destroy_on_impact)
				{	Destroy(gameObject);
				}
			}
		}
	}
}
