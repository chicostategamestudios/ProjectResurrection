//This script was written by James | Last edited by James | Modified on April 20, 2017
//The purpose of this script is to have an enemy chase the player. This is implemented by using a Nav Mesh Agent.
//It will then decide what to do with its list of actions.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAI : MonoBehaviour 
{

    public float ray_length = 4f; //ray length for sphere cast
	public float ray_width = 5f; //ray width for sphere cast
	public Transform target; //the target the AI will be chasing

    public bool performing_action = false; //this is to keep track if the ai is performing an action.
	public float performing_time = 0.5f; //this is how long the action takes to perform.
    public float cooldown_action = 0.5f; //how long the action will take before it performs another action.

    public float distance_to_player;
    public bool alerted = false; //once the ai has been alerted, it will start chasing the player.

    public GameObject cast_pos;

    public BasicAI_Attack attack_script;

    public enum ai_state 
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
        
        Debug.Log("performing dodge now");
		yield return new WaitForSeconds(performing_time);
        
        Debug.Log("done with dodge");
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

    private void Awake()
    {
        GameObject basic_ai = this.transform.FindChild("WeaponSpawn").gameObject;
        attack_script = basic_ai.GetComponent<BasicAI_Attack>();
        target = GameObject.Find("Player").transform;
    }


    void Update () 
	{

        distance_to_player = Vector3.Distance(target.position, transform.position); //calculate distance to player

        if (distance_to_player < 25) //if the player is close enough, this will set the ai to be alerted. if the enemy is alerted then it will chase the player.
        {
            alerted = true;
        }

        if (distance_to_player <= 5)
        {
            Quaternion rotate = Quaternion.LookRotation(target.position -  transform.position);
            transform.rotation = rotate;
        }

        if (alerted)
        {
            transform.GetComponent<UnityEngine.AI.NavMeshAgent>().destination = target.position; //telling this object to chase after the target's position.
        }


        if (distance_to_player < 5 && !performing_action) //if the ai is close enough and not performing an action, then it will pick a random action
														  //and then perform the action.
		{
			performing_action = true;  //the ai is now doing an action
			ai_state current_state = (ai_state)Random.Range(3, 5);
			switch (current_state) 
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
        
    }



}