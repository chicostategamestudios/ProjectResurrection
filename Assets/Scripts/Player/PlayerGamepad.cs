//Original Author: Alexander Stamatis || Last Edited: Alexander Stamatis | Modified on March 9, 2017
//This script deals with player movement, camera, and some player collision and trigger interactions in the runtime world

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerGamepad : MonoBehaviour
{

    //MOVEMENT
    [Tooltip("How fast the player moves")]
    public float speed;
    [Tooltip("How fast the player gets to max speed. Value between 0 and 1.")]
    public float acceleration;
    [Tooltip("How fast the player slows down. Value between 0 and 1.")]
    public float deacceleration;
    private float current_speed, speed_smooth_velocity, speed_smooth_time, current_speed_multiplier;
    private float camera_timer;
    private bool disable_left_joystick;

    //PLAYER
    private float player_direction;
    private float player_rotation_speed;
    private Rigidbody player_rigidbody;
    private Vector3 player_movement_direction;

    //RAIL
    [Tooltip("Value between .01 and 4 for rail speed.")]
    public float rail_boost_speed;
    private bool on_rail;
    private Vector3 rail_direction;
    GameObject rail_first_pos, rail_second_pos;

    //JUMP
    public int jump_counter, jump_limit;
    [Tooltip("Value between 10 and 50 for jump_force.")]
    public float jump_force;
    private bool falling;
    private bool jump;
    private float jump_timer;

    //CAMERA
    private float camera_rotation_speed, turn_smooth_velocity, turn_smooth_time;
    public bool camera_recenter;
    private bool gamepad_allowed;
    private GameObject camera_anchor; //grabbing this from the hierarchy to override camera rotation
    public bool use_camera_type_1;

    //GAMEPAD
    private Vector3 input_joystick_left, input_joystick_right, input_direction, last_direction;
    public bool GamepadAllowed
    {
        get
        {
            return gamepad_allowed;
        }
        set
        {
            gamepad_allowed = value;
        }
    }
    private float last_angle, real_angle;
    private float delta_before, delta_now;
    private float delta, difference_in_degrees;

    //RING 
    private bool ring_transit;
    private Quaternion ring_rotation;
    private Vector3 ring_direction;
    private bool in_ring;
    //get script from col.gameObject
    RingManager ring_manager_script;

    //WALL
    private bool on_wall;
    private Vector3 wall_direction;

    //CHECK LAST FRAME DIRECTION
    private float last_second_player_direction;

    //Casts a ray infront, to stop player running into wall
    private Ray ray;
    private RaycastHit hit;

    //DASH
    [Tooltip("How long will the dash last. Recommend values under 5 seconds")]
    public float dash_duration;
    private float dash_timer;
    [Tooltip("The speed of the dash. Enter value between 10 - 150")]
    public float dash_speed;
    public bool can_dash, dash;
    private float last_captured_y_pos; //Used to cancel out y movement
    private Vector3 last_captured_player_direction;//to commit a player to a direction, can't change directions when dashing
    private TrailRenderer dash_trail_renderer;

    //BOOSTER
    public float booster_force;

    void Awake()
    {
        if(use_camera_type_1 == false)
        {
            use_camera_type_1 = true;
        }

        if(booster_force == 0)
        {
            booster_force = 10f;
        }

        //get camera
        if (camera_anchor == null)
        {
            camera_anchor = GameObject.Find("Camera Anchor");
        }

        //get player rigidbody
        if (player_rigidbody == null)
        {
            player_rigidbody = GetComponent<Rigidbody>();
        }

        //get trail
        if (dash_trail_renderer == null)
        {
            dash_trail_renderer = GetComponent<TrailRenderer>();
        }

        if (dash_trail_renderer.enabled == true)
        {
            dash_trail_renderer.enabled = false;
        }


        //DASH
        if (dash_speed == 0)
        {
            dash_speed = 80f;
        }

        if (dash_speed > 150)
        {
            Debug.LogError("Dash speed too fast to check colliders. Please use under 150");
        }

        if (dash_duration == 0)
        {
            dash_duration = 0.3f;
        }
        if (dash_duration > 5f)
        {
            Debug.LogError("Please use a value under 5 seconds.");
        }
        if (can_dash == false)
        {
            can_dash = true;
        }

        if (dash_trail_renderer == false)
        {
            dash_trail_renderer = GetComponent<TrailRenderer>();
        }

        disable_left_joystick = false;
    }

    void Start()
    {

        //This will enable player control, for example gamepad_allowed is set to false when the player is in the sonic rings
        gamepad_allowed = true;

        if (acceleration > 1 || acceleration < 0)
        {
            //Console.("Please enter a value between 0 and 1 for acceleration");
        }
        else if (acceleration == 0)
        {
            acceleration = 0.5f;
        }

        if (deacceleration > 1 || deacceleration < 0)
        {
            //Console.Error("Please enter a value between 0 and 1 for deacceleration");
        }
        else if (deacceleration == 0)
        {
            deacceleration = 0.2f;
        }


        if (camera_rotation_speed == 0)
        {
            camera_rotation_speed = 200;
        }

        if (turn_smooth_time == 0)
        {
            turn_smooth_time = 0.4f;
        }

        if (speed == 0)
        {
            speed = 0.5f;
        }

        if (current_speed_multiplier == 0)
        {
            current_speed_multiplier = 48;
        }


        if (jump_limit == 0)
        {
            jump_counter = 0;
            jump_limit = 1; //change to 2 for double jump
        }

        if (jump_force == 0)
        {
            jump_force = 35;
            jump_force *= 100000f;
        }

        //speed of player on rail
        if (rail_boost_speed == 0)
        {
            rail_boost_speed = 3f;
        }

        //Used for checking left joystick rotation rate...
        //... and checks previous frame to current frame
        InvokeRepeating("LastFrameLeftJoystick", 0, 0.1f);
    }

    void Update()
    {

        if (gamepad_allowed)
        {
            if (!disable_left_joystick)
            {
                //JOYSTICK INPUTS
                //Track Vector3 pos of left and right joystick.
                input_joystick_right = new Vector3(Input.GetAxisRaw("RightJoystickY"), Input.GetAxisRaw("RightJoystickX"), 0);
                input_joystick_left = new Vector3(Input.GetAxisRaw("LeftJoystickX"), 0, Input.GetAxisRaw("LeftJoystickY"));
            }


            /////////////////////////////////////////////////////////////////////////////
            //	PAUSE                         
            /////////////////////////////////////////////////////////////////////////////

            if (Input.GetButtonDown("Controller_Start"))
            {
                if (Time.timeScale == 1)
                {

                    Time.timeScale = 0;
                }
                else
                {
                    Time.timeScale = 1;
                }
            }


            /////////////////////////////////////////////////////////////////////////////
            //	RING                           
            /////////////////////////////////////////////////////////////////////////////

            if (ring_transit)
            {
                transform.eulerAngles = new Vector3(0, ring_rotation.eulerAngles.y, 0);
                camera_anchor.transform.rotation = Quaternion.Slerp(camera_anchor.transform.rotation, Quaternion.Euler(camera_anchor.transform.eulerAngles.x, ring_rotation.eulerAngles.y, 0), 5 * Time.deltaTime);
            }

            player_direction = Mathf.Atan2(input_direction.x, input_direction.z) * Mathf.Rad2Deg; //calculate the direction of the joystick, doing this by finding the theta angle, we are given the x value from the input_joystick_left.x and y value from the input_joystick_left.right

        }
    }

    void FixedUpdate()
    {

        //used to disable or enable the controller
        //used when the player is in the ring
        if (gamepad_allowed)
        {

            //slow down rotation of player while on air
            if (falling)
            {
                player_rotation_speed = 6f;
            }
            else
            {
                player_rotation_speed = 18f;
            }


            ///////////////////////////////////////////////////////////
            //  MOVEMENT                                             //
            ///////////////////////////////////////////////////////////

            //assigns input_joystick_left as a normalized to shorten the vectors length
            input_direction = input_joystick_left.normalized;

            //Calculate joystick rotation sensitivity
            //Used to slow down player
            delta_now = (player_direction - 0f) / Time.fixedDeltaTime;
            delta = Mathf.Abs(delta_now - delta_before);
            //this will calculate the difference or change in last frame's joystick position to current joystick position
            difference_in_degrees = Mathf.Abs(player_direction - last_second_player_direction);

            //If something is 1 unit infront of the player, the player can't dash
            Vector3 forward = transform.TransformDirection(Vector3.forward);

            //if we are not using joystick
            if (input_direction != Vector3.zero)
            {
                //calculate the direction of the joystick, doing this by finding the theta angle, we are given the x value from the input_joystick_left.x and y value from the input_joystick_left.right
                player_direction = Mathf.Atan2(input_direction.x, input_direction.z) * Mathf.Rad2Deg;
                //slowly rotate from the initial rotation to the player rotation, adding camera_anchor.eulerAngles to make it so the axis is based of the camera rotation
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, player_direction + camera_anchor.transform.eulerAngles.y, 0), player_rotation_speed * Time.deltaTime);
                speed = acceleration;
            }

            if (input_direction == Vector3.zero)
            {
                //change value of speed (used to control player movement) and slow it down
                speed = deacceleration;
                //used to smoothly increase and decrease the velocity of the player, its a value...
                current_speed = Mathf.SmoothDamp(current_speed, input_joystick_left.sqrMagnitude * current_speed_multiplier, ref speed_smooth_velocity, speed);
                //...taking that value and utilizing it here
                transform.position += transform.forward * current_speed * Time.deltaTime;
            }

            if (input_joystick_left != Vector3.zero)
            {
                //used to smoothly increase and decrease the velocity of the player, its a value...
                current_speed = Mathf.SmoothDamp(current_speed - ((delta / 1000) / 3.3f), input_joystick_left.sqrMagnitude * current_speed_multiplier, ref speed_smooth_velocity, speed);
                if (current_speed < 4.0f)
                {
                    current_speed = 4.0f;
                }

                player_movement_direction = transform.forward;
                

                //check if theres anything infront of the player
                if (Physics.Raycast(transform.position, forward, out hit, 1))
                {
                    //ignore the trigger collision box of gameobjects with tag launchring
                    if (hit.collider.tag != "Launch Ring" && hit.collider.tag != "Wall")
                    {
                        //if something is infront of the player within a distance, stop the player
                        current_speed = 0;
                    }
                    if(hit.collider.tag == "Wall")
                    {
                        on_wall = true;
                    }
                }
                else
                {
                    //the player can still move
                    transform.position += player_movement_direction * current_speed * Time.deltaTime;
                }
            }

            /////////////////////////////////////////////////////////////////////////////
            //	WALL                        
            /////////////////////////////////////////////////////////////////////////////

            if (on_wall)
            {
               // transform.position = new Vector3(transform.position.x, hit.collider.transform.position.y, transform.position.z);


            }

            /////////////////////////////////////////////////////////////////////////////
            //  CAMERA        
            //  This is the only thing that moves the camera with the right joystick                
            /////////////////////////////////////////////////////////////////////////////

            //Do the following if the right joystick is moving
            if (use_camera_type_1)
            {
                if (Input.GetAxisRaw("RightJoystickX") != 0 || Input.GetAxisRaw("RightJoystickY") != 0)
                {
                    //Get the input from the right joystick and start rotating...
                    Vector3 target_rotation = input_joystick_right * camera_rotation_speed * Time.deltaTime;
                    //...the camera
                    camera_anchor.transform.eulerAngles += target_rotation;
                }
            }
    
            /////////////////////////////////////////////////////////////////////////////
            //	JUMP                         
            /////////////////////////////////////////////////////////////////////////////

            if ((Input.GetButton("Controller_A")) && jump_counter < jump_limit)
            {
                player_rigidbody.AddForce(Vector3.up * jump_force * Time.fixedDeltaTime, ForceMode.Impulse);
                jump_timer = 0f;
                jump = true;
                falling = true;
                jump_counter++;
                ToggleOnRail();
                if (on_wall)
                {
                    StartCoroutine(MoveFor(1.0F));
                }
            }

            /////////////////////////////////////////////////////////////////////////////
            //	RAIL                           
            /////////////////////////////////////////////////////////////////////////////

            if (on_rail)
            {
                disable_left_joystick = true;
                transform.position = Vector3.MoveTowards(transform.position, rail_second_pos.transform.position, rail_boost_speed);
                transform.position = new Vector3(transform.position.x, rail_first_pos.transform.position.y, transform.position.z);
                float distance_from_end_rail = Vector3.Distance(transform.position, rail_second_pos.transform.position);
                if (distance_from_end_rail < 5.0f)
                {
                    on_rail = false;
                    StartCoroutine(MoveFor(1.0f));
                    //if get off get boosted towards rail_first_forward instead of player's direction FIX!!!!!!!!!!!!!!!!!
                }
            }
            else
            {
                disable_left_joystick = false;
            }

            //Activate dash
            if ((Input.GetButton("Controller_X")) && can_dash)
            {
                dash = true;
                //capturing the last forward direction of the player, to commit the player to dash only to that directions
                //by doing so, the player can't change direction while dashing
                last_captured_y_pos = transform.position.y;
                last_captured_player_direction = transform.forward;
                Vector3 temp_forward = transform.TransformDirection(Vector3.forward);
                //if the player hits with something infront of it, stop the dash
                if (Physics.Raycast(transform.position, temp_forward, 1))
                {
                    can_dash = false;
                }
                else
                {
                    can_dash = true;
                }
            }

            //While dash is activated, this is where the player actually dashes
            if (dash)
            {
                can_dash = false;
                dash_timer += Time.fixedDeltaTime;
                if (dash_timer < dash_duration)
                {
                    //enable the trail render
                    dash_trail_renderer.enabled = true;
                    //move the player
                    transform.position += last_captured_player_direction * dash_speed * Time.fixedDeltaTime;
                    transform.position = new Vector3(transform.position.x, last_captured_y_pos, transform.position.z);
                }
                else
                {
                    //reset the time to 0 and the can_dash to true, to reuse dash 
                    ResetDashValues();
                }
            }
        }
    }

    //Used to capture last frames of the left joystick position, to compare it with the current one, this will calculate the delta (rate of change) of the left joystick
    void LastFrameLeftJoystick()
    {
        delta_before = (player_direction - 0f) / Time.fixedDeltaTime;
        last_second_player_direction = player_direction;
    }

    //DASH
    //As the title of the function intends, to reset dash values for reuse of dash
    void ResetDashValues()
    {
        dash = false;
        dash_timer = 0;
        dash_trail_renderer.enabled = false;
    }
    

        //This will move the player for a little bit forward after the player has exited the rings or rails
    IEnumerator MoveFor(float seconds)
    {
        current_speed = Mathf.SmoothDamp(current_speed, current_speed_multiplier * 50, ref speed_smooth_velocity, 0.5f);
        gamepad_allowed = true;
        yield return new WaitForSeconds(0.5f);
        in_ring = false;
    }

    void ToggleOnRail()
    {
        on_rail = false;
    }


    /////////////////////////////////////////////////////////////////////////////
    //	COLLIDERS               
    /////////////////////////////////////////////////////////////////////////////

    void OnCollisionEnter(Collision col)
    {

        //JUMPING
        jump_counter = 0;
        falling = false;

        //DASH
        //if touched the ground
        if (col.gameObject.name == "Ground")
        {
            
        }else
        {
            
        }
        //player can dash again
        ResetDashValues();
        can_dash = true;


        //WALL
        //wall booster
        if (col.gameObject.tag == "Wall")
        {
            on_wall = true;
            //get direction of wall
            wall_direction = col.gameObject.transform.forward;
        }

        //DEATH ZONE
        //if the player 
        if (col.gameObject.name == "Death Zone")
        {
            transform.position = GameObject.Find("Spawn Zone").transform.position;
        }
        //used to smoothly increase and decrease the velocity of the player, its a value...
        current_speed = Mathf.SmoothDamp(current_speed, input_joystick_left.sqrMagnitude * current_speed_multiplier, ref speed_smooth_velocity, speed);
        //...taking that value and utilizing it here
        transform.position += transform.forward * current_speed * Time.deltaTime;

    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag == "Rail")
        {
            on_rail = false;
        }
        if (col.gameObject.tag == "Wall")
        {
            on_wall = false;
        }
    }


    void OnTriggerEnter(Collider col)
    {

       
        if (col.gameObject.tag == "Rail")
        {
            on_rail = true;
            rail_first_pos = col.gameObject;
            rail_second_pos = col.gameObject.transform.parent.GetChild(0).transform.gameObject;
        }


        if (col.gameObject.name == rail_second_pos.name)
        {
            on_rail = false;
            StartCoroutine(MoveFor(1.0f));
        }

        if (col.gameObject.tag == "Launch Ring")
        {

            in_ring = true;
            //Used to calculate direction
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            //Used to compare directions
            Vector3 to_other = col.transform.up - transform.position;
            
            ring_manager_script = col.transform.parent.transform.parent.GetComponent<RingManager>();
            ring_manager_script.engage_transit = true;
            //get col.gameObject name
            string col_name = col.transform.parent.name;
            //get last character of col.gameObject.name, its a number
            //the rings are automatically named in RingManager.cs
            string last_character = col_name.Substring(col_name.Length - 1);
            ring_manager_script.counter = Convert.ToInt32(last_character);

            if (ring_manager_script.counter < ring_manager_script.child_count - 1)
            {
                ring_manager_script.counter++;
                ring_manager_script.engage_transit = true;
            }
            else
            {
                ring_manager_script.counter++;
                ring_manager_script.engage_transit = false;
            }

            //this is executed when the player reaches the final ring, to push the player our a bit
            if (ring_manager_script.counter == ring_manager_script.child_count)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles.x, col.transform.eulerAngles.z, transform.rotation.eulerAngles.z), 1.0f);
                ring_direction = col.transform.up;
                player_movement_direction = ring_direction;
                StartCoroutine(MoveFor(3.0f));
            }

        }

        if (col.gameObject.tag == "Launch Pad")
        {
            GetComponent<Rigidbody>().AddForce(col.gameObject.transform.forward * 500000 * booster_force * Time.deltaTime, ForceMode.Impulse);
        }

    }


}

