using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Player))]
public class PlayerInput : MonoBehaviour {

	Player player;

	public bool isFrozen;
	
	void Start () {
		player = GetComponent<Player> ();
	}

	void Update () {
		if(isFrozen)
			return;
		Vector2 directionalInput = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		player.SetDirectionalInput (directionalInput);

		if (Input.GetKeyDown (KeyCode.Space)) {
			player.OnJumpInputDown ();
		}
		if (Input.GetKeyUp (KeyCode.Space)) {
			player.OnJumpInputUp ();
		}

		if (Input.GetKey(KeyCode.LeftShift) && !player.controller.isHolding)
		{
			player.controller.isTryingToClimb = true;
		}
		else
		{
			player.controller.isTryingToClimb = false;
		}
		
	}
}
