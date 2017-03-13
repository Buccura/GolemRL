//GolemRL Player Object
using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{	public float player_speed = 100.0f; //Move speed
	public float player_turn_rate = 90.0f; //Turn speed (degrees/sec)
	public float player_projectile_speed = 200.0f; //Projectile muzzel velocity
	public GameObject basic_bullet;

	void Start()
	{	
	}

	void Update()
	{	//Movement
		float x_mov = Input.GetAxis("Horizontal") * Time.deltaTime * player_speed;
		float z_mov = Input.GetAxis("Vertical") * Time.deltaTime * player_speed;
		Vector3 aim_point;

		//Aiming
		if (Camera.main != null)
		{	aim_point = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Pointer location
		}
		else
		{	aim_point = transform.forward;
		}
		Vector3 aim_displace = aim_point - transform.position; //Displacement from player to pointer
		Vector3 player_heading = transform.forward;
		float aim_angle = Mathf.Atan2(aim_displace.x,aim_displace.z) - Mathf.Atan2(player_heading.x,player_heading.z);
		aim_angle *= 180.0f / Mathf.PI; //Desired rotation change in degrees
		float turn_angle = Mathf.Sign(aim_angle) * player_turn_rate * Time.deltaTime; //Maximum possible turn this tick
		if( Mathf.Abs(aim_angle) < player_turn_rate*Time.deltaTime ) //If desired rotation less than maximum
		{	turn_angle = aim_angle; //Then don't overshoot
		}

		//Update transforms
		transform.Translate(x_mov, 0.0f, z_mov);
		transform.Rotate(0.0f, turn_angle, 0.0f);

		//Firing
		if ( Input.GetMouseButtonDown(0) )
		{	GameObject shot = Instantiate(basic_bullet, transform.position, transform.rotation);
			shot.GetComponent<Rigidbody>().AddForce( new Vector3(x_mov, 0.0f, z_mov) + transform.forward*player_projectile_speed );
		}
	}
}
