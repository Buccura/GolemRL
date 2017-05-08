//GolemRL Powerup Bonus Script
//Move Speed Buff
using UnityEngine;

public class PowerupMoveSpeed : PowerupBonusScript
{	private PlayerController movement_script;

	public override void Begin() //Initiate this buff
	{	base.Begin();
		if (owner != null)
		{	movement_script = owner.GetComponent<PlayerController>();
			if (movement_script != null)
			{	movement_script.player_speed *= strength;
			}
		}
	}

	public override void End() //Terminate this buff
	{	if (movement_script != null)
		{	movement_script.player_speed /= strength;
		}
		base.End();
	}
}
