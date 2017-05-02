//This script was written by James | Last edited by James | Modified on May 2, 2017
//The purpose of this script is to have an enemy chase the player. This is implemented by using a Nav Mesh Agent.
//It will then decide what to do with its list of actions.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAI : MonoBehaviour 
{
    public int left_or_right = -1; // will decide to move left or right in the dodge. 0 is left, 1 is right.

    public float distance_to_player; //used to keep track between this ai and the player.
    public float performing_time = 0.5f; //this is how long the action takes to perform.
    public float dodge_time = 2f;
    public float cooldown_action = 0.5f; //how long the action will take before it performs another action.
    public float turn_speed = 1f; //the turn speed of the ai.

    private bool first_alert = false; //used to keep track if the ai has been alerted the first time.
    private bool alerted = false; //once the ai has been alerted, it will start chasing the player.
    private bool performing_action = false; //this is to keep track if the ai is performing an action.

    private Quaternion look_rotation;

    private Vector3 direction;

    public GameObject left_dodge, right_dodge;

    private BasicAI_Attack attack_script; //to access its attack script.

    private Transform target; //the target the AI will be chasing



    public enum ai_state //state number starts at 0. So for example ai_state[2] is dodge
    {
        idle,     
        walking,
		dodging,
        attack_1,
        attack_2,
        attack_3
    } // states of the AI.

	public IEnumerator Dodge()
    {
        alerted = false; //set alert to false to make the ai stop chasing the player to allow dodging.
        left_or_right = Random.Range(0, 2);
		yield return new WaitForSeconds(dodge_time);
        alerted = true; //set alert back to true to make the ai chase the player again.
        left_or_right = -1;
		yield return new WaitForSeconds(cooldown_action);
		performing_action = false;  //the ai is done with the action
    }

    public IEnumerator Attack_1()
    {
        attack_script.check_attack_left = true;
        yield return new WaitForSeconds(performing_time);
        //waits for performing time to be done... then add more things here if needed for after action
		yield return new WaitForSeconds(cooldown_action);
        //this is the time to wait for the next action to be performed.
		performing_action = false;  //the ai is done with the action
    }

    public IEnumerator Attack_2()
    {
        attack_script.check_attack_right = true;
        yield return new WaitForSeconds(performing_time);
        //waits for performing time to be done... then add more things here if needed for after action
        yield return new WaitForSeconds(cooldown_action);
        //this is the time to wait for the next action to be performed.
        performing_action = false;  //the ai is done with the action
    }

    public IEnumerator Attack_3()
    {
        attack_script.check_attack_top = true;
        yield return new WaitForSeconds(performing_time);
        //waits for performing time to be done... then add more things here if needed for after action
        yield return new WaitForSeconds(cooldown_action);
        //this is the time to wait for the next action to be performed.
        performing_action = false;  //the ai is done with the action
    }

    //instantiation to set up the AI attacks.
    private void Awake()
    {
        GameObject basic_ai = this.transform.FindChild("WeaponSpawn").gameObject;
        attack_script = basic_ai.GetComponent<BasicAI_Attack>(); 
        target = GameObject.Find("Player").transform; //sets the target of this ai to the player
    }


    void Update () 
	{

        distance_to_player = Vector3.Distance(target.position, transform.position); //calculate distance to player

        
        if (distance_to_player < 25 && !first_alert) //if the player is close enough, this will set the ai to be alerted. if the enemy is alerted then it will chase the player. AKA aggro range
        {
            alerted = true;
            first_alert = true;
        }
        

        if (distance_to_player <= 20) //if the player is close, this will keep the ai rotated towards the player
        {
            direction = (target.position - transform.position).normalized;
            look_rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, look_rotation, Time.deltaTime * turn_speed);

            //legacy code below for really good aim bot.
            /*
            Vector3 look_pos = target.position;
            look_pos.y = transform.position.y;
            Quaternion rotate = Quaternion.LookRotation(look_pos -  transform.position);
            transform.rotation = rotate;
            */
        }

        if (alerted)  //if the ai is aggro, then it will chase the player.
        {
            transform.GetComponent<UnityEngine.AI.NavMeshAgent>().destination = target.position; //telling this object to chase after the target's position.
        }


        if (distance_to_player <= 5.5f && !performing_action) //if the ai is close enough and not performing an action, then it will pick a random action
														  //and then perform the action.
		{
			performing_action = true;  //the ai is now doing an action
			ai_state current_state = (ai_state)Random.Range(2, 6);  //will pick from dodge through attack 3
			switch (current_state) //based on the choice, do the coroutines
			{
			case ai_state.dodging:
				StartCoroutine ("Dodge");
				break;
			case ai_state.attack_1:
				StartCoroutine ("Attack_1");
				break;
			case ai_state.attack_2:
				StartCoroutine ("Attack_2");
				break;
            case ai_state.attack_3:
                StartCoroutine("Attack_3");
                break;
            }// at the end of each coroutines, it will set performing_action back to false to allow for a loop if they are within range.
		}


        if (left_or_right == 0)
        {
            transform.position = Vector3.Lerp(transform.position, left_dodge.transform.position, 1f * Time.deltaTime);
        }
        else if (left_or_right == 1)
        {
            transform.position = Vector3.Lerp(transform.position, right_dodge.transform.position, 1f * Time.deltaTime);
        }

    }



}