//Original Author: Alexander Stamatis || Last Edited: Alexander Stamatis | Modified on May 2, 2017
//This script deals with player movement, camera, and some player collision and trigger interactions in the runtime world

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class PlayerGamepad : MonoBehaviour
{

    public delegate void Reset();

    //MOVEMENT
    [Tooltip("How fast the player moves")]
    public float speed;
    [Tooltip("How fast the player gets to max speed. Value between 0 and 1.")]
    public float acceleration;
    [Tooltip("How fast the player slows down. Value between 0 and 1.")]
    public float deacceleration;
    private float current_speed, speed_smooth_velocity, current_speed_multiplier;
    private bool disable_left_joystick, disable_right_joystick; //left stick is movement, right stick is camera

    //PLAYER
    private float player_direction;
    private float player_rotation_speed;
    private Rigidbody player_rigidbody;
    private Vector3 player_movement_direction;
    private bool on_air;

    //RAIL
    [Tooltip("Value between .01 and 4 for rail speed.")]
    public float rail_boost_speed;
    private bool on_rail, exiting_rail;
    GameObject rail_first_pos, rail_second_pos, main_long_rail_pos;
    private bool rail_going_forward;

    //JUMP
    public int jump_counter, jump_limit;
    [Tooltip("Value between 10 and 50 for jump_force.")]
    public float jump_force;
    private bool falling;
    private bool jump;
    private float jump_timer;
    private bool can_jump;

    //CAMERA
    private float camera_rotation_speed, turn_smooth_velocity, turn_smooth_time;
    public bool camera_recenter;
    private bool gamepad_allowed;
    private GameObject camera_anchor; //grabbing this from the hierarchy to override camera rotation
    public bool use_camera_type_1;

    //CHECKPOINT SYSTEM
    public Transform[] checkpoints;

    //GAMEPAD
    private Vector3 input_joystick_left, input_joystick_right, input_direction, last_direction;
    public bool GamepadAllowed
    {
        get { return gamepad_allowed; }
        set { gamepad_allowed = value; }
    }
    private bool AllowGamepadPlayerMovement, AllowGamepadCameraMovement;

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
    private GameObject wall_obj;
    private Vector3 wall_target_pos;
    private bool get_off_wall;

    //CHECK LAST FRAME DIRECTION
    private float last_second_player_direction;

    //Casts a ray infront, to stop player running into wall
    private Ray ray;
    private RaycastHit hit, hit_down, hit_down_2;

    //DASH
    [Tooltip("How long will the dash last. Recommend values under 5 seconds")]
    public float dash_duration;
    private float dash_timer;
    [Tooltip("The speed of the dash. Enter value between 10 - 150")]
    public float dash_speed;
    public bool can_dash, dash, give_dash;
    private float last_captured_y_pos; //Used to cancel out y movement
    private Vector3 last_captured_player_direction;//to commit a player to a direction, can't change directions when dashing
    private TrailRenderer dash_trail_renderer;
    private int dash_counter;
    

    //GUI Notification
    private bool gui_speed_enable;
    private Text gui_top_right_notifier;

    //BOOSTER
    public float booster_force;

    void Awake()
    {

        if(gui_top_right_notifier == null)
        {
            if (GameObject.Find("top_right_text_1"))
            {
                gui_top_right_notifier = GameObject.Find("top_right_text_1").GetComponent<Text>();
            }
        }

        AllowGamepadCameraMovement = true;
        AllowGamepadPlayerMovement = true;

        can_jump = true;

        if (use_camera_type_1 == false)
        {
            use_camera_type_1 = true;
        }

        if (booster_force == 0)
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
            can_dash = false;
        }

        if (dash_trail_renderer == false)
        {
            dash_trail_renderer = GetComponent<TrailRenderer>();
        }

        disable_left_joystick = false;

        //Checkpoints
        //Open up 3 slots in checkpoint
        checkpoints = new Transform[4];
        //for each checkpoint index check if there is something in it
        //else remind in the console that all of those indexes are not assigned
        for (int i = 1; i < 5; i++)
        {
            if (checkpoints[i - 1] == null)
            {
                checkpoints[i - 1] = GameObject.Find("checkpoint_" + i).transform;
            }
            else
            {
                Debug.LogError("please assign gameobject to " + checkpoints[i - 1]);
            }
        }
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

        //change this to control player desired speed
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
        //while in ring
        //disable player gravity and the player trail render component
        //make the rigidbody of the player y set to zero, in order for the velocity.y to not affect player height transformation
        if (in_ring)
        {
           
            player_rigidbody.useGravity = false;
            dash_trail_renderer.enabled = true;
            player_rigidbody.velocity = new Vector3(player_rigidbody.velocity.x, 0, player_rigidbody.velocity.z);
        }
        else
        {
        }

        

        if (gamepad_allowed)
        {
            if (!disable_left_joystick)
            {
                //JOYSTICK INPUTS
                //Track Vector3 pos of left and right joystick.
                input_joystick_left = new Vector3(Input.GetAxisRaw("LeftJoystickX"), 0, Input.GetAxisRaw("LeftJoystickY"));
            }
            if (!disable_right_joystick)
            {
                input_joystick_right = new Vector3(Input.GetAxisRaw("RightJoystickY"), Input.GetAxisRaw("RightJoystickX"), 0);
            }

            /////////////////////////////////////////////////////////////////////////////
            //	PAUSE                         
            /////////////////////////////////////////////////////////////////////////////

            if (Input.GetButtonDown("Controller_Start"))
            {
                if (Time.timeScale == 1)
                {
                    Time.timeScale = 0;
                } else
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

        if (jump_counter > 0 || on_air)
        {
            can_dash = true;
        }
        else
        {
            can_dash = false;
        }
    }

    void FixedUpdate()
    {
        //used to disable or enable the controller
        //used when the player is in the ring
        if (gamepad_allowed)
        {

            
            //IF ON AIR
            if (Physics.Raycast(transform.position, -transform.up, out hit_down_2, 1.5f))
            {
                on_air = false;
                //
                if (dash_counter == 1 && !Input.GetButton("Controller_X"))
                {
                    can_dash = true;
                    dash_counter = 0;
                }
                give_dash = true;
            }
            else
            {
                
                on_air = true;
            }

            if (on_air)
            {
                if (give_dash)
                {
                    dash_counter = 0;
                    can_dash = true;
                    ResetDashValues();
                    give_dash = false;
                }
            }

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
            if (AllowGamepadPlayerMovement)
            {
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
                    if (!on_air)
                    {
                        speed = acceleration;
                    }
                }

                if (input_direction == Vector3.zero)
                {
                    if (!on_air)
                    {
                        //change value of speed (used to control player movement) and slow it down
                        speed = deacceleration;
                    }

                    //...taking that value and utilizing it here
                    if (!on_rail)
                    {
                        transform.position += player_movement_direction * current_speed * Time.deltaTime; //MOVING THE PLAYER
                    }
                }

                if (input_joystick_left != Vector3.zero)
                {

                    //check if theres anything infront of the player
                    if (Physics.Raycast(transform.position, forward, out hit, 1.5f))
                    {
                        //ignore the trigger collision box of gameobjects with tag launchring
                        if (hit.collider.tag == "Launch Ring" || hit.collider.tag == "Wall" || hit.collider.tag == "Rail")
                        {
                            //if something is infront of the player within a distance, stop the player
                            if (!exiting_rail)
                            {
                                player_movement_direction = transform.forward;
                            }
                            if (exiting_rail)
                            {
                                current_speed = 20f;
                            }

                            if (!on_rail)
                            {
                                transform.position += player_movement_direction * current_speed * Time.deltaTime;
                            }
                        }

                        else
                        {
                            current_speed = 0;
                        }

                        if (hit.collider.tag == "Wall")
                        {
                            on_wall = true;
                        }
                    }
                    else
                    {
                        //if the player isn't exiting a rail, don't move the player using the rails direction
                        //the tranform.forward will overwrite the rails direction
                        if (!exiting_rail)
                        {
                            player_movement_direction = transform.forward;
                        }
                        if (!on_rail)
                        {
                            transform.position += player_movement_direction * current_speed * Time.deltaTime;
                        }
                    }

                }

                //Slow down the player while on air
                if (on_air)
                {
                    current_speed_multiplier = 33.0f;
                }
                else
                {
                    current_speed_multiplier = 48.0f;
                }

                //used to smoothly increase and decrease the velocity of the player, its a value...
                current_speed = Mathf.SmoothDamp(current_speed - ((delta / 1000) / 3.3f), input_joystick_left.sqrMagnitude * current_speed_multiplier, ref speed_smooth_velocity, speed);

                //same thing as Mathf.Clamp() but this works better for some reason
                if (current_speed > 48f)
                {
                    current_speed = 48f;
                }
                else if (current_speed < 0)
                {
                    current_speed = 0;
                }

            }

        

            /////////////////////////////////////////////////////////////////////////////
            //	WALL                        
            /////////////////////////////////////////////////////////////////////////////

            RaycastHit hit_wall;

            // Cast a sphere wrapping character controller 10 meters forward
            // to see if it is about to hit anything.
            if (Physics.SphereCast(transform.position, 5, transform.forward, out hit_wall, 1) && get_off_wall == false)
            {
                //print(hit_wall.transform.gameObject.name);
                if (hit_wall.transform.tag == "Wall")
                {
                    player_rigidbody.useGravity = false;
                    wall_target_pos = new Vector3(transform.position.x, hit_wall.transform.position.y, transform.position.z);
                    transform.position = Vector3.Lerp(transform.position, wall_target_pos, 10.5f * Time.deltaTime);
                    transform.position += wall_direction * 20 * Time.deltaTime;
                }
            }

            /////////////////////////////////////////////////////////////////////////////
            //	IF ON WALL              
            /////////////////////////////////////////////////////////////////////////////

            if (on_wall && player_rigidbody.velocity.magnitude < 170f)
            {
          
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

            //-----------------------------------------------------
            //	JUMP                         
            //-----------------------------------------------------

            //check if button A is pressed and if the jump counter is less than 0 (increments by 1 if statement below executes)
            if ((Input.GetButton("Controller_A")) && jump_counter < jump_limit && can_jump)
            {
                //raycast checks if there is something below the player
                //make sure that the ray goes father below the ground, if its a 1, it will cause issues like z-fighting (3d modeling), 
                //it won't know if its touching or touching the ground because its doing both if the object is a scale of 1 and the distance of the ray is 1
                if (Physics.Raycast(transform.position, -transform.up, out hit_down, 1.5f) || on_wall)
                {
                    player_rigidbody.AddForce(Vector3.up * jump_force * Time.fixedDeltaTime, ForceMode.Impulse);
                    jump_timer = 0f;
                    jump = true;
                    falling = true;
                    jump_counter++;
                    can_jump = false;
                    if (on_wall)
                    {
                        StartCoroutine(MoveFor(1.5F));
                    }
                }


                if (on_rail)
                {
                    exiting_rail = true;
                    AllowGamepadPlayerMovement = true;
                }
                StartCoroutine(GetOffWall());
                on_wall = false;
            }

            if ((Input.GetButtonUp("Controller_A")) && !can_jump)
            {
                can_jump = true;
            }


            //-------------------------------------------------
            //	RAIL                           
            //-------------------------------------------------

            //WHAT A MESS, trying to make it so that the playe doesnt drift horizontally away from the rail
            if (on_rail)
            {
                player_rigidbody.isKinematic = true;

                //calculate distance from players position to the targeted position or end position of rail
                float distance_from_end_rail = Vector3.Distance(transform.position, rail_second_pos.transform.position);
                //disable movement
                disable_left_joystick = true;
                //storing the last pos that player has to go in a vector3
                Vector3 look_at_last_rail_target_pos = rail_second_pos.transform.position;
                //setting y to players y axis, to avoid y tilting
                look_at_last_rail_target_pos.y = transform.position.y;
                //aligning player rotation to the pos the player has to go
                transform.LookAt(look_at_last_rail_target_pos);
                //moving the player
                transform.position += transform.forward * 100f * Time.fixedDeltaTime;

                if (distance_from_end_rail < 5.0f)
                {
                    exiting_rail = true;
                    //if get off get boosted towards rail_first_forward instead of player's direction FIX!!!!!!!!!!!!!!!!!
                }
            }
            else
            {
                disable_left_joystick = false;
            }

            if (exiting_rail)
            {
                player_rigidbody.isKinematic = false;
                on_rail = false;
                StartCoroutine(MoveFor(1.0f));
            }


            //Activate dash
            if ((Input.GetButton("Controller_X")) && can_dash && dash_counter < 1)
            {
                dash = true;
                print("dash");
                //capturing the last forward direction of the player, to commit the player to dash only to that directions
                //by doing so, the player can't change direction while dashing
                last_captured_y_pos = transform.position.y;
                last_captured_player_direction = transform.forward;
                Vector3 temp_forward = transform.TransformDirection(Vector3.forward);
                //if the player hits with something infront of it, stop the dash
                if (Physics.Raycast(transform.position, temp_forward, 1))
                {
                    can_dash = false;
                    dash = false;
                }
                else
                {
                    dash_counter = 1;
                }
            }

            //While dash is activated, this is where the player actually dashes
            if (dash)
            {
                can_dash = false;
                dash_timer += Time.fixedDeltaTime;
                if (Physics.Raycast(transform.position, transform.forward, 1))
                {
                    can_dash = false;
                    dash = false;
                }
                else
                {
                    dash_counter = 1;
                }
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
                   // speed = deacceleration;
                    StartCoroutine(MoveFor(3.0f));
                    ResetDashValues();
                }
            }

            /////////////////////////////////////////////////////////////////////////////
            //	CHECKPOINTS                        
            /////////////////////////////////////////////////////////////////////////////

            //D-PADS
            float d_pad_vertical = Input.GetAxis("DPadVertical");
            float d_pad_horizontal = Input.GetAxis("DPadHorizontal");
       
            //make camera rotation the same as player rotation
            if(d_pad_horizontal != 0 || d_pad_vertical != 0)
            {
                camera_anchor.transform.rotation = transform.rotation;
            }

            if (d_pad_horizontal == 1)
            {
                transform.position = checkpoints[2].position;
                transform.rotation = checkpoints[2].rotation;
            } else if (d_pad_horizontal == -1)
            {
                transform.position = checkpoints[0].position;
                transform.rotation = checkpoints[0].rotation;
            }

            if (d_pad_vertical == 1)
            {
                transform.position = checkpoints[1].position;
                transform.rotation = checkpoints[1].rotation;
            }
            else if (d_pad_vertical == -1)
            {
                transform.position = checkpoints[3].position;
                transform.rotation = checkpoints[3].rotation;
            }

            //if you press the back controller button, then transform the players position to a gameobject named "Spawn Point"
            if (Input.GetButtonDown("Controller_Back"))
            {
                transform.position = GameObject.Find("Spawn Point").transform.position;
                transform.rotation = GameObject.Find("Spawn Point").transform.rotation;
                //make camera rotation the same as player rotation
                camera_anchor.transform.rotation = transform.rotation;
            }


        }//gamepad
    }//fixedUpdate 

    private void LateUpdate()
    {
        //GUI
        if (gui_speed_enable)
        {
            //Notifies speed for testing
            //Changes color of text
            Color temp_color = new Color(0.05f * current_speed, .4f * current_speed, 1 * -current_speed, 1);
            gui_top_right_notifier.color = temp_color;
            gui_top_right_notifier.text = "SPEED: " + (int)current_speed + " MPH";
        }
    }


    //Used to capture last frames of the left joystick position, to compare it with the current one, this will calculate the delta (rate of change) of the left joystick
    void LastFrameLeftJoystick()
    {
        delta_before = (player_direction - 0f) / Time.fixedDeltaTime;
        last_second_player_direction = player_direction;
    }

    IEnumerator GetOffWall()
    {
        get_off_wall = true;
        yield return new WaitForSeconds(1.5f);
        get_off_wall = false;
    }

    //DASH
    //As the title of the function intends, to reset dash values for reuse of dash
    void ResetDashValues()
    {
        if (dash_counter < 1)
        {
            can_dash = true;
        }
        dash = false;
        dash_timer = 0;
        dash_trail_renderer.enabled = false;
    }

    //This will move the player for a little bit forward after the player has exited the rings or rails
    IEnumerator MoveFor(float seconds)
    {
        current_speed = 40f;
        current_speed_multiplier = 20f;
        current_speed = Mathf.SmoothDamp(current_speed, current_speed_multiplier * 50, ref speed_smooth_velocity, 0.5f);
        gamepad_allowed = true;
        if (exiting_rail)
        {
            current_speed_multiplier = 20f;
            if (rail_going_forward)
            {
                player_movement_direction = rail_second_pos.transform.forward;
            }
            if (!rail_going_forward)
            {
                player_movement_direction = -rail_second_pos.transform.forward;
            }
        }
        yield return new WaitForSeconds(0.5f);
        in_ring = false;
        exiting_rail = false;
    }


    IEnumerator TurnOffWall()
    {
        on_wall = false;
        yield return new WaitForSeconds(2.0f);
    }

    //Disable gravity (velocity.y), since it affects ring movetowards/lerping
    IEnumerator DisableGravityForRing()
    {
        GetComponent<Rigidbody>().useGravity = false;
        yield return new WaitForSeconds(3.0f);
        GetComponent<Rigidbody>().useGravity = true;
    }


    /////////////////////////////////////////////////////////////////////////////
    //	COLLIDERS               
    /////////////////////////////////////////////////////////////////////////////

    void OnCollisionEnter(Collision col)
    {
        //JUMPING
        jump_counter = 0;
        falling = false;

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
            jump_counter = 0;
        }

        //DEATH ZONE
        //if the player 
        if (col.gameObject.name == "Death Zone")
        {
            transform.position = GameObject.Find("Spawn Zone").transform.position;
        }
        //        //used to smoothly increase and decrease the velocity of the player, its a value...
        //        current_speed = Mathf.SmoothDamp(current_speed, input_joystick_left.sqrMagnitude * current_speed_multiplier, ref speed_smooth_velocity, speed);
        //        //...taking that value and utilizing it here
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag == "Rail")
        {
            on_rail = false;
        }
        if (col.gameObject.tag == "Wall")
        {
            StartCoroutine(TurnOffWall());
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Wall")
        {
            wall_obj = col.gameObject;
            jump_counter = 0;
        }


        if (col.gameObject.tag == "Rail")
        {

            jump_counter = 0;
            can_jump = true;

            //Find the difference between the player rotation and rail rotation
            Vector3 diff_rotation = transform.rotation.eulerAngles - col.gameObject.transform.rotation.eulerAngles;
            //Using Abs or absolute to make the values positive
            diff_rotation = new Vector3(Mathf.Abs(diff_rotation.x), Mathf.Abs(diff_rotation.y), Mathf.Abs(diff_rotation.z));

            //Stuff that happens when touching objects with Rail tag
            if (!exiting_rail && !on_rail)
            {
                main_long_rail_pos = col.gameObject.transform.parent.GetChild(3).transform.gameObject;
                Vector3 main_long_rail = col.gameObject.transform.parent.GetChild(3).transform.position;
                //transform.position = main_long_rail.

                //the statement below, makes sure that if it touches the first enter cube that it goes the correct way
                //by setting the first touched object to rail_first pos
                //then for the second pos the other child in the list
                if (col.gameObject.name == col.gameObject.transform.parent.GetChild(0).name)
                {
                    rail_first_pos = col.gameObject.transform.parent.GetChild(0).transform.gameObject;
                    rail_second_pos = col.gameObject.transform.parent.GetChild(1).transform.gameObject;
                    rail_going_forward = false;
                    transform.position = Vector3.Lerp(transform.position, rail_first_pos.transform.position, 0.2f);
                    //transform.position = rail_first_pos.transform.position;

                }
                //the statement below, makes sure that if it touches the first enter cube that it goes the correct way
                //by setting the first touched object to rail_first pos
                //then for the second pos the other child in the list
                if (col.gameObject.name == col.gameObject.transform.parent.GetChild(1).name)
                {
                    rail_first_pos = col.gameObject;
                    rail_second_pos = col.gameObject.transform.parent.GetChild(0).transform.gameObject;
                    rail_going_forward = true;

                    transform.position = Vector3.Lerp(transform.position, rail_first_pos.transform.position, 0.2f);
                    //transform.position = rail_first_pos.transform.position;

                }
                if (col.gameObject.name == col.gameObject.transform.parent.GetChild(2).name)
                {
                    //If the difference in rotation is between 90 and 270 degrees, then the player is facing the opposite way
                    if (diff_rotation.y > 90 && diff_rotation.y < 270)
                    {
                        rail_first_pos = col.gameObject.transform.parent.GetChild(0).transform.gameObject;
                        rail_second_pos = col.gameObject.transform.parent.GetChild(1).transform.gameObject;
                        rail_going_forward = false;
                    }
                    else //else the player has the same transform.forward as the rail that is being touched
                    {
                        rail_first_pos = col.gameObject;
                        rail_second_pos = col.gameObject.transform.parent.GetChild(0).transform.gameObject;
                        rail_going_forward = true;
                    }
                }


            }

            on_rail = true;
            if (col.gameObject.name == rail_second_pos.name)
            {
                exiting_rail = true;
            }
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

            if (ring_manager_script.counter == 0)
            {
                AllowGamepadPlayerMovement = false;

            }

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
                //Enables player movement with left joystick
                AllowGamepadPlayerMovement = true;
                //Aligns the player's rotation with the ring
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles.x, col.transform.eulerAngles.z, transform.rotation.eulerAngles.z), 1.0f);
                ring_direction = col.transform.up;
                player_movement_direction = ring_direction;
                StartCoroutine(MoveFor(4.0f));
                StartCoroutine(DisableGravityForRing());
                //player_rigidbody.AddForce(ring_direction * 10000000 / 3.0f * Time.deltaTime, ForceMode.Impulse);
            }
        }

        if (col.gameObject.tag == "Launch Pad")
        {
            //simple addforce jumper for launch pad
            GetComponent<Rigidbody>().AddForce(col.gameObject.transform.forward * 500000 * booster_force * Time.deltaTime, ForceMode.Impulse);
            //to NOT allow player to jump after hitting launch pad
            jump_counter++;
            //to allow player to use dash
            dash_counter++;
        }
    }
}