//GolemRL Player Object Script
//TODO: Bullets at wierd angles
using UnityEngine;

public class PlayerController : MonoBehaviour
{   public bool cam_relative_controls = false;
    public float player_speed = 5.0f; //Move speed
	public float player_turn_rate = 180.0f; //Turn speed (degrees/sec)
	public float player_projectile_speed = 5.0f; //Base projectile speed
	public GameObject basic_bullet;

	private Vector3 player_vel;

	void Start()
	{	
	}

	void Update()
	{	float aim_angle; //The degree angle between the player heading and cursor
		float turn_angle; //The number of degrees the player will rotate this tick
        Vector3 aim_point; //Where the cursor is pointing
		Vector3 aim_vector; //The unit vector from the player to the cursor, desired heading

        //Movement
        player_vel.x = Input.GetAxis("Horizontal") * player_speed;
        player_vel.z = Input.GetAxis("Vertical") * player_speed;
        if (cam_relative_controls && Camera.main != null) //Rotate movement along y-axis to match camera
        {   float cos_y = Mathf.Cos( Camera.main.transform.eulerAngles.y*Mathf.Deg2Rad );
            float sin_y = Mathf.Sin( Camera.main.transform.eulerAngles.y*Mathf.Deg2Rad );
            float temp = player_vel.x;
            player_vel.x = player_vel.x * cos_y + player_vel.z * sin_y;
            player_vel.z = player_vel.z * cos_y - temp * sin_y;
        }

        //Aiming
        if (Camera.main != null)
		{	float d; //Distance to intersection with y-plane
			float denom; //Dot product of aim_dir and y_norm, denominator in distance equation
			Ray aim_ray; //Ray from camera though cursor
			Vector3 y_norm = new Vector3(0.0f,1.0f,0.0f); //Normal vector to y-plane
			
			aim_ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			denom = Vector3.Dot(aim_ray.direction, y_norm);
			if (denom != 0.0f) //Not parallel to y-plane
			{	d = Vector3.Dot(-aim_ray.origin, y_norm) / denom;
				//d = ([point on plane]-[point on ray] dot y_norm) / ([ray_direction] dot y_norm)
				aim_point = aim_ray.GetPoint(d); //Intersection point with y-plane
			}
			else
			{	aim_point = transform.forward;
				Debug.Log ("Warning: Cursor out of bounds!");
			}
		}
		else
		{	aim_point = transform.forward;
			Debug.Log ("Warning: Main camera not found!");
		}
        Debug.DrawLine(transform.position, aim_point, Color.yellow);
        Debug.DrawRay(transform.position, transform.forward, Color.green);
        aim_vector = aim_point - transform.position; //Displacement from player to cursor
        aim_vector.y = 0.0f; //Nullify y-component
        aim_vector.Normalize(); //Unit vector of desired heading
        Debug.DrawRay(transform.position, aim_vector, Color.red);
        aim_angle = Mathf.Clamp(Vector3.Dot(transform.forward, aim_vector), -1.0f, 1.0f); //Prevent NaN due to rounding error
        aim_angle = Mathf.Acos(aim_angle); //Angle between heading and desired heading
        aim_angle *= Mathf.Sign( aim_vector.x*transform.forward.z - aim_vector.z*transform.forward.x ); //Signed angle
		aim_angle *= Mathf.Rad2Deg; //Desired rotation change in degrees
        turn_angle = Mathf.Sign(aim_angle) * player_turn_rate * Time.deltaTime; //Maximum possible turn this tick
		if( Mathf.Abs(aim_angle) < Mathf.Abs(turn_angle) ) //If desired rotation less than maximum
		{	turn_angle = aim_angle; //Then don't overshoot (prevents shakiness)
        }

		//Update transforms
		transform.Translate(player_vel.x*Time.deltaTime, 0.0f, player_vel.z*Time.deltaTime, Space.World);
        transform.Rotate(0.0f, turn_angle, 0.0f);
       
		//Firing
		if ( Input.GetMouseButtonDown(0) )
		{	GameObject bullet = Instantiate(basic_bullet, transform.position + transform.forward, transform.rotation);
			BulletController bullet_ctrl = bullet.GetComponent<BulletController>();
			bullet_ctrl.bullet_vel = player_vel + transform.forward*player_projectile_speed;
        }
	}
}
