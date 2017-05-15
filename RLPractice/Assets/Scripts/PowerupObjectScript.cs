//GolemRL Powerup Object Script
using UnityEngine;

public class PowerupObjectScript : MonoBehaviour
{	public bool timeout = false; //Despawns after lifetime expires
	public float lifetime = 60.0f; //Time to live in seconds
	public float spin_rate = 15.0f; //Counter-clockwise spin (deg/sec)
	public GameObject[] powerup_bonus_prefabs; //Powerup prefabs to attach to this unit

	private GameObject[] powerup_bonuses; //Powerups to give to unit

	void Start()
	{	if (powerup_bonus_prefabs.Length > 0) //Prefab bonuses
		{	powerup_bonuses = new GameObject[powerup_bonus_prefabs.Length];
			for(int i = 0; i < powerup_bonus_prefabs.Length; i++)
			{	GameObject bonus_instance = Instantiate(powerup_bonus_prefabs[i],transform);
				powerup_bonuses[i] = bonus_instance;
			}
		}
		else
		{	PowerupBonusScript[] bonuses = GetComponentsInChildren<PowerupBonusScript>();
			if (bonuses.Length > 0) //Instanced bonuses
			{	powerup_bonuses = new GameObject[bonuses.Length];
				for(int i = 0; i < bonuses.Length; i++)
				{	powerup_bonuses[i] = bonuses[i].gameObject;
				}
			}
		}
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
			if (powerup_bonuses != null)
			{	for(int i = 0; i < powerup_bonuses.Length; i++)
				{	GrantPowerupBonus(target, powerup_bonuses[i]);
				}
			}
			Destroy(gameObject);
		}
	}

	void GrantPowerupBonus(GameObject unit, GameObject powerup_type)
	{	float stren = 0.0f;
		float max_dur = 0.0f;
		float remain = 0.0f;
		PowerupBonusScript powerup = powerup_type.GetComponent<PowerupBonusScript>();
		PowerupBonusScript[] buffs = unit.GetComponentsInChildren<PowerupBonusScript>();
		foreach( PowerupBonusScript buff in buffs )
		{	if ( buff.bonus_type == powerup.bonus_type )
			{	stren = buff.strength;
				max_dur = buff.max_duration;
				remain = buff.lifetime;
				buff.End();
				break;
			}
		}
		GameObject new_powerup = Instantiate(powerup_type,unit.transform);
		PowerupBonusScript new_buff = new_powerup.GetComponent<PowerupBonusScript>();
		if (new_buff.strength < stren ) //Inferior bonuses recover duration
		{	new_buff.strength = stren;
			new_buff.max_duration = max_dur;
			new_buff.duration += remain;
		}
		new_buff.Begin();
		//Debug.Log(new_buff);
	}
}
