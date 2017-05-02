//This script was written by James | Last edited by James | Modified on April 27, 2017
//The purpose of this script is to manage the player's health


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 100; //the health of the player.
    private bool is_alive = true; //this is to keep track if the player is alive.
    
    public void DamageReceived(int damage) //function to apply the damage to the player's health.
    {
        health -= damage;

        if(health > 0) //this is to display the health of the player in console.
        { 
            print("Player health is now: " + health);
        }

        if (health <= 0 && is_alive)
        {
            is_alive = false;
            Debug.Log("You died...");
            //this.gameObject.SetActive(false);
        }
    }

}
