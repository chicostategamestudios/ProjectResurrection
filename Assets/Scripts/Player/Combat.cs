using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour {

    public GameObject attack_prefab; // Prefab of attack hitbox
	public GameObject target_prefab; // Prefab of the traget destination of movement
    private GameObject camera_anchor; // finds the "Camera Anchor" gameobject to base rotation around
    private Transform weapon_spawn; // Spot where the weapon hitbox prefab will spawn from
    private RaycastHit hit; // used to detect collision
    private Vector3 target;
    private Vector3 input_joystick_left, input_direction;
	public float light_attack_distance; // distance travelled by light attack
    public float light_attack_time; // time it takes to move distance of light attack
    public float strong_attack_distance; // distance travelled by strong attack
    public float strong_attack_time; // time it takes to move distance of strong attack
    public float dodge_distance; // distance travelled while dodging
    public float dodge_time; // time it take to move distance of dodge
    private float distance_length;
    private int combo_counter;
    private bool is_light_attacking; // check to see if player is light attacking
    private bool is_strong_attacking; // check to see if player is strong attacking
    private bool is_comboing;
    private bool is_invunerable; // check to see if player is invunerable from dodging
    private bool something_too_close; // an object is too close to move forward

    private float controller_drift = 0.3f;

    // Placeholder value till animations are implemented
    private float next_attack;

    private void Start()
    {
        next_attack = 0;
        camera_anchor = GameObject.Find("Camera Anchor");
        weapon_spawn = GameObject.Find("Weapon_Spawn").transform;

        something_too_close = false;
    }

    void Update () {

        //Debug.Log(something_too_close);
        input_joystick_left = new Vector3(Input.GetAxisRaw("LeftJoystickX"), 0, Input.GetAxisRaw("LeftJoystickY")); // gets left joystick direction

        // Dodge movement
		if (is_invunerable && !something_too_close) // Checks if the player is dodging or an oject is too close to dodge
        {
            GameObject target_move = GameObject.FindGameObjectWithTag("Player Move Target"); // find the location of target_prefab

            // move player 10 units in direction joystick is pointing over "dodge_time"
            transform.position = Vector3.MoveTowards(transform.position, target_move.transform.position, dodge_time);
			//Debug.Log (transform.forward * dodge_distance);
		}

        // Attack movement
		if (is_light_attacking && !something_too_close) // checks to see if the player is already attacking or an object is too close
        {
			GameObject target_move = GameObject.FindGameObjectWithTag("Player Move Target"); // find the location of target_prefab
			// move player x units in direction joystick is pointing
			transform.position = Vector3.MoveTowards(transform.position, target_move.transform.position, light_attack_time);
			//Debug.Log (transform.forward * light_attack_distance);
		}


    }

    void FixedUpdate () {
        input_direction = input_joystick_left.normalized;

        // Fast Attack
		if (Input.GetButtonDown("Controller_Y") && !is_light_attacking) // if the Y button is pressed and the player is not already attacking
        {
            //Debug.Log("Light Attack");
            // Disable Player movement
            GetComponent<PlayerGamepad>().GamepadAllowed = false;
            // Check for Combo
            // TBA
            if (is_comboing)
            {
                combo_counter++;
            }
            else
            {
                combo_counter = 1;
            }

            // Set Player rotation to Camera Anchor rotation
            // This is so the player will alway face the enemy when they attack
            transform.eulerAngles = new Vector3 (transform.eulerAngles.x, camera_anchor.transform.eulerAngles.y, transform.eulerAngles.z);
			//Debug.Log("Rotation = " + transform.rotation);

            // raycast to detect if objects are infront of player
            Vector3 target_infront = transform.TransformDirection(Vector3.forward);

            // check to see if something is in the way
            // bool Physics.SphereCast(Ray ray, float radius, out RaycastHit hitInfo, float maxDistance))
            if (Physics.Raycast (transform.position, target_infront, out hit, (light_attack_distance))) // Raycast is a bit buggy with walls may consider changing to spherecast
            {
                // check to see if a wall or enemy is in the path of the player
                if (hit.collider.tag == "Wall" || hit.collider.tag == "Enemy")
                {
                    something_too_close = true;
                }
            }


            // Instantiate Move Target
            Instantiate(target_prefab, transform.position + (transform.forward * light_attack_distance), transform.rotation); // create target marker
            target_prefab.GetComponent<DestroyMove>().set_life = light_attack_time; // set the target markers life to equal the time it takes the player to attack

            // Instantiate Attack
            (Instantiate(attack_prefab, weapon_spawn.position, weapon_spawn.rotation *= Quaternion.Euler(0, 0, 0)) as GameObject).transform.parent = this.transform;
			is_light_attacking = true;
			// Start Animation Coroutine
            StartCoroutine(WaitForFastAttackAnimation()); // Call the Fast attack function at bottom of code
        }

        // Strong Attack
        // TBA
		if (Input.GetButtonDown("Controller_B") && !is_strong_attacking) // If controller button B is pressed and player is not already strong attacking
        {
            Debug.Log("Heavy Attack");
            // Disable Player movement
            GetComponent<PlayerGamepad>().GamepadAllowed = false;
            // Check for Combo
            // TBA
            if (is_comboing)
            {
                combo_counter++;
            }
            else
            {
                combo_counter = 1;
            }

            // Set Player rotation to Camera Anchor rotation
            // This is so the player will alway face the enemy when they attack
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, camera_anchor.transform.eulerAngles.y, transform.eulerAngles.z);
            Debug.Log("Rotation = " + transform.rotation);

            // raycast to detect if objects are infront of player
            Vector3 target_infront = transform.TransformDirection(Vector3.forward);

            // check to see if something is in the way
            if (Physics.Raycast(transform.position, target_infront, out hit, (strong_attack_distance)))
            {
                if (hit.collider.tag == "Wall" || hit.collider.tag == "Enemy")
                {
                    something_too_close = true;
                }
            }

            // Instantiate Move Target
            Instantiate(target_prefab, transform.position + (transform.forward * strong_attack_distance), transform.rotation); // create target marker
            target_prefab.GetComponent<DestroyMove>().set_life = strong_attack_time;
            // Instantiate Attack
            (Instantiate(attack_prefab, weapon_spawn.position, weapon_spawn.rotation *= Quaternion.Euler(0, 0, 0)) as GameObject).transform.parent = this.transform;
            is_strong_attacking = true; // set strong attack equal to true
            // Start Animation Coroutine
            StartCoroutine(WaitForStrongAttackAnimation()); // call strong attack function at bottom of code
        }

        // Dodge
        // My build did not have right trigger change "Controller_RB" in later builds to better represent design doc //
        if (Input.GetButtonDown("Controller_RB") && (Input.GetAxis("LeftJoystickX") > controller_drift || Input.GetAxis("LeftJoystickX") < -controller_drift || Input.GetAxis("LeftJoystickY") > controller_drift || Input.GetAxis("LeftJoystickY") < -controller_drift) && !is_invunerable)
        {
            //Debug.Log("Dodging");
            //Debug.Log(Input.GetAxis("LeftJoystickY"));
            // if game controller is disabled
            GetComponent<PlayerGamepad>().GamepadAllowed = true;

            // raycast to detect if objects are infront of player
            Vector3 target_infront = transform.TransformDirection(Vector3.forward);

            // check to see if something is in the way
            if (Physics.Raycast(transform.position, target_infront, out hit, (dodge_distance)))
            {
                //Debug.Log(hit);
                if (hit.collider.tag == "Wall" || hit.collider.tag == "Enemy")
                {
                    Debug.Log(hit.collider.tag);
                    something_too_close = true;
                }
            }

            Instantiate(target_prefab, transform.position + (transform.forward * dodge_distance), transform.rotation); // create target marker
            is_invunerable = true; // make player invunerable
            target_prefab.GetComponent<DestroyMove>().set_life = .5f; // set life to the dodge time

            //GetComponent<Rigidbody>().AddForce(transform.forward * 500000 * dodge_time * Time.deltaTime, ForceMode.Impulse);
            GetComponent<PlayerGamepad>().GamepadAllowed = false;
            StartCoroutine(Invunerable());
        }

        // Counter
        // TBA
        if (Input.GetButtonDown("Controller_Y") && Input.GetButtonDown("Controller_B"))
        {

        }
    }

    IEnumerator WaitForFastAttackAnimation ()
    {
        yield return new WaitForSeconds(.3f); // placeholder until animations are implemented use WaitTillAnimationEnd after
		GetComponent<PlayerGamepad>().GamepadAllowed = true;
		is_light_attacking = false;
        something_too_close = false;
        //StartCoroutine(ComboTimer());
    }

    IEnumerator WaitForStrongAttackAnimation()
    {
        yield return new WaitForSeconds(.3f); // placeholder until animations are implemented use WaitTillAnimationEnd after
        GetComponent<PlayerGamepad>().GamepadAllowed = true;
        is_strong_attacking = false;
        something_too_close = false;
        //StartCoroutine(ComboTimer());
    }

    IEnumerator Invunerable()
	{
        yield return new WaitForSeconds(.5f); // placeholder until animations are implemented use WaitTillAnimationEnd after
        is_invunerable = false;
        something_too_close = false;
        GetComponent<PlayerGamepad>().GamepadAllowed = true;
    }
}
