//This script was written by James | Last edited by James | Modified on April 25, 2017
//The purpose of this script is to manage the player's health


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 100;
    private bool is_alive = true;
    
    public void DamageReceived(int damage)
    {
        health -= damage;
        if (health <= 0 && is_alive)
        {
            is_alive = false;
            Debug.Log("Player has died...");
            //this.gameObject.SetActive(false);
        }
    }

}
