using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Bandit2 : MonoBehaviour {

    [SerializeField] float      m_speed = 4.0f;
    [SerializeField] float      m_jumpForce = 4.5f;

    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private Sensor_Bandit       m_groundSensor;
    private bool                m_grounded = false;
    private bool                m_combatIdle = false;
    private bool                m_isDead = false;

    private ParticleSystem smokeParticleSystem; // Reference to your Particle System component

    public float health;
    private float maxHealth;
    public GameObject bloodEffect;

    private float timeBtwAttack;
    public float startTimeBtwAttack;
    public Transform attackPos;
    public Transform attackPosBottom;
    public LayerMask whatIsEnemies;

    public float attackRange;
    public int damage;

    public GameOverManager gameOverManager;
    public Image healthBar;
    public Image nitroBar;

    public float nitroDuration = 5f; // Duration of nitro boost
    public float nitroSpeedBoost = 2f; // Multiplier for speed boost
    public float nitroRechargeRate = 0.5f; // Rate at which nitro refills

    private float currentNitroLevel2;
    private bool isNitroActive;
    // Use this for initialization
    void Start () {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Bandit>();
        // Get the Particle System component (assuming it's attached to the same GameObject)
        smokeParticleSystem = GetComponentInChildren<ParticleSystem>();
        // Disable emission initially
        var emissionModule = smokeParticleSystem.emission;
        emissionModule.enabled = false;
        maxHealth = health;
        currentNitroLevel2 = 1f; // Full nitro bar initially
        isNitroActive = false;
    }
	
	// Update is called once per frame
	void Update () {
        // Detect nitro activation (e.g., Left Shift)
        if (Input.GetButtonDown("Nitro2") && currentNitroLevel2 > 0.7)
        {
            ActivateNitro();
        }

        if (timeBtwAttack <= 1)
        {
            // attack possible
            if (Input.GetButtonDown("Hit2"))
            {
                m_animator.SetTrigger("Attack");
                Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange,whatIsEnemies);
                for (int i = 0; i < enemiesToDamage.Length; i++)
                {
                    Debug.Log(enemiesToDamage[i].ToString());
                    enemiesToDamage[i].GetComponent<Bandit>().TakeDamage(damage);
                }
            }
            if (timeBtwAttack <= 0)
            {
                timeBtwAttack = startTimeBtwAttack;
            }
        }
        else
        {
            timeBtwAttack -= Time.deltaTime;
        }

        if (health <= 0)
        {
            m_animator.SetTrigger("Death");
            gameOverManager.ShowGameOverScreen();
            //Destroy(gameObject);
        }
        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State()) {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
            StopSmokeParticleSystem();
            if(isNitroActive)
            {
                Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPosBottom.position, attackRange, whatIsEnemies);
                
                for (int i = 0; i < enemiesToDamage.Length; i++)
                {
                    Debug.Log(enemiesToDamage[i].GetComponent<Bandit>().ToString());
                    Debug.Log("onhead");
                    enemiesToDamage[i].GetComponent<Bandit>().TakeDamage(2 * damage);
                    
                }
            }
            
        }

        //Check if character just started falling
        if(m_grounded && !m_groundSensor.State()) {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // -- Handle input and movement --
        float inputX = Input.GetAxis("Horizontal2");

        // Swap direction of sprite depending on walk direction
        if (inputX > 0)
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        else if (inputX < 0)
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        // Move
        m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeed", m_body2d.velocity.y);

        // -- Handle Animations --
        //Death
        if (Input.GetKeyDown("e")) {
            if(!m_isDead)
                m_animator.SetTrigger("Death");
            else
                m_animator.SetTrigger("Recover");

            m_isDead = !m_isDead;
        }
            
        //Hurt
        else if (Input.GetKeyDown("q"))
            m_animator.SetTrigger("Hurt");

        //Attack
        //else if(Input.GetButtonDown("Hit2")){
        //    m_animator.SetTrigger("Attack");
        //}

        //Change between idle and combat idle
        else if (Input.GetKeyDown("f"))
            m_combatIdle = !m_combatIdle;

        //Jump
        else if (Input.GetButtonDown("Jump2") && m_grounded) {
            if(isNitroActive)
            {
                StartCoroutine(StartSmokeParticleSystem());
            }
            else
            {
                m_animator.SetTrigger("Jump");
                m_grounded = false;
                m_animator.SetBool("Grounded", m_grounded);
                m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
                m_groundSensor.Disable(0.2f);
            }

            
        }

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
            m_animator.SetInteger("AnimState", 2);

        //Combat Idle
        else if (m_combatIdle)
            m_animator.SetInteger("AnimState", 1);

        //Idle
        else
            m_animator.SetInteger("AnimState", 0);

        // Recharge nitro over time
        if (!isNitroActive)
        {
            currentNitroLevel2 += nitroRechargeRate * Time.deltaTime/60f;
            currentNitroLevel2 = Mathf.Clamp01(currentNitroLevel2); // Keep it between 0 and 1
            nitroBar.fillAmount = currentNitroLevel2;

        }
    }

    public void StopSmokeParticleSystem()
    {
        smokeParticleSystem.Simulate(0f, false, true);
        smokeParticleSystem.Stop();
    }

    public IEnumerator StartSmokeParticleSystem()
    {
        // Enable emission
        var emissionModule = smokeParticleSystem.emission;
        emissionModule.enabled = true;
        smokeParticleSystem.Play();
        yield return new WaitForSecondsRealtime(2); // Wait for 4 seconds
        m_animator.SetTrigger("Jump");
        m_grounded = false;
        m_animator.SetBool("Grounded", m_grounded);
        m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
        m_groundSensor.Disable(0.2f);
    }

    public void TakeDamage(int damage)
    {
        Instantiate(bloodEffect, transform.position, Quaternion.identity);
        health -= damage;
        m_animator.SetTrigger("Hurt");
        healthBar.fillAmount = health / maxHealth;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
        Gizmos.DrawWireSphere(attackPosBottom.position, attackRange);
    }

    private void ActivateNitro()
    {
        // Apply speed boost
        // Example: Increase motor torque for a car controller
        // Example: Add force to a rigidbody
        m_jumpForce = m_jumpForce * nitroSpeedBoost;

        // Play visual effects and sound

        // Start nitro countdown
        isNitroActive = true;
        currentNitroLevel2 = Mathf.Clamp01(0f); // Keep it between 0 and 1
        nitroBar.fillAmount = currentNitroLevel2;
        Invoke(nameof(DeactivateNitro), nitroDuration);
    }

    private void DeactivateNitro()
    {
        // Reset speed boost
        // Stop visual effects and sound

        m_jumpForce = 3.5f;
        // Nitro is no longer active
        isNitroActive = false;
    }
}
