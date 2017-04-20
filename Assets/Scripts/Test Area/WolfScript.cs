using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float distance = Vector3.Distance(transform.position, GameObject.Find("Player").transform.position);
        //print(distance);
        if(distance < 4.1f)
        {
            GetComponent<Animation>().Play("Wolf_Skeleton|Wolf_Idle_");
        }
        else
        {
            GetComponent<Animation>().Play("Wolf_Skeleton|Wolf_Run_Cycle_");
        }
    }
}
