using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyMove : MonoBehaviour 
{
	private float lifetime;
    public float set_life;

    /*public void SetLife(float set_life)
    {
        lifetime = set_life;
    }*/

	void Start ()
	{
        Debug.Log(lifetime);
        lifetime = set_life;
		Destroy (gameObject, lifetime);
    }

    void OnTriggerStay (Collider col) 
	{
		if (col.tag == "Player") 
		{
			Destroy (gameObject);
		}
	}
}
