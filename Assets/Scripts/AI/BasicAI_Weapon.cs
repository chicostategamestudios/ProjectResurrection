//This script was written by James | Last edited by James | Modified on April 27, 2017
//Purpose of this script is to destroy the weapon after a few seconds. Purely for hitbox of AI attacks.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAI_Weapon : MonoBehaviour {

    public int damage = 5; //damage of the weapon to hurt the player.

    public bool falling = false;

    private PlayerHealth player_hp_script; //this is to access the player's health script

    private void OnTriggerEnter(Collider other) //if the weapon hits the player, apply the damage to the player's health script
    {
        if(other.gameObject.tag == "Player")
        {
            player_hp_script = other.GetComponentInParent<PlayerHealth>();
            player_hp_script.DamageReceived(damage);
        }
    }

    private void DestroySelf() //this is to turn off the weapon after it is done with attacking.
    {
        Destroy(this.gameObject);
    }

    private void Start()
    {
        Invoke("DestroySelf", 0.5f); //destroy the game object after 0.5 seconds
    }


    private void Update()
    {
        if(falling)
        {
            Vector3 temp = this.transform.position;
            temp.y -= 6f * Time.deltaTime;
            this.transform.position = temp;
        }
    }
}
