//Original Author: Alexander Stamatis || Last Edited: Alexander Stamatis | Modified on February 8, 2017
//Camera behavior, position camera anchor to players position and clamping camera rotation

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour {

	private GameObject player;
	private float temp_axis;

	// Use this for initialization
	void Awake () {
		player = GameObject.Find ("Player");
	}

	// Update is called once per frame
	void Update () {
		transform.position = player.transform.position;

		//Clamping x axis
		if (transform.rotation.eulerAngles.x < 60) {
			temp_axis = transform.rotation.eulerAngles.x;
		}

		if (transform.rotation.eulerAngles.x > 330) {
			temp_axis = transform.rotation.eulerAngles.x;
		}

		transform.rotation = Quaternion.Euler (temp_axis, transform.rotation.eulerAngles.y, 0);
	}
}