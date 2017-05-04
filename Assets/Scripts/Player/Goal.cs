//Originally created by Brandon Borders May 4, 2017. Last updated May 4, 2017.
//Attach this file to the player. It will allow them to advance to the level indicated on touching the goal object. 
//Make sure that the Goal object is tagged "Goal". 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour {

	[Tooltip("Name of the level that the Goal takes you to.'")]
	public string NextLevel;

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag ("Goal")) 
		{
			SceneManager.LoadScene(NextLevel);
		}
	}
}
