//GolemRL Player Object
using UnityEngine;

public class PlayerController : MonoBehaviour
{	public double player_speed = 100.0; //Move speed
	public double player_turn_rate = 90.0; //Turn speed (degrees/sec)
	public double player_projectile_speed = 200.0; //Projectile muzzel velocity
	public Projectile basic_bullet;
	
	void Update()
	{	//Movement
		double x_mov = Input.GetAxis("Horizontal") * Time.deltaTime * player_speed;
		double z_mov = Input.GetAxis("Vertical") * Time.deltaTime * player_speed;
		
		//Aiming
		Vector3 aim_point Camera.ScreenToWorldPoint(Input.mousePosition); //Pointer location
		Vector3 aim_displace = aim_point - transform.Position; //Displacement from player to pointer
		Vector3 player_heading = transform.Forward;
		double aim_angle = Math.Atan2(aim_displace.x,aim_displace.z) - Math.Atan2(player_heading.x,player_heading.z);
		aim_angle *= 180.0 / Math.PI; //Desired rotation change in degrees
		double turn_angle = Math.Sign(aim_angle) * player_turn_rate * Time.deltaTime; //Maximum possible turn this tick
		if( Math.Abs(aim_angle) < player_turn_rate*Time.deltaTime ) //If desired rotation less than maximum
		{	turn_angle = aim_angle; //Then don't overshoot
		}
		
		//Update transforms
		transform.Translate(x_mov, 0.0, z_mov);
		transform.Rotate(0.0, turn_angle, 0);
		
		//Firing
		if ( Input.GetMouseButtonDown(0) )
		{	Projectile proj = Instantiate(basic_bullet, transform.position, transform.rotation);
			proj.rigidbody.AddForce( Vector3(x_mov, 0.0, z_mov) + transform.forward*player_projectile_speed );
		}
	}
}
