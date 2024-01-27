using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1Logic : MonoBehaviour
{
    float horizontalMove = 0f;
    public float runSpeed = 40f;

    public CharacterController2D controller;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        
    }

    void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, false);
    }
}
