//GolemRL Player Object Script
using UnityEngine;

public class PlayerController : MonoBehaviour
{	public float player_speed = 5.0f; //Move speed
	public float player_turn_rate = 90.0f; //Turn speed (degrees/sec)
	public float player_projectile_speed = 50.0f; //Base projectile speed
	public GameObject basic_bullet;

	private Vector3 player_vel;

	void Start()
	{	
	}

	void Update()
	{	float aim_angle; //The angle between the player heading and cursor
		float turn_angle; //The number of degrees the player will rotate this tick
		Vector3 aim_point; //Where the cursor is pointing
		Vector3 aim_displace; //The displacement vector from the player to the cursor

		//Movement
		player_vel.x = Input.GetAxis("Horizontal") * player_speed;
		player_vel.z = Input.GetAxis("Vertical") * player_speed;

        //Aiming
        if (Camera.main != null)
		{	aim_point = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Pointer location //Actually camera location!
		}
		else
		{	aim_point = transform.forward;
			Debug.Log ("Warning: Main camera not found!");
		}
        aim_displace = aim_point - transform.position; //Displacement from player to cursor
		aim_angle = Mathf.Atan2(aim_displace.x,aim_displace.z) - Mathf.Atan2(transform.forward.x,transform.forward.z);
		aim_angle *= 180.0f / Mathf.PI; //Desired rotation change in degrees
		turn_angle = Mathf.Sign(aim_angle) * player_turn_rate; //Maximum possible turn this tick
		if( Mathf.Abs(aim_angle) < player_turn_rate ) //If desired rotation less than maximum
		{	turn_angle = aim_angle; //Then don't overshoot (prevents shakiness)
        }

		//Update transforms
		transform.Translate(player_vel.x*Time.deltaTime, 0.0f, player_vel.z*Time.deltaTime);
		transform.Rotate(0.0f, turn_angle*Time.deltaTime, 0.0f);

		//Firing
		if ( Input.GetMouseButtonDown(0) )
		{	GameObject bullet = Instantiate(basic_bullet, transform.position + transform.forward, transform.rotation);
			BulletController bullet_ctrl = bullet.GetComponent<BulletController>();
			bullet_ctrl.bullet_vel = player_vel + transform.forward*player_projectile_speed;
        }
	}
}
