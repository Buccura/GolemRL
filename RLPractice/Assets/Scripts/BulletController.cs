//GolemRL Bullet Object Script
using UnityEngine;

public class BulletController : MonoBehaviour
{	public bool piercing = false; //Bullet only stops for walls
	public bool destroy_on_impact = true; //Bulled destroyed instead of stopping
	public bool take_damage = false; //Can be destroyed by other projectiles
	public float lifetime = 4.0f; //Time to live in seconds
	public float damage = 1.0f; //Damage in hitpoints
	public float hitpoints = 0.0f; //Hitpoints if can be damaged
	public float pierce_hp_cost = 0.0f; //Hitpoints lost each impact
	public Vector3 bullet_vel; //Trajectory of bullet
	public GameObject owner; //Unit that fired projectile

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
		if (target != null && target.tag != "Floor")
		{	//Debug.Log(target.tag);
			if (target.tag == "Wall")
			{	bullet_vel = Vector3.zero;
				if (destroy_on_impact)
				{	Destroy(gameObject);
				}
			}
			else if (target.tag == "Destructible") //Doodads
			{	DestructableDoodadScript ds = target.GetComponent<DestructableDoodadScript>();
				ds.TakeDamage(damage);
				CheckPierce();
			}
			else if (target.tag == "Enemy")
			{	if (owner == null || owner.tag != "Enemy")
				{	//TODO: Damage enemy and add score
					CheckPierce();
				}
			}
			else if (target.tag == "Player")
			{	if (owner == null || owner.tag != "Player")
				{	PlayerController pc = target.GetComponent<PlayerController>();
					pc.TakeDamage(damage,owner);
					CheckPierce();
				}
			}
			else if (target.tag == "Projectile")
			{	BulletController target_ctrl = target.GetComponent<BulletController>();
				if (target_ctrl.take_damage && (owner == null || target_ctrl.owner == null || target_ctrl.owner.tag != owner.tag) ) //Avoid friendly fire
				{	target_ctrl.hitpoints -= damage;
					if (target_ctrl.hitpoints <= 0.0f)
					{	Destroy(target);
					}
					CheckPierce();
				}
			}
		}
	}

	protected void CheckPierce() //Check whether to destroy or continue
	{	if (!piercing)
		{	Destroy(gameObject);
		}
		else
		{	hitpoints -= pierce_hp_cost;
			if (hitpoints <= 0.0f)
			{	Destroy(gameObject);
			}
		}
	}
}
