//GolemRL Powerup Object Script
using UnityEngine;

public class PowerupObjectScript : MonoBehaviour
{	public bool timeout = false; //Despawns after lifetime expires
	public float lifetime = 60.0f; //Time to live in seconds
	public float spin_rate = 15.0f; //Counter-clockwise spin (deg/sec)
	public GameObject bonus_prefab; //Prefab with powerup bonus script to attach to the unit

	private PowerupBonusScript bonus_script; //Script that handles buff

	void Start()
	{	bonus_script = bonus_prefab.GetComponent<PowerupBonusScript>();
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
		if (target.tag == "Player")
		{	Debug.Log("Powerup!");
			GrantPowerupBonus(target, bonus_script);
			Destroy(gameObject);
		}
	}

	void GrantPowerupBonus(GameObject unit, PowerupBonusScript powerup)
	{	float stren = 0.0f;
		float dur = 0.0f;
		float remain = 0.0f;
		PowerupBonusScript[] buffs = unit.GetComponents<PowerupBonusScript>();
		foreach( PowerupBonusScript buff in buffs )
		{	if ( buff.bonus_type == powerup.bonus_type )
			{	stren = buff.strength;
				dur = buff.duration;
				remain = buff.lifetime;
				buff.End();
				break;
			}
		}
		PowerupBonusScript new_buff = unit.AddComponent<PowerupBonusScript>();
		new_buff = powerup;
		if (new_buff.strength < stren ) //Inferior bonuses recover duration
		{	new_buff.strength = stren;
			new_buff.lifetime += remain;
			if (new_buff.lifetime > dur)
			{	new_buff.lifetime = dur;
			}
		}
	}
}
