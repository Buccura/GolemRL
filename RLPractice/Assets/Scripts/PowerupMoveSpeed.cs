//GolemRL Powerup Bonus Script
//Move Speed Buff
using UnityEngine;

public class PowerupMoveSpeed : PowerupBonusScript
{	private PlayerController movement_script;

	public override void Start()
	{	movement_script = gameObject.GetComponent<PlayerController>();
		movement_script.player_speed *= strength;
		base.Start();
	}

	public override void End() //Terminate this buff
	{	movement_script.player_speed /= strength;
		base.End();
	}
}
