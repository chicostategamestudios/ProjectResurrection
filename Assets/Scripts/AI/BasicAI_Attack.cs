using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAI_Attack : MonoBehaviour {

    private Vector3 left_spawn_pos; //the position in which the object is first. This will be used to reset the position back to where it originally was
    private Vector3 right_spawn_pos;
    private Vector3 top_spawn_pos;
    public Vector3 current_pos;

    public float duration = 0.5f;
    private float current_timer = 0f;

    public bool check_attack_left = false;
    private bool attacking_left = false;
    public bool check_attack_right = false;
    private bool attacking_right = false;
    public bool check_attack_top = false;
    private bool attacking_top = false;
    private bool done_attacking = false;


    public GameObject weapon_left_prefab;
    public GameObject weapon_right_prefab;
    private GameObject left_arm_pos;
    private GameObject right_arm_pos;
    private GameObject top_arm_pos;
    private GameObject swing_pos;
    

    // Use this for initialization
    void Start()
    {
        left_arm_pos = GameObject.Find("Left Arm Position");
        right_arm_pos = GameObject.Find("Right Arm Position");
        top_arm_pos = GameObject.Find("Top Right Arm Position");
        swing_pos = GameObject.Find("WeaponSpawn");

    }

    void DoneAttacking()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        done_attacking = false;
    }
    // Update is called once per frame
    void Update()
    {
        left_spawn_pos = left_arm_pos.transform.position;
        right_spawn_pos = right_arm_pos.transform.position;
        top_spawn_pos = top_arm_pos.transform.position;

        if (check_attack_left)
        {
            GameObject sword = Instantiate(weapon_left_prefab, left_spawn_pos, Quaternion.identity);
            sword.transform.parent = gameObject.transform;
            check_attack_left = false;
            attacking_left = true;
        }

        if(attacking_left)
        {
            transform.Rotate(Vector3.up, 200 * Time.deltaTime);
            current_timer += Time.deltaTime;
            if (current_timer > duration)
            {
                current_timer = 0;
                attacking_left = false;
                done_attacking = true;
            }
        }

        if (check_attack_right)
        {
            GameObject sword = Instantiate(weapon_right_prefab, right_spawn_pos, Quaternion.identity);
            sword.transform.parent = gameObject.transform;
            attacking_right = true;
            check_attack_right = false;
        }

        if (attacking_right)
        {
            transform.Rotate(Vector3.down, 200 * Time.deltaTime);
            current_timer += Time.deltaTime;
            if (current_timer > duration)
            {
                current_timer = 0;
                attacking_right = false;
                done_attacking = true;
            }
        }

        if (check_attack_top)
        {
            GameObject sword = Instantiate(weapon_right_prefab, top_spawn_pos, Quaternion.identity);
            sword.transform.parent = gameObject.transform;
            attacking_top = true;
            check_attack_top = false;
        }

        if (attacking_top)
        {
            transform.Rotate(Vector3.down, 200 * Time.deltaTime);
            //transform.Rotate(Vector3.forward, 200 * Time.deltaTime);
            current_timer += Time.deltaTime;
            if (current_timer >= duration)
            {
                current_timer = 0;
                attacking_top = false;
                done_attacking = true;
                
            }
        }

        if(done_attacking)
        {
            Invoke("DoneAttacking", .1f);
        }


        
    }
}
