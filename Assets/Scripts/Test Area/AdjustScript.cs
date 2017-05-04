//This script was used to create easily adjustable variables for testing the Save() and Load() methods. 
//This script can be deleted safely and does not belong in the final build. 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustScript : MonoBehaviour {
	void OnGUI()
	{
		if(GUI.Button(new Rect(10, 100, 100, 30), "Health up"))
		{
			GameControl.data.health += 10;
		}
		if(GUI.Button(new Rect(10, 140, 100, 30), "Experience up"))
		{
			GameControl.data.experience += 100;
		}
		if(GUI.Button(new Rect(10, 180, 100, 30), "Health down"))
		{
			GameControl.data.health -= 10;
		}
		if(GUI.Button(new Rect(10, 220, 100, 30), "Experience down"))
		{
			GameControl.data.experience -= 100;
		}
		if(GUI.Button(new Rect(10, 260, 100, 30), "Save"))
		{
			GameControl.data.Save();
		}
		if(GUI.Button(new Rect(10, 300, 100, 30), "Load"))
		{
			GameControl.data.Load();
		}
	}
}
