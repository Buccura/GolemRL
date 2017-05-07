//GolemRL Powerup Bonus Script
//A script inheriting from this one should be attached to a prefab as a template to add to a unit.
using UnityEngine;

public class PowerupBonusScript : MonoBehaviour
{	public string bonus_type = "Null"; //Bonus type ID for purposes of stacking
	public float duration = 10.0f; //Duration of bonus (seconds)
	public float lifetime; //Remaining duration of bonus (seconds)
	public float strength = 1.0f; //Strength of bonus

	public virtual void Start()
	{	lifetime = duration;
	}

	public virtual void Update()
	{	lifetime -= Time.deltaTime;
		if (lifetime < 0.0f)
		{	End();
		}
	}

	public virtual void End() //Terminate this buff
	{	Destroy(this);
	}
}
