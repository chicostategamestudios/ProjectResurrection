//Original Author: Alexander Stamatis || Last Edited: Alexander Stamatis | Modified on February 12, 2017
//This script transports the player through the ring
//To make this script work, the writen code in PlayerGamepad.cs -> OnTriggerEnter is needed to make the transportation work

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingManager : MonoBehaviour {

	public List<Transform> list_children;
	public int child_count, counter;
	public GameObject last_child;
	public GameObject player, camera_anchor;
	public bool engage_transit;

	void Awake(){
		camera_anchor = GameObject.Find ("Camera Anchor");
	}

	void Start () 
	{
		player = GameObject.Find ("Player");
		for (int i = 0; i < transform.childCount; i++) 
		{
			list_children.Add (transform.GetChild(i));
			list_children [i].name = "torus " + i;
            list_children[i].tag = "Launch Ring";
		}
		child_count = transform.childCount;
		last_child = transform.GetChild (child_count - 1).gameObject;
		engage_transit = false;
	}

	void FixedUpdate(){
		if (engage_transit) {
            player.GetComponent<Rigidbody>().useGravity = false;
            player.transform.position = Vector3.MoveTowards(player.transform.position, list_children[counter].position, Time.deltaTime * 60* 3);
               
        } else {
			player.GetComponent<Rigidbody> ().useGravity = true;
			counter = 0;
		}
	}

    //Need to make it so that you can't move while in ring

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject.name);
    }


}
