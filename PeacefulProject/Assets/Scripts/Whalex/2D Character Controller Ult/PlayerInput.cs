using UnityEngine;
using System.Collections;

namespace Whalex
{
	// raw input -> velocity -> movement
	// flat ground(stand, straight jump, walk) / steep slope (slide, jump along normal, shift walk) / on air (fall, no jump, no walk)
	[RequireComponent (typeof (Player))]
	public class PlayerInput : MonoBehaviour {

		Player player;

		void Start () {
			player = GetComponent<Player> ();
		}

		void Update () {
			Vector2 directionalInput = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
			player.SetDirectionalInput (directionalInput);

			if (Input.GetKeyDown (KeyCode.Space)) {
				player.OnJumpInputDown ();
			}
			if (Input.GetKeyUp (KeyCode.Space)) {
				player.OnJumpInputUp ();
			}
		}
	}

}

