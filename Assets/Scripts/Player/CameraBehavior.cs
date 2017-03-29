//Original Author: Alexander Stamatis || Last Edited: Tony Alessio | Modified on March 21, 2017
//Camera behavior, position camera anchor to players position and clamping camera rotation
//
//
//
//
//
//
//
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour {
		private bool In_Combat_Currently = false;		//This is a bool that tracks if the player is in combat or not

		private GameObject Camera_Combat_Handler_Holder;		//Holds a game object for referencing the center point of the combat camera
		private GameObject player;					//Holds a game object for referencing the "player"
		private GameObject targeted_enemy;						//Holds a game object for referencing the "enemy"
		public Vector3 camera_movement_direction;

		/* PROGRAMMER NOTE: This should be changed to PUBLIC while testing. It should be set to PRIVATE if testing is comeplete. */
		[Tooltip("IF YOU ARE SEEING THIS, THEN A PROGRAMMER DID NO FOLLOW DIRECTIONS")]
		public  bool Combat_Camera_Toggle = true;				/* TESTING ONLY */
		//	private bool Combat_Camera_Toggle = true;				//Boolean toggle for if the "camera" is in default mode, or combat mode
		/* This starts as true, but in Awake() will be set to false (for testing reasons) */

		//Inspector Cameras
		//public Camera Camera_Normal;			//This is an inspector element that will hold the default (non-combat) camera
		//public Camera Camera_Combat;			//This is an inspector element that will hold the combat camera
		//public Camera Camera_Cinematic;		//This is an inspector element that will hold the cinematic camera


	private float temp_axis;
	private GameObject enemy;

	// Use this for initialization
	void Awake () {
			player = GameObject.Find ("Player");		//Assigns the "player" GameObject to an object in the hierarchy named "Player"
			targeted_enemy = GameObject.Find ("Enemy");			//Assigns the "targeted_enemy" GameObject to an object in the hierarchy named "Enemy"

			Camera_Combat_Handler_Holder = GameObject.Find ("Camera_Combat_Handler");

			Combat_Camera_Toggle = false;
			/* PROGRAMMER NOTE: This is primarily for testing purposes. 
		    * The combat camera turns on by default, and then this line turns it off to make sure they both work */
	}

	void Start () {
		enemy = GameObject.Find ("Enemy");
	}

	// Update is called once per frame
	void Update () {
		transform.position = player.transform.position;		//This update's the current tranform.position of Camera Anchor to the player's position
		Camera_Updater ();			//This is a function call that will decide if lock-on is toggled on/off & combat is engaged/disengaged
		/* Did the player click Right Stick down? */
		if (Input.GetButtonDown ("Controller_Right_Stick_Click") )
		{
			Debug.Log ("Controller Right Stick Click: Pressed");
			if (Combat_Camera_Toggle == true){Combat_Camera_Toggle = false;}
			else if (Combat_Camera_Toggle == false){Combat_Camera_Toggle = true;}
		}

		//////////////////////////////////////////////////////////////ALEX CODE//////////////////////////////////////////////////////////////
		//Clamping x axis
		if (transform.rotation.eulerAngles.x < 60) {
			temp_axis = transform.rotation.eulerAngles.x;
		}

		if (transform.rotation.eulerAngles.x > 330) {
			temp_axis = transform.rotation.eulerAngles.x;
		}

		transform.rotation = Quaternion.Euler (temp_axis, transform.rotation.eulerAngles.y, 0);
		//////////////////////////////////////////////////////////////ALEX CODE//////////////////////////////////////////////////////////////

	}

	void Camera_Updater (){
		//If the player's camera is NOT in Combat Mode
		if (Combat_Camera_Toggle == false) {
			In_Combat_Currently = false;
			//Camera_Normal.transform.LookAt (targeted_enemy.transform);
		}
		//If the player's camera IS in Combat Mode
		else if (Combat_Camera_Toggle == true) {
			In_Combat_Currently = true;

			transform.position = player.transform.position;		//This update's the current tranform.position of Camera Anchor to the player's position
			//We need a bool loop to turn on/off this when lock on in turned on/off
			transform.LookAt (enemy.transform);
			//END
		}
	}
}