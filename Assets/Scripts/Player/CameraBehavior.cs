// Original Author: Tony Alessio || Last Edited: Tony Alessio | Modified on May 2, 2017
// Camera behavior, position camera anchor to players position and clamping camera rotation
// This script also contains the lock-on behavior for combat lock-on / lock-off
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

    RaycastHit hitInfo;

    /* These variables are used for lock-on */
    private bool Found_An_Enemy = false;                    //This is a bool that tracks if "Lock-On" found an enemy or not
    private float Distance_To_Enemy = 0;                    //This is the distance between the player and the locked-on enemy
    private float maximum_lockon_distance = 30;             //This is the maximum distance the player can be from the enemy and remain locked on
    private float maximum_lockon_radius = 15;               //This is the radius of the circle cast out of the circleCast to detect enemies
                                                            //	private RaycastHit hit;

    //This is an array of enemies (GameObjects) that the player can lock-on to
    private GameObject[] targetable_enemies_arr;

    private GameObject player;                              //Holds a game object for referencing the "player"
    private GameObject targeted_enemy;                      //Holds a game object for referencing the "enemy"

    private bool Locked_On_To_Enemy = true;                 //Boolean toggle for if the "camera" is in default mode, or combat mode
                                                            /* This starts as true, but in Awake() will be set to false (for testing reasons) */

    private float temp_axis;



    void Awake()
    {
        /* These are used for initialization*/
        player = GameObject.Find("Player");                                 //Assigns the "player" GameObject to an object in the hierarchy named "Player"
        targeted_enemy = GameObject.FindGameObjectWithTag("Enemy");         //Assigns the "targeted_enemy" GameObject to an object in the hierarchy tagged with "Enemy"

        //		Camera_Combat_Handler_Holder = GameObject.Find ("Camera_Combat_Handler");
        Locked_On_To_Enemy = false;
        /* PROGRAMMER NOTE: This is primarily for testing purposes.
		*  The combat camera turns on by default, and then this line turns it off to make sure both states work correctly.
		*/
    }



    void Start()
    {
        GatherAllEnemies();             //This function will gather all the enemies (GameObjects) present in the level in order to determine lock-on priority
        DisplayTargetsInArray();        //This function will display all the enemies (elements) of the array ( targetable_enemies_arr[i] )
    }



    void ScanRight()
    {
        /* ALL THIS CODE WILL NEED TO BE WRAPPED IN A CHECK (Is the player already locked on to something?) */
        Vector3 forward_dir = transform.TransformDirection(Vector3.forward) * maximum_lockon_distance;
        Debug.DrawRay(transform.position, forward_dir, Color.green);

        Vector3 t_e_value = transform.TransformDirection(targeted_enemy.transform.right) * maximum_lockon_distance;
        // Physics.SphereCast(Vector3 ORIGIN, float RADIUS, Vector3 DIRECTION, out RaycastHit hitInfo, float MAXDISTANCE);
        /* This next line is used to check for an available target in front of player character */
        //Found_An_Enemy = Physics.SphereCast (transform.position, maximum_lockon_radius, transform.forward, out hit, maximum_lockon_distance);
        //if (hit.collider.tag == "Enemy") {Debug.Log ("ENEMY HIT!");}				//FOR DEBUGGING. CAN REMOVE LATER
        //Debug.DrawRay (transform.position, transform.forward, Color.green);			//FOR DEBUGGING. CAN REMOVE LATER
        //Debug.Log ("'Found_An_Enemy' is: " + Found_An_Enemy);						//FOR DEBUGGING. CAN REMOVE LATER
        Physics.SphereCast(targeted_enemy.transform.position, maximum_lockon_radius, t_e_value, out hitInfo, maximum_lockon_distance);
        Debug.Log("THIS WAS HIT: " + hitInfo);

        //		if (Found_An_Enemy)
        if (hitInfo.collider.tag == "Enemy")
        {
            Debug.LogError("YOU TARGETED A NEW ENEMY!");
            //LockOn_Camera_Updater ();
        }

        Debug.DrawRay(targeted_enemy.transform.position, t_e_value, Color.green);
    }



    void FixedUpdate()
    {
        transform.position = player.transform.position;         //This update's the current tranform.position of Camera Anchor to the player's position
        LockOn_Camera_Updater();                                //This is a function call that will decide if lock-on is toggled on/off & combat is engaged/disengaged
        Return_Distance();                                      //This is a function call that will check for distance between player and potential lock-on enemy


        /* If player moves right stick (more than halfway) to the right */
        if (Input.GetAxis("RightJoystickX") > 0.5f)
        {
            Debug.LogError("Received right stick X-axis input: RIGHT");
            // Call function that scans for target to the RIGHT
            ScanRight();
        }

        /* If player moves right stick (more than halfway) to the left */
        if (Input.GetAxis("RightJoystickX") < -0.5f)
        {
            Debug.LogError("Received right stick X-axis input: LEFT");
            // Call function that scans for target to the LEFT
        }



        /* Did the player click Right Stick down? (Try to Lock-On) */
        if (Input.GetButtonDown("Controller_Right_Stick_Click"))        //The player clicked the Right Stick in, BEGIN Lock-On Checks
                                                                        //		if (Input.GetAxis("Controller_RT") > 0)
        {
            //Debug.Log("Controller Right Stick Click: Pressed");
            if (Locked_On_To_Enemy == true) { Locked_On_To_Enemy = false; }     //THIS IS A TEST TO ALLOW LOCK-OFF
            else if (Locked_On_To_Enemy == false)       //if not locked on to an enemy
            {
                Found_An_Enemy = CalculateDistanceBetweenAllEnemies();
                if (Found_An_Enemy)
                {
                    if (Locked_On_To_Enemy == true) { Locked_On_To_Enemy = false; }
                    else if (Locked_On_To_Enemy == false) { Locked_On_To_Enemy = true; }
                    else { Debug.LogError("Error in 'Locked_On_To_Enemy' logic. You should not see this message."); }
                }
            }
            else { Debug.LogError("Error in 'Locked_On_To_Enemy' logic. You should not see this message."); }
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



    //This function is used to find the distance between the player and the targetable enemy
    void Return_Distance()
    {
        //		Debug.Log ("'Distance_To_Enemy' is: " + Distance_To_Enemy);								//Display the distance between the player and the enemy
        if (Distance_To_Enemy > maximum_lockon_distance) { Locked_On_To_Enemy = false; }                //Force player Lock-Off if player moves too far from enemy
        float temp_dist = Vector3.Distance(player.transform.position, targeted_enemy.transform.position);       //Calculate distance between player and enemy
        Distance_To_Enemy = temp_dist;                                                                  //Set the Distance_To_Enemy to the calculation solution
    }



    //Based on a boolean flag, sets the camera to follow the lock-ed on enemy, or return to normal camera movement
    void LockOn_Camera_Updater()
    {
        //If the player's camera is NOT in Combat Mode
        if (Locked_On_To_Enemy == false) { /* This is when the camera is set back to normal mode */ }
        //If the player's camera IS in Combat Mode
        else if (Locked_On_To_Enemy == true)
        {
            /* This is when the camera is set to combat mode */
            //transform.position = player.transform.position;             		//This update's the current tranform.position of Camera Anchor to the player's position
            transform.LookAt(targeted_enemy.transform);                         //Forces the Camera Anchor, and camera, to face the enemy while "locked-on"
        }
    }



    //This function will gather all the enemies (GameObjects) present in the level in order to determine lock-on priority
    void GatherAllEnemies()
    {
        targetable_enemies_arr = GameObject.FindGameObjectsWithTag("Enemy");
    }



    //This function will display all the enemies (elements) of the array (targetable_enemies_arr[i] )
    void DisplayTargetsInArray()
    {
        for (int index = 0; index < targetable_enemies_arr.Length; index++)
        {
            Debug.Log("Targetable Enemy #" + index + " is: " + targetable_enemies_arr[index]);
        }
    }



    //This function will calculate the distance between the player and all the enemies
    bool CalculateDistanceBetweenAllEnemies()
    {
        float calculated_distance_to_enemy = 0.0f;                              //This variable will be used (& overwritten each time) to store the distance between the enemy and the player
        float current_shortest_tracked_distance_to_enemy = 1000000000.0f;       //This variable will be used to keep track of the shortest distance to enemy (& overwritten only a new shortest distance is found)

        //Debug for displaying the length of the array
        Debug.Log("Length of array is: " + targetable_enemies_arr.Length);

        for (int distance_index = 0; distance_index < targetable_enemies_arr.Length; distance_index++)
        {
            //calculate distance to enemy
            calculated_distance_to_enemy = Vector3.Distance(player.transform.position, targetable_enemies_arr[distance_index].transform.position);

            //DEBUG - Display this distance
            Debug.Log("Distance to enemy #" + distance_index + "(Named: " + targetable_enemies_arr[distance_index] + ") is: " + calculated_distance_to_enemy);
            //END DEBUG

            //Compare current shortest to newly calculated: If not shorter, ignore & move on
            if (calculated_distance_to_enemy > current_shortest_tracked_distance_to_enemy)
            {
                //DO NOTHING
            }
            //compare current shortest to newly calculated: if shorter, set to current shortest
            else if (calculated_distance_to_enemy <= current_shortest_tracked_distance_to_enemy /*&& maximum_lockon_distance < calculated_distance_to_enemy*/)
            {
                //Set stored distance to calculated distance to update this new shortest distance
                current_shortest_tracked_distance_to_enemy = calculated_distance_to_enemy;

                //DEBUG - Display this distance
                //THIS KEEPS GETTING CALLED BECAUSE IT IS INSIDE THE LOOP
                Debug.Log("Current shortest distance to enemy is: " + current_shortest_tracked_distance_to_enemy);
                //END DEBUG

                targeted_enemy = targetable_enemies_arr[distance_index];

                if (current_shortest_tracked_distance_to_enemy <= maximum_lockon_distance)
                {
                    LockOn_Camera_Updater();
                }

            }
            else
            {
                //DO NOTHING
                Debug.LogError("You should not see this error. Something went wrong in the logic of calculating the shortest distance to enemy.");
            }
        }


        //Once the smallest calculated distance is found, decide what to do with it:
        if (current_shortest_tracked_distance_to_enemy <= maximum_lockon_distance)
        {
            //If the calculated distance is less than (or equal to) the maximum lock-on distance, return true to lock-on
            return true;
        }
        else if (current_shortest_tracked_distance_to_enemy > maximum_lockon_distance)
        {
            //If the calculated distance is more than the maximum lock-on distance, return false to prevent lock-on
            return false;
        }
        else
        {
            //This is only for error checking. Both above statements should catch any case. If this gets triggered, then something went horribly wrong.
            return false;
            Debug.LogError("You should not see this error. Something went wrong in the logic of calculating the shortest distance to enemy.");
        }
    }
}
