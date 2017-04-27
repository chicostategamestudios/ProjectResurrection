//This script holds game variables. This script can save and load these variables to a file. 
//When you want to use variables from othis script, call them using:
	//	GameControl.data.relevent_variable
	//where relevent_varaible is the variable you want to use or change. This can be done from any script. 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameControl : MonoBehaviour {

	public static GameControl data;

	//Persistent Variables Section
	//These Variables are saved on Save() and loaded on Load()
	//MAKE SURE EVERY VARIABLE HERE IS ALSO SAVED IN THE SAVE FUNCTION AND LOADED IN THE LOAD FUNCTION!!!
	//IN ADDITION, ALL PERSISTENT VARIABLES SHOULD ALSO BE IN THE PLAYERDATA CLASS
	[Tooltip("Health doesn't do anything currently. If this is no longer the case, update this tooltip.")]
	public float health; //health was used as a variable to test saving and loading. You can delete all references of it or use it; I don't care. Please change this comment if you use it. 
	[Tooltip("Experience doesn't do anything currently. If this is no longer the case, update this tooltip.")]
	public float experience; //experience was used as a varaible to test saving and loading. You can delete all references of it or use it; I don't care. Please change this comment if you use it. 
	[Tooltip("The level the player is currently on")]
	public string current_level; //The level the player is currently on.
	[Tooltip("The player's score.")]
	public int score; //The player's score. 
	[Tooltip("The last checkpoint the player used.")]
	public int checkpoint; //The last checkpoint the player used. 

	//These variables are used for the ExampleData class. They are not public because you should never use them. 
	float example_float;
	int example_int;
	string example_string;
	ExampleClass example_class;


	void Awake() {
		if (data == null) {
			DontDestroyOnLoad (gameObject);
			data = this;
		} else if (data != this) {
			Destroy (gameObject);
		}
	}
		
	/*
	//This function was used for testing purposes during the creation of this script.
	//This function can be safely deleted. 
	void OnGUI()
	{
		GUI.Label(new Rect(10, 10, 100, 30), "Health: " + health);
		GUI.Label(new Rect(10, 50, 100, 30), "Experience: " + experience);
	}
	*/

	//Load on start
	void OnEnable()
	{
		Load();
	}

	//Save on exit
	void OnDisable()
	{
		Save ();
	}

	//Call every save function; Save everything. 
	public void Save()
	{
		SavePlayer ();
		SaveExample ();
	}

	//Save all variables in the PlayerData class to a serialized binary file.
	void SavePlayer()
	{
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/playerdata.dat");

		PlayerData save = new PlayerData ();
		//MAKE SURE EVERY VARIABLE IN THE PLAYERDATA CLASS IS LISTED HERE AND IN THE RELATED LOAD FUNCTION!!
		save.health = health;
		save.experience = experience;
		save.current_level = current_level;
		save.score = score;
		save.checkpoint = checkpoint;

		bf.Serialize (file, save);
		file.Close ();
	}

	//Use this as an example for creating new save functions. 
	void SaveExample()
	{
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/exampledata.dat");

		ExampleData save = new ExampleData ();
		//MAKE SURE EVERY VARIABLE IN THE EXAMPLEDATA CLASS IS LISTED HERE AND IN THE RELATED LOAD FUNCTION!!
		save.example_float = example_float;
		save.example_int = example_int;
		save.example_string = example_string;
		save.example_class = example_class;

		bf.Serialize (file, save);
		file.Close ();
	}


	//Call every load function; Load everything.
	public void Load()
	{
		LoadPlayer ();
		LoadExample ();
	}

	//Load all variables in the PlayerData class from a serialized binary file. 
	void LoadPlayer()
	{
		if (File.Exists (Application.persistentDataPath + "/playerdata.dat")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/playerdata.dat", FileMode.Open);
			PlayerData load = (PlayerData)bf.Deserialize (file);
			file.Close ();
			//MAKE SURE EVERY VARIABLE IN THE PLAYERDATA CLASS IS LISTED HERE AND IN THE RELATED SAVE FUNCTION!!
			health = load.health;
			experience = load.experience;
			current_level = load.current_level;
			score = load.score;
			checkpoint = load.checkpoint;
		}
	}

	//Use this as a template for creating new Load functions.
	void LoadExample()
	{
		if (File.Exists (Application.persistentDataPath + "/exampledata.dat")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/exampledata.dat", FileMode.Open);
			ExampleData load = (ExampleData)bf.Deserialize (file);
			file.Close ();
			//MAKE SURE EVERY VARIABLE IN THE EXAMPLEDATA CLASS IS LISTED HERE AND IN THE RELATED SAVE FUNCTION!!
			example_float = load.example_float;
			example_int = load.example_int;
			example_string = load.example_string;
			example_class = load.example_class;
		}
	}



	//This class contains all the Player Data. Put any persistent variables relevent to the player here. 
	//IF YOU ADD A NEW VARIABLE HERE, ALSO ADD IT TO THE SAVEPLAYER() AND LOADPLAYER() FUNCTIONS!!!
	//If you want to store persistent variables that do not relate to the player, you should create a 
	//new class for it, and create Save and Load functions to save/load that class as well. Use the
	//example classes and functions as a template. 
	[Serializable]
	class PlayerData
	{
		public float health;
		public float experience;
		public string current_level;
		public int score;
		public int checkpoint;
	}
		
	//Use this as a template for creating new classes for data. 
	[Serializable]
	class ExampleData
	{
		public float example_float;
		public int example_int;
		public string example_string;
		public ExampleClass example_class;
	}
		
	//This is just to show you can also save classes, but only if you give the class the [Serializable] tag. 
	[Serializable]
	class ExampleClass
	{
		public bool example_bool;
	}
}
