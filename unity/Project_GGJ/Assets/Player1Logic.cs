using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1Logic : MonoBehaviour
{
    float horizontalMove = 0f;
    public float runSpeed = 40f;
    bool jump = false;
    bool crouch = false;

    public CharacterController2D controller;
    private ParticleSystem smokeParticleSystem; // Reference to your Particle System component
    
    // Start is called before the first frame update
    void Start()
    {
        // Get the Particle System component (assuming it's attached to the same GameObject)
        smokeParticleSystem = GetComponent<ParticleSystem>();
        // Disable emission initially
        var emissionModule = smokeParticleSystem.emission;
        emissionModule.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            StartSmokeParticleSystem();
        }

        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;
        }else if (Input.GetButtonUp("Crouch"))
        {
            crouch = false;
        }

    }

    void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        jump = false;
    }

    public void StopSmokeParticleSystem()
    {
        smokeParticleSystem.Simulate(0f, false, true);
        smokeParticleSystem.Stop();
    }

    public void StartSmokeParticleSystem()
    {
        // Enable emission
        Debug.Log("StartSmokeParticleSystem");
        var emissionModule = smokeParticleSystem.emission;
        emissionModule.enabled = true;
        smokeParticleSystem.Play();
    }
}
