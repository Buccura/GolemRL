//GolemRL Player Object Script
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{	public bool cam_relative_controls = false; //Player movement relative to camera instead of grid
	public bool player_alive = true; //Is the player still alive?
	public int player_score = 0; //Gain score by damaging and killing enemies
	public float player_hp = 100.0f; //Current hitpoints
	public float player_hp_max = 100.0f; //Max hitpoints
	public float player_armor_flat = 0.0f; //Additive damage reduction
	public float player_armor_scale = 1.0f; //Muplicative damage reduction
	public float player_damage_mult = 1.0f; //Outgoing damage multiplier
	public float player_speed = 5.0f; //Move speed (units/sec)
	public float player_turn_rate = 180.0f; //Turn speed (degrees/sec)
	public float player_gun_speed_mult = 1.0f; //Projectile speed multiplier
	public float player_gun_rof_mult = 1.0f; //Rate of fire multiplier
	public bool[] unlocked_weapons; //Unlocked weapon slots
	public GameObject[] player_weapons; //Array of available weapons
	public Animator anim;

    public float moveAngle;

    private int weapon_slots; //Number of selectable weapons
	private int selected_weapon = 0; //Value from 0 to weapon_slots-1
	private Vector3 player_vel; //Velocity vector
	private Quaternion player_aim; //Desired heading
	private Rigidbody player_rb; //Physics for player
	private GunController gun_ctrl; //Script of selected weapon

    public Slider HPBar;



	void Start()
	{	player_aim = new Quaternion();
		player_rb = GetComponent<Rigidbody>();
		Debug.Assert(player_weapons.Length == unlocked_weapons.Length); //Arrays must be equal length
		weapon_slots = player_weapons.Length;
		for(int i = 0; i < weapon_slots; i++)
		{	gun_ctrl = player_weapons[i].GetComponent<GunController>();
			if (!unlocked_weapons[i])
			{	gun_ctrl.Hide();
			}
			else if (gun_ctrl.hide_on_deselect)
			{	gun_ctrl.Hide();
			}
		}
		gun_ctrl = player_weapons[selected_weapon].GetComponent<GunController>();
		gun_ctrl.Select(0.0f);
	}

	void Update()
	{
        HPBar.value = player_hp;
        player_vel = Vector3.zero; //Don't drift away if dead
		if (player_alive)
		{	Vector3 aim_point; //Where the cursor is pointing
			Vector3 aim_vector; //A vector from the player to the cursor

			//Movement
			player_vel.x = Input.GetAxis("Horizontal") * player_speed;
			player_vel.z = Input.GetAxis("Vertical") * player_speed;
			if (cam_relative_controls && Camera.main != null) //Rotate movement along y-axis to match camera
			{	Quaternion camera_y_rot = Quaternion.Euler(0.0f, Camera.main.transform.eulerAngles.y, 0.0f);
				player_vel = camera_y_rot * player_vel; //Rotate velocity by camera y-axis rotation
			}

			// Animation Controls
			if (anim != null)
			{	if(player_vel.x != 0 || player_vel.z != 0)
				{	//anim.SetFloat("WalkForward", 1f);
					anim.SetBool("isWalking", true);
				}
				else
				{	anim.SetBool("isWalking", false);
					//anim.SetFloat("WalkForward", 0f);
				}
                // Debug.Log(Mathf.Atan2(player_vel.x,player_vel.z) * Mathf.Rad2Deg + " " + ( 180f * transform.rotation.y));

                moveAngle = Mathf.Abs((180f * transform.rotation.y) - (Mathf.Atan2(player_vel.x, player_vel.z) * Mathf.Rad2Deg));
                Debug.Log(player_vel.x);
                //Debug.Log(moveAngle);
				if(moveAngle < 65 && moveAngle > 28)  
				{	anim.SetBool("walkForward", true);
					anim.SetBool("walkBackward", false);
                    anim.SetBool("walkLeft", false);
                    anim.SetBool("walkRight", false);
                }
				else if(moveAngle >= 157 && moveAngle <= 247)
				{	anim.SetBool("walkForward", false);
					anim.SetBool("walkBackward", true);
                    anim.SetBool("walkLeft", false);
                    anim.SetBool("walkRight", false);
                }
                else if(moveAngle >= 65 && moveAngle < 157)
                {
                    
                    anim.SetBool("walkForward", false);
                    anim.SetBool("walkBackward", false);
                    anim.SetBool("walkLeft", true);
                    anim.SetBool("walkRight", false);
                }
                else if (moveAngle <= 28 || moveAngle > 247)
                {

                    anim.SetBool("walkForward", false);
                    anim.SetBool("walkBackward", false);
                    anim.SetBool("walkLeft", false);
                    anim.SetBool("walkRight", true);
                }
                else
				{	anim.SetBool("walkForward", false);
					anim.SetBool("walkBackward", false);
                    anim.SetBool("walkLeft", false);
                    anim.SetBool("walkRight", false);
                }
			}
			else
			{	//Debug.Log("Warning: Animator not set!");
			}

			//Aiming
			if (Camera.main != null)
			{	float dist; //Distance to intersection with xz-plane
				Ray look_ray; //Ray from camera to cursor
				Plane xz_plane; //Plane where Y=0

				look_ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				xz_plane = new Plane(Vector3.up,Vector3.zero);
				if ( xz_plane.Raycast(look_ray, out dist) ) //If look_ray intersects xz-plane
				{	aim_point = look_ray.GetPoint(dist); //Intersection point with xz-plane
				}
				else
				{	aim_point = transform.position + transform.forward;
					Debug.Log("Warning: Cursor out of bounds!");
				}
			}
			else
			{	aim_point = transform.position + transform.forward;
				Debug.Log("Warning: Main camera not found!");
			}
			aim_vector = aim_point - transform.position; //Displacement from player to cursor
			aim_vector.y = 0.0f; //Nullify y-component
			//aim_vector.Normalize();
			player_aim = Quaternion.LookRotation(aim_vector); //The heading from player to cursor
			Debug.DrawLine(transform.position, aim_point, Color.yellow);
			Debug.DrawRay(transform.position, transform.forward, Color.green);
			Debug.DrawRay(transform.position, aim_vector.normalized, Color.red);
		}
		HandleCommandInput(); //Various player key input
	}
	
	void FixedUpdate()
	{	player_rb.velocity = player_vel;
		player_rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, player_aim, player_turn_rate*Time.deltaTime));
	}

	public bool SwitchWeapon(int weapon_num)
	{	if ( selected_weapon == weapon_num )
		{	Debug.Log("Already equipped!");
			return false;
		}
		if (weapon_num >= 0 && weapon_num < weapon_slots && player_weapons[weapon_num] != null) //Valid weapon
		{	if (unlocked_weapons[weapon_num])
			{	GunController gc = player_weapons[weapon_num].GetComponent<GunController>();
				if ( gc.ammo_count >= gc.ammo_cost ) //Enough ammo
				{	gun_ctrl.Deselect();
					selected_weapon = weapon_num;
					gun_ctrl = gc;
					gun_ctrl.Select(1.0f); //TODO: Replace with proper animation time (swap out + swap in)
					return true;
				}
				else
				{	Debug.Log("Not enough ammo!");
					return false;
				}
			}
			else
			{	Debug.Log("Not unlocked!");
				return false;
			}
		}
		else
		{	Debug.Log("Error: Invalid weapon slot.");
			return false;
		}
	}
	
	public void NextWeapon() //Switch to next available weapon
	{	int checked_slot = (selected_weapon+1)%weapon_slots;
		bool not_found = true;
		while ( not_found && checked_slot != selected_weapon )
		{	if ( SwitchWeapon(checked_slot) )
			{	not_found = false;
			}
			else
			{	checked_slot = (checked_slot+1)%weapon_slots;
			}
		}
	}
	
	public void PrevWeapon() //Switch to previous available weapon
	{	int checked_slot = (selected_weapon-1+weapon_slots)%weapon_slots;
		bool not_found = true;
		while ( not_found && checked_slot != selected_weapon )
		{	if ( SwitchWeapon(checked_slot) )
			{	not_found = false;
			}
			else
			{	checked_slot = (checked_slot-1+weapon_slots)%weapon_slots;
			}
		}
	}

	public void RestoreHealth(float amount, bool percent = true, bool revive = false)
	{	if (revive)
		{	player_alive = true;
		}
		if (percent)
		{	player_hp *= amount;
		}
		else
		{	player_hp += amount;
		}
		if (player_hp > player_hp_max)
		{	player_hp = player_hp_max;
		}
	}

	public void TakeDamage(float amount, GameObject source, bool unreduced = false) //Player takes damage
	{	if (player_alive)
		{	if (unreduced)
			{	player_hp -= amount;
			}
			else
			{	player_hp -= Mathf.Max(0.0f, (amount-player_armor_flat)*player_armor_scale);
			}
			if ( player_hp <= 0.0f)
			{	player_alive = false;
				player_hp = 0.0f;
				Debug.Log(gameObject.name+" killed by "+source.name);
			}
		}
	}

	private void HandleCommandInput() //Various player input
	{	if (player_alive) //Stuff you need to be conscious for
		{	//Firing
			if ( Input.GetButtonDown("Fire1") )
			{	gun_ctrl.active_muzzle = 0; //Start at first muzzle for first shot
				if (gun_ctrl.ammo_count < gun_ctrl.ammo_cost)
				{	Debug.Log("Out of ammo! Switching to default weapon.");
					SwitchWeapon(0);
				}
			}
			if ( Input.GetButton("Fire1") && gun_ctrl.FireReady() )
			{	gun_ctrl.Fire(player_vel);
				/*//Angular velocity is insignificant for now
				float delta_t = Time.deltaTime;
				Quaternion end_rotation = Quaternion.RotateTowards(transform.rotation, player_aim, player_turn_rate*delta_t);
				float delta_angle = RotationDeltaAngle(transform.rotation, end_rotation);
				gun_ctrl.FireWithAngular(player_vel, transform.position, delta_angle, delta_t);*/
			}
		}
		//Stuff you don't need to be conscious for
		//Switching Weapons
		if ( Input.GetButtonDown("WeapNext") || Input.GetAxisRaw("Mouse ScrollWheel") > 0 )
		{	NextWeapon();
		}
		else if ( Input.GetButtonDown("WeapPrev") || Input.GetAxisRaw("Mouse ScrollWheel") < 0 )
		{	PrevWeapon();
		}
		else if ( Input.GetButtonDown("Weap1") )
		{	SwitchWeapon(0);
		}
		else if ( Input.GetButtonDown("Weap2") )
		{	SwitchWeapon(1);
		}
		else if ( Input.GetButtonDown("Weap3") )
		{	SwitchWeapon(2);
		}
		else if ( Input.GetButtonDown("Weap4") )
		{	SwitchWeapon(3);
		}
		else if ( Input.GetButtonDown("Weap5") )
		{	SwitchWeapon(4);
		}
		else if ( Input.GetButtonDown("Weap6") )
		{	SwitchWeapon(5);
		}
		else if ( Input.GetButtonDown("Weap7") )
		{	SwitchWeapon(6);
		}
		else if ( Input.GetButtonDown("Weap8") )
		{	SwitchWeapon(7);
		}
		else if ( Input.GetButtonDown("Weap9") )
		{	SwitchWeapon(8);
		}
		else if ( Input.GetButtonDown("Weap10") )
		{	SwitchWeapon(9);
		}
	}

	/*private float RotationDeltaAngle(Quaternion before, Quaternion after) //Signed angle between two rotations (in degrees)
	{	Vector3 vect_a = after * Vector3.forward;
		Vector3 vect_b = before * Vector3.forward;
		float angle_a = Mathf.Atan2(vect_a.x, vect_a.z) * Mathf.Rad2Deg;
		float angle_b = Mathf.Atan2(vect_b.x, vect_b.z) * Mathf.Rad2Deg;
		return Mathf.DeltaAngle(angle_a, angle_b);
	}*/
}
