//This script was written by Tony Alessio | Last edited by Tony Alessio | Modified on Mar 2, 2017
/*
SCRIPT DESCRIPTION
	The functionality of this script is to get the UI representation of the button linked with
the function call for that specific button. This script can be placed on any object in the 
scene, but one has been provided named "Options_Menu_Functionality."
	Once attached to an object, click that object and you will then be able to attach the UI 
elements to their respective fields:
- BACK Button -> [BACK (Button)]

	Once the editor knows which button is associated with its UI element, the player will be 
able to click the button to activate the function call. Most function calls simply change the 
current scene to the scene displayed on the UI button.
*/

//Libraries
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Options_UI_Functionality : MonoBehaviour {
	//Member Variables
	/* This section will allow UI elements to be dragged and dropped in the inspector to assign UI elements to functions */
	[Tooltip("Drag and drop the 'BACK' UI element into this field")]
	public Button BACKButton;		//This will be used by the inspector to dictate which button is the "BACK" button

	void Start () {
		Button BackButton = BACKButton.GetComponent<Button>();			//Assigns the UI element to its script counterpart
		BackButton.onClick.AddListener(BackOnClick);					//Back Script
	}



	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//	Back - Back Button Functionality (BEGIN)
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/* This function will allow the player to click on the Back button and move to the "main_menu" scene */
	void BackOnClick()
	{
		Main_Menu_Functionality.Singleton_Main_Menu_Functionality.Options_Menu_Obj.SetActive (false);		//Sets Options_Menu to become ingvisible
		Main_Menu_Functionality.Singleton_Main_Menu_Functionality.Start_Menu_Obj.SetActive (true);			//Sets start_menu to become visible
		Main_Menu_Functionality.Singleton_Main_Menu_Functionality.optionsButton.Select();					//Sets the Options button as the active cursor
	}
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//	Back - Back Button Functionality (END)
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



	//	"camera_recenter" is the variable name in the "PlayerGamepad.cs" script that allows toggling on and off of camera recenter



}
