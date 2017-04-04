//Original Author: Alexander Stamatis || Last Edited: Tony Alessio | Modified on March 30, 2017
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

public class CameraBehavior : MonoBehaviour
{
    private float Distance_To_Enemy = 0;                    //This is the distance between the player and the locked-on enemy
    private float maximum_lockon_distance = 30;             //This is the maximum distance the player can be from the enemy and remain locked on

    private bool Found_An_Enemy = false;                    //This is a bool that tracks if "Lock-On" found an enemy or not

    private GameObject player;                              //Holds a game object for referencing the "player"
    private GameObject targeted_enemy;                      //Holds a game object for referencing the "enemy"

    private bool Combat_Camera_Toggle = true;               //Boolean toggle for if the "camera" is in default mode, or combat mode
                                                            /* This starts as true, but in Awake() will be set to false (for testing reasons) */

    private float temp_axis;
    private GameObject enemy;

    // Use this for initialization
    void Awake()
    {
        player = GameObject.Find("Player");             //Assigns the "player" GameObject to an object in the hierarchy named "Player"
        targeted_enemy = GameObject.Find("Enemy");          //Assigns the "targeted_enemy" GameObject to an object in the hierarchy named "Enemy"

        //		Camera_Combat_Handler_Holder = GameObject.Find ("Camera_Combat_Handler");
        Combat_Camera_Toggle = false;
        /* PROGRAMMER NOTE: This is primarily for testing purposes.
		* The combat camera turns on by default, and then this line turns it off to make sure they both work
		*/
    }

    void Start()
    {
        enemy = GameObject.Find("Enemy");       // <<< This is how you will pick which enemy to lock on to. FOR NOW, JUST A TEST
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position;     //This update's the current tranform.position of Camera Anchor to the player's position
        Camera_Updater();           //This is a function call that will decide if lock-on is toggled on/off & combat is engaged/disengaged
        Return_Distance();          //This is a function call that will check for distance between player and potential lock-on enemy

        /* Did the player click Right Stick down? (Try to Lock-On) */
        if (Input.GetButtonDown("Controller_Right_Stick_Click"))        //The player clicked the Right Stick in, BEGIN Lock-On Checks
        {
            Debug.Log("Controller Right Stick Click: Pressed");
            RaycastHit hit;
            //			Physics.SphereCast(Vector3 ORIGIN, float RADIUS, Vector3 DIRECTION, out RaycastHit hitInfo, float MAXDISTANCE);		/*DEBUGGING: Might need a layerMask to ignore the ground*/
            /* This next line is used to check for an available target in front of player character */
            Found_An_Enemy = Physics.SphereCast(/*ORIGIN*/ transform.position, /*RADIUS*/ 5, /*DIRECTION*/ transform.forward, out hit, /*MAX DISTANCE*/ 30);
            //			Debug.DrawRay (transform.position, transform.forward, Color.green);			//FOR DEBUGGING. CAN REMOVE LATER
            //			Debug.Log("'Found_An_Enemy' is: " + Found_An_Enemy);						//FOR DEBUGGING. CAN REMOVE LATER

            if (Found_An_Enemy)
            {
                if (Combat_Camera_Toggle == true) { Combat_Camera_Toggle = false; }
                else if (Combat_Camera_Toggle == false) { Combat_Camera_Toggle = true; }
                else { Debug.LogError("Error in 'Combat_Camera_Toggle' logic. You should not see this message."); }
            }
        }

        //////////////////////////////////////////////////////////////ALEX CODE//////////////////////////////////////////////////////////////
        //Clamping x axis
        if (transform.rotation.eulerAngles.x < 60)
        {
            temp_axis = transform.rotation.eulerAngles.x;
        }

        if (transform.rotation.eulerAngles.x > 330)
        {
            temp_axis = transform.rotation.eulerAngles.x;
        }

        transform.rotation = Quaternion.Euler(temp_axis, transform.rotation.eulerAngles.y, 0);
        //////////////////////////////////////////////////////////////ALEX CODE//////////////////////////////////////////////////////////////

    }

    void Return_Distance()
    {
        //		Debug.Log ("'Distance_To_Enemy' is: " + Distance_To_Enemy);										//Display the distance between the player and the enemy
        if (Distance_To_Enemy > maximum_lockon_distance) { Combat_Camera_Toggle = false; }                  //Force player Lock-Off if player moves too far from enemy
        float temp_dist = Vector3.Distance(player.transform.position, enemy.transform.position);        //Calculate distance between player and enemy
        Distance_To_Enemy = temp_dist;                                                                  //Set the Distance_To_Enemy to the calculation solution
    }

    void Camera_Updater()
    {
        //If the player's camera is NOT in Combat Mode
        if (Combat_Camera_Toggle == false)
        {
            /* This is when the camera is set back to normal mode */
        }
        //If the player's camera IS in Combat Mode
        else if (Combat_Camera_Toggle == true)
        {
            /* This is when the camera is set to combat mode */
            transform.position = player.transform.position;             //This update's the current tranform.position of Camera Anchor to the player's position
            transform.LookAt(enemy.transform);                          //Forces the Camera Anchor, and camera, to face the enemy while "locked-on"
        }
    }
}
