//GolemRL Player Object
using UnityEngine;

public class PlayerController : MonoBehaviour
{	public double player_speed = 100.0;
	
	void Update()
	{	double x = Input.GetAxis("Horizontal") * Time.deltaTime * player_speed;
		double z = Input.GetAxis("Vertical") * Time.deltaTime * player_speed;
		
		//transform.Rotate(0, 1, 0);
		transform.Translate(x, 0, z);
	}
}
