using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    public GameObject player;

	// Use this for initialization
	void Start () {
        if (this.gameObject.name == "sharknado point")
        {
            player = GameObject.Find("Player");

        }
        else
        {
            player = GameObject.Find("sharknado point");
        }
	}

    Quaternion rotation;

	// Update is called once per frame
	void Update () {
        rotation = Quaternion.LookRotation(player.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 1f);
        transform.position += transform.forward * 20f * Time.deltaTime;
	}
}
