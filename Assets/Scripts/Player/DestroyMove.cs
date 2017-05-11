using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyMove : MonoBehaviour 
{
	private float lifetime; // target's lifetime
    public float set_life; // set target lifetime based on the move that is being used

    /*public void SetLife(float set_life)
    {
        lifetime = set_life;
    }*/

    void Start ()
	{
        Debug.Log(lifetime);
        lifetime = set_life; // set lifetime to target lifetime
		Destroy (gameObject, lifetime); // destroy gameobject after lifetime so there isn't hundreds
    }

    void OnTriggerStay (Collider col) 
	{
		if (col.tag == "Player") // if the player gets to the target before lifetime is finished
		{
			Destroy (gameObject); // go ahead and destroy that object
		}
	}
}
