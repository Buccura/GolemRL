//GolemRL Powerup Bonus Script
//A script inheriting from this one should be attached to a powerup and instantiated onto a unit.
using UnityEngine;

public class PowerupBonusScript : MonoBehaviour
{	public string bonus_type = "Null"; //Bonus type ID for purposes of stacking
	public float max_duration = 10.0f; //Max limit on duration of bonus (seconds)
	public float duration = 10.0f; //Duration of bonus (seconds)
	public float strength = 1.0f; //Strength of bonus
	public float lifetime; //Remaining duration of bonus (seconds)

	protected GameObject owner; //Unit with bonus
	protected bool active = false; //Bonus is attached to unit

	public virtual void Start()
	{	
	}

	public virtual void Update()
	{	if (active)
		{	lifetime -= Time.deltaTime;
			if (lifetime < 0.0f)
			{	End();
			}
		}
	}

	public virtual void Begin() //Initiate this buff
	{	if (duration > max_duration)
		{	duration = max_duration;
		}
		lifetime = duration;
		owner = transform.parent.gameObject; //Begin() doesn't wait for Start(), so put this here
		active = true;
	}

	public virtual void End() //Terminate this buff
	{	Destroy(gameObject);
	}
}
