//GolemRL Gun Object Script
using UnityEngine;

public class GunController : MonoBehaviour
{	public GameObject gun_projectile; //Type of bullet
	public GameObject[] hideable_models; //Objects to hide if hide_on_deselect
	public GameObject[] muzzle_points; //Objects representing points where bullets appear
	public bool hide_on_deselect = false; //Gun doesn't render while deselected
	public bool muzzle_in_order = false; //Muzzles take turns, else fire all at once
	public int active_muzzle = 0; //Next muzzle to fire if muzzle_in_order
	public int ammo_count = 0; //Bullets remaining
	public int ammo_cost = 0; //Cost per firing
	public int ammo_max = 0; //Max ammo count (0 = No limit)
	public int shotgun_pellets = 1; //Projectiles per muzzle
	public float fire_cooldown = 0.5f; //Seconds between firing
	public float muzzle_velocity = 5.0f; //Base speed for projectile
	public float spread_deg = 0.0f; //Maximum spread of projectiles (degrees from center)
	public float speed_mult = 1.0f; //Projectile speed multiplier
	public float rof_mult = 1.0f; //Rate of fire multiplier
	public float damage_mult = 1.0f; //Damage multiplier

	private int muzzle_count; //Number of muzzle points
	private float fire_counter = 0.0f; //Time until next shot
	private Transform[] muzzle_tf; //Points where bullets appear
	private GameObject gun_owner; //Unit holding gun
	private PlayerController player_owner; //Script of player holding gun, else null

    public AudioSource gunSoundBad;
    public Npc_Health TempAudio;

    public bool isDead;

    void Start()
	{	gun_owner = transform.parent.gameObject;
		muzzle_count = muzzle_points.Length;
		muzzle_tf = new Transform[muzzle_count];
		for (int i = 0; i < muzzle_count; i++)
		{	muzzle_tf[i] = muzzle_points[i].GetComponent<Transform>();
		}
		if (gun_owner.tag == "Player")
		{	player_owner = gun_owner.GetComponent<PlayerController>();
		}
	}

	void Update()
	{
        if(!isDead)
        {
            float rof_mod = 1.0f;
            if (player_owner != null)
            {
                rof_mod = player_owner.player_gun_rof_mult;
            }
            if (fire_counter > 0.0f)
            {
                fire_counter -= Time.deltaTime * rof_mult * rof_mod;
            }
        }
        
	}

	public void Fire(Vector3 player_velocity)
	{	if ( fire_counter <= 0.0f && ammo_count >= ammo_cost )
		{	fire_counter = fire_cooldown; //Reset cooldown
			ammo_count -= ammo_cost; //Subtract ammo
			if (muzzle_in_order) //Single bullet
			{	for (int pellet = 0; pellet < shotgun_pellets; pellet++)
				{	Spawn_Projectile(player_velocity, active_muzzle);
				}
				active_muzzle = (active_muzzle+1)%muzzle_count; //Switch to next muzzle
			}
			else //Multiple bullets
			{	for (int i = 0; i < muzzle_count; i++)
				{	for (int pellet = 0; pellet < shotgun_pellets; pellet++)
					{	Spawn_Projectile(player_velocity, i);
					}
				}
			}
		}
	}

	public bool FireReady()
	{
        
        return (fire_counter <= 0.0f) && (ammo_count >= ammo_cost);
	}

	public void Deselect() //Switch away from this weapon
	{	if (hide_on_deselect)
		{	Hide();
		}
	}

	public void Select(float delay) //Switch to this weapon
	{	active_muzzle = 0;
		if (hide_on_deselect)
		{	Unhide();
		}
		if ( fire_counter < delay ) //Can't shoot until animation finishes
		{	fire_counter = delay;
		}
	}

	public void Hide() //Don't render object or children
	{	Renderer rend = GetComponent<Renderer>();
		if (rend != null)
		{	rend.enabled = false;
		}
		Renderer[] child_renderers = GetComponentsInChildren<Renderer>();
		foreach( Renderer r in child_renderers )
		{	r.enabled = false;
		}
		foreach( GameObject model in hideable_models )
		{	rend = model.GetComponent<Renderer>();
			if (rend != null)
			{	rend.enabled = false;
			}
			child_renderers = model.GetComponentsInChildren<Renderer>();
			foreach( Renderer r in child_renderers )
			{	r.enabled = false;
			}
		}
	}

	public void Unhide() //Render object and children
	{	Renderer rend = GetComponent<Renderer>();
		if (rend != null)
		{	rend.enabled = true;
		}
		Renderer[] child_renderers = GetComponentsInChildren<Renderer>();
		foreach( Renderer r in child_renderers )
		{	r.enabled = true;
		}
		foreach( GameObject model in hideable_models )
		{	rend = model.GetComponent<Renderer>();
			if (rend != null)
			{	rend.enabled = true;
			}
			child_renderers = model.GetComponentsInChildren<Renderer>();
			foreach( Renderer r in child_renderers )
			{	r.enabled = true;
			}
		}
	}

	private void Spawn_Projectile(Vector3 inherited_velocity, int muzzle_num)
	{	float dmg_mod = 1.0f;
		float vel_mod = 1.0f;
		float spread; //Random angle for bullet
		if (player_owner != null)
		{	dmg_mod = player_owner.player_damage_mult;
			vel_mod = player_owner.player_gun_speed_mult;
		}
		GameObject bullet = Instantiate(gun_projectile, muzzle_tf[muzzle_num].position, muzzle_tf[muzzle_num].rotation);
       //
		BulletController bullet_ctrl = bullet.GetComponent<BulletController>();
		bullet_ctrl.owner = gun_owner;
		bullet_ctrl.damage *= damage_mult * dmg_mod;
		Vector3 trajectory = muzzle_tf[muzzle_num].forward; //Firing vector subject to spread
		if (spread_deg > 0.0f)
		{	spread = Random.Range(-spread_deg,spread_deg); //Random angle in spread arc
			trajectory = Quaternion.AngleAxis(spread, muzzle_tf[muzzle_num].up) * trajectory; //Rotate trajectory along y-axis
			Debug.DrawRay(muzzle_tf[muzzle_num].position,Quaternion.AngleAxis(-spread_deg, muzzle_tf[muzzle_num].up) * muzzle_tf[muzzle_num].forward*5, Color.blue,fire_cooldown/rof_mult);
			Debug.DrawRay(muzzle_tf[muzzle_num].position,Quaternion.AngleAxis(spread_deg, muzzle_tf[muzzle_num].up) * muzzle_tf[muzzle_num].forward*5, Color.blue,fire_cooldown/rof_mult);
		}
		Debug.DrawRay(muzzle_tf[muzzle_num].position,trajectory*5, Color.magenta,fire_cooldown/rof_mult);
		bullet_ctrl.bullet_vel = inherited_velocity + muzzle_velocity*speed_mult*vel_mod*trajectory;
        
    }
	//Account for angular velocity. This ends up being only ~3 units/sec at distance 0.75 from center at 180 deg/sec.
	//Could be useful at larger distances from center of rotation.
	/*public void FireWithAngular(Vector3 player_velocity, Vector3 player_position, float delta_angle, float delta_t)
	{	Vector3 muzzle_offset;
		float angular_velocity;
		if (fire_counter <= 0.0f)
		{	fire_counter = fire_cooldown;
			muzzle_offset = muzzle_tf.position - player_position;
			angular_velocity = muzzle_offset.magnitude * delta_angle * Mathf.Deg2Rad / delta_t;
			GameObject bullet = Instantiate(gun_projectile, muzzle_tf.position, muzzle_tf.rotation);
			BulletController bullet_ctrl = bullet.GetComponent<BulletController>();
			bullet_ctrl.bullet_vel = player_velocity + angular_velocity*muzzle_tf.forward + muzzle_velocity*speed_mult*muzzle_tf.forward;
		}
	}*/
}
