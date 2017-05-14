//GolemRL Powerup Bonus Script
//Instant Health Buff
using UnityEngine;

public class PowerupInstantHealth : PowerupBonusScript
{	public bool restore_percent = true;

	private PlayerController unit_script;

	public override void Update()
	{	//No constant effect
	}

	public override void Begin() //Initiate this buff
	{	base.Begin();
		if (owner != null)
		{	unit_script = owner.GetComponent<PlayerController>();
			if (unit_script != null && strength != 0.0f)
			{	unit_script.RestoreHealth(strength,restore_percent);
			}
		}
		End();
	}
}
