//This script was written by James | Last edited by James | Modified on April 20, 2017
//Purpose of this script is to destroy the weapon after a few seconds. Purely for hitbox of AI attacks.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAI_Weapon : MonoBehaviour {


    public float duration = 0.5f;
    private float current_timer = 0f;

    public int damage = 5;


    private PlayerHealth player_hp_script;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            player_hp_script = other.GetComponentInParent<PlayerHealth>();
            player_hp_script.DamageReceived(damage);
            Debug.Log("Player was hit!");
        }
            
    }



    void Update ()
    {
        current_timer += Time.deltaTime;
        if (current_timer >= duration)
        {
            Destroy(this.gameObject);
        }
	}
}
