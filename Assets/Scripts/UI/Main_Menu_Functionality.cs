//This script was written by Tony Alessio | Last edited by Tony Alessio | Modified on Mar 2, 2017
/*
SCRIPT DESCRIPTION
	The functionality of this script is to get the UI representation of the button linked with
the function call for that specific button. This script can be placed on any object in the 
scene, but one has been provided named "Main_Menu_Functionality."
	Once attached to an object, click that object and you will then be able to attach the UI 
elements to their respective fields:
- Start_menu_obj -> [Start_Menu]
- PLAY Button -> [PLAY (Button)]
- Play_menu_obj -> [Play_Menu)]
- Options Button -> [Options (Button)]
- Options_menu_obj -> [Options_Menu]
- Exit Button -> [Exit (Button)]
- Quit_menu_obj -> [Quit_BG]
- Quit_yes_Button -> [Quit_Yes (Button)]
- Quit_no_button -> [Quit_No (Button)]

	Once the editor knows which button is associated with its UI element, the player will be 
able to click the button to activate the function call. Most function calls simply change the 
current scene to the scene displayed on the UI button.
*/

//Libraries
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Main_Menu_Functionality : MonoBehaviour {
	//Member Variables
	/* This section will allow UI elements to be dragged and dropped in the inspector to assign UI elements to functions */

	[Tooltip("Drag and drop the 'Start_Menu' UI element into this field")]
	//This gameobject will be used to hold the Start_Menu canvas object so that it can be turned on and off
	public GameObject start_menu_obj;		//This will be used by the inspector to dictate which UI element is the "Start_Menu" parent
											//Holding this gameobject variable is necessary in order to hide the "Start_Menu" when any other button is selected
		public GameObject Start_Menu_Obj{ get{ return start_menu_obj;} }		//Getter/Setter for start_menu_obj

	[Tooltip("Drag and drop the 'PLAY' UI element into this field")]
	public Button PLAYButton;       //This will be used by the inspector to dictate which button is the "Play" button
		[Tooltip("Drag and drop the 'Play_Menu' UI element into this field")]
		//This gameobject will be used to hold the Play_Menu canvas object so that it can be turned on and off
		public GameObject play_menu_obj;		//This will be used by the inspector to dictate which UI element is the "Play_Menu" parent
												//Holding this gameobject variable is necessary in order to hide the "Play_Menu" until the PLAY Button is selected

    [Tooltip("Drag and drop the 'Options' UI element into this field")]
	public Button optionsButton;        //This will be used by the inspector to dictate which button is the "Options" button
		[Tooltip("Drag and drop the 'Play_Menu' UI element into this field")]
		//This gameobject will be used to hold the Options_Menu canvas object so that it can be turned on and off
		public GameObject options_menu_obj;		//This will be used by the inspector to dictate which UI element is the "Options_Menu" parent object
												//Holding this gameobject variable is necessary in order to hide the "Options_Menu" until the Options Button is selected
			public GameObject Options_Menu_Obj{ get{ return options_menu_obj;} }		//Getter/Setter for options_menu_obj


    [Tooltip("Drag and drop the 'Exit' UI element into this field")]
	public Button exitButton;		//This will be used by the inspector to dictate which button is the "Exit" button
		[Tooltip("Drag and drop the 'Quit_Menu' UI element into this field")]
		public GameObject quit_menu_obj;		//This will be used by the inspector to dictate which UI element is the "Quit_Menu" parent
												//Holding this gameobject variable is necessary in order to hide the Quit Menu until the Quit Button is selected
		[Tooltip("Drag and drop the 'Quit_Yes' UI element into this field")]
		public Button quit_yes_Button;		//This will be used by the inspector to dictate which button is the "Yes" button, under the Quit parent
		[Tooltip("Drag and drop the 'Quit_No' UI element into this field")]
		public Button quit_no_Button;		//This will be used by the inspector to dictate which button is the "No" button, under the Quit parent

	static public Main_Menu_Functionality Singleton_Main_Menu_Functionality;

	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//	Awake() is being used for error checking of incorrectly attatched components in the inspector (BEGIN)
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	void Awake(){
		Singleton_Main_Menu_Functionality = this;
	
		if (quit_menu_obj == null) {	//Checks to see if quit_menu_obj was correctly attached in the inspector
			Debug.LogError ("The gameobject for 'quit_menu_obj' was not assigned before the game was started.");
			/* This would be for assigning the game object to the inspector in case it wasn't assigned correctly during edit mode */
			/*
			 * Currently this does not work, because the object starts out as inactive. 
			 * If you want to use the next line of code, then...
			 * You will need to set it to active in the inspector, 
			 * this code will assign it correctly in awake, 
			 * and then turn it off in start once the connection has been made.
			 */
			//quit_menu_obj = GameObject.Find("Quit_Menu");
			//GameObject.Find ("Play_Menu_Functionality");
		}
	}
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//	Awake() (END)
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



	void Start () {
		Button PlayButton = PLAYButton.GetComponent<Button>();                  //Assigns the UI element to its script counterpart
		Button OptionsButton = optionsButton.GetComponent<Button>();            //Assigns the UI element to its script counterpart
		Button ExitButton = exitButton.GetComponent<Button>();                  //Assigns the UI element to its script counterpart

		Button QuitYesButton = quit_yes_Button.GetComponent<Button>();          //Assigns the UI element to its script counterpart
		Button QuitNoButton = quit_no_Button.GetComponent<Button>();            //Assigns the UI element to its script counterpart

//		Toggle Camera_AutoRotate = options_menu_obj.GetComponentInChildren<Toggle>();

		PlayButton.onClick.AddListener(PlayOnClick);                    //Play Script
		OptionsButton.onClick.AddListener(OptionsOnClick);              //Options Script
		ExitButton.onClick.AddListener(QuitOnClick);                    //Quit Script

		QuitYesButton.onClick.AddListener(QuitYesOnClick);              //Quit - Yes Script
		QuitNoButton.onClick.AddListener(QuitNoOnClick);                //Quit - No Script
	}



	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//	PLAY - Play Button Functionality (BEGIN)
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/* This function will allow the player to click on the Play button and trigger the "Play_Menu" object */
	void PlayOnClick()
    {
		//SceneManager.LoadScene("play_game");		//DEPRICATED: Use to use seperate scenes to load UI menus. Feel free to delete
		play_menu_obj.SetActive(true);			//Sets play_menu to become visible
		start_menu_obj.SetActive(false);		//Sets start_menu to become invisible
		play_menu_obj.transform.parent.GetComponent<Play_Game_UI_Functionality>().storyButton.Select();			//The "Story" button will be highlighted
		/* This allows the parent of play_menu_obj, which is Play_Menu_Functionality, 
		 * to be called and then the component that is the storyButton within the 
		 * Play_Game_UI_Functionality script can be accessed.
		 */
	}
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//	PLAY - Play Button Functionality (END)
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//	Options - Options Button Functionality (BEGIN)
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/* This function will allow the player to click on the Options button and move to the "options_menu" scene */
	void OptionsOnClick()
    {
		//SceneManager.LoadScene("options_menu");		//DEPRICATED: Use to use seperate scenes to load UI menus. Feel free to delete

		/* This will make the Options_Menu appear */
		options_menu_obj.SetActive(true);			//Sets Options_Menu to become visible
		start_menu_obj.SetActive(false);			//Sets start_menu to become invisible
		options_menu_obj.transform.parent.GetComponent<Options_UI_Functionality>().BACKButton.Select();
		/*
		 * This allows the parent of options_menu_obj, which is Options_Menu_Functionality, 
		 * to be called and then the component that is the BACKButton within the 
		 * Options_UI_Functionality script can be accessed.
		 */
	}
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//	Options - Options Button Functionality (END)
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//	Exit - Quit Button Functionality (BEGIN)
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/* This function will allow the player to click on the Quit button and will create a confirmation prompt */
	void QuitOnClick()
    {
		/* This will make the Quit_Menu appear */
		quit_menu_obj.SetActive(true);			//Sets quit_menu to become visible
		start_menu_obj.SetActive(false);		//Sets start_menu to become invisible
		quit_no_Button.Select ();				//Sets the focus of the cursor to Quit's "No" button
	}

	void QuitYesOnClick()
    {
		/* This will make the game Quit out */
		Application.Quit ();
	}

	void QuitNoOnClick()
    {
		/* This will make the game Quit menu disappear */
		quit_menu_obj.SetActive(false);			//Sets quit_menu to become invisible
		start_menu_obj.SetActive(true);			//Sets start_menu to become visible
		exitButton.Select ();					//Sets the focus of the cursor to "Exit" button
	}
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//	Exit - Quit Button Functionality (END)
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
