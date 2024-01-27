using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2Logic : MonoBehaviour
{
    float horizontalMove = 0f;
    public float runSpeed = 40f;
    bool jump = false;

    public CharacterController2D_2 controller;
    bool crouch;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal2") * runSpeed;
        if (Input.GetButtonDown("Jump2"))
        {
            Debug.Log("Jump2");
            jump = true;
        }

        if (Input.GetButtonDown("Crouch2"))
        {
            crouch = true;
        }
        else if (Input.GetButtonUp("Crouch2"))
        {
            crouch = false;
        }

    }

    void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        jump = false;
    }
}
