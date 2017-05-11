using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {
	
    // Will be used to detect collisions for combos
	// Update is called once per frame
	void Update () {
        Destroy(gameObject, .3f);
	}

    //Detect if hitting an Enemy
    void OnTriggerEnter(Collider col)
    {
        Debug.Log("Collision Detected");
        if (col.tag == "Enemy") // detect if an enemy is hit
        {
            Debug.Log("Hit Enemy");
        }

        if (col.tag == "Player")
        {
            return;
        }
    }

}
