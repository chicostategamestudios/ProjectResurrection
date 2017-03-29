using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boosters : MonoBehaviour {

	[Tooltip("Adjust this to affect the thrust of the player, recommended value 5-30")]
	public float force;

	void Start () {
		if (this.gameObject.tag == "Launch Pad") { 
			if (force == 0) {
				force = 40.5f;
			}
		} else {
			if (force == 0) {
				force = 19.5f;
			}
		}
		force *= 10000;
	}
		
}
