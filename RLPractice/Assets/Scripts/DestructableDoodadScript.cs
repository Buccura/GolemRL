using UnityEngine;

public class DestructableDoodadScript : MonoBehaviour
{	public float hitpoints = 10.0f; //Hitpoints until destruction
	public float loot_heading = 0.0f; //Y-rotation of loot object
	public Vector3 loot_offset; //Shift loot object position
	public GameObject loot_prefab; //Object to spawn on death
	public GameObject[] bonus_prefabs; //Use for attaching bonuses to a powerup

	void Start()
	{	
	}

	void Update()
	{		
	}

	public void TakeDamage(float amount)
	{	hitpoints -= amount;
		if (hitpoints <= 0.0f)
		{	Die();
		}
	}

	public void Die()
	{	if (loot_prefab != null)
		{	GameObject spawned_object = Instantiate(loot_prefab, transform.position+loot_offset, Quaternion.Euler(0.0f,loot_heading,0.0f));
			
			PowerupObjectScript powerup = spawned_object.GetComponent<PowerupObjectScript>();
			if (powerup != null)
			{	powerup.powerup_bonus_prefabs = bonus_prefabs;
			}
		}
		Destroy(gameObject);
	}
}
