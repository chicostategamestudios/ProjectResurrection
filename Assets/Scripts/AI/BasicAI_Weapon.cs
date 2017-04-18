using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAI_Weapon : MonoBehaviour {

    public float duration = 0.5f;
    private float current_timer = 0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        current_timer += Time.deltaTime;
        if (current_timer >= duration)
        {
            Destroy(this.gameObject);
        }
	}
}
