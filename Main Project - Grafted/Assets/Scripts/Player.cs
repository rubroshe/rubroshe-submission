using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    enum Ability {Health, Speed, AttackSpeed, Dash}
    float xPosition;
    float yPosition;

    [SerializeField] float currentHealth, maxHealth;
    [SerializeField] int currentExperience,
        maxExperience,
        currentLevel;
    [SerializeField] public float attackDamage = 1f;
    [SerializeField] GameObject sword; // Default weapon png
    [SerializeField] GameObject dagger; // Dagger png
    [SerializeField] private GameObject swordSwoosh; // Swoosh texture prefab for sword (white)
    [SerializeField] private float attackInterval = 3f; // attack interval default for Player (non-weapon specified)
    private float nextAttackTime = 0f; // When the next attack can happen (similar to dash)
    public static float projectileSpeed = 5.5f;
    public static int projectileAmount = 1;
    public static float projectileDamage = 1.5f;

    private bool level5BuffApplied = false;
    private bool level8BuffApplied = false;
    private bool level10BuffApplied = false;


    [SerializeField] float moveSpeed = 3f;
    
    public float dashSpeed = 8f; // speed of the dash/how far player goes - 8 to 10 is good for upgrade later
    public float dashDuration = 0.12f; // how long dash effect lasts
    private Vector2 lastDirection = Vector2.right; // Default to right if Player hasn't moved yet
    private bool isDashing = false;
    [SerializeField] public float dashCooldown = 3f; // Time in secs before player can dash again
    [SerializeField] private float nextDashTime = 0f; // Time when the player can next dash

    Vector2 movement;
    public Rigidbody2D rb;
    [SerializeField] Image healthBar;
    [SerializeField] Image experienceBar;

    // Start is called before the first frame update
    void Start()
    {
        Ability myAbility = Ability.Health;
        xPosition = transform.position.x; // Grab the initial x pos
        yPosition = transform.position.y; // Grab the initial y pos 

    }


    public Animator animator; 


    // Update is called once per frame
    void Update()
    {
        //     animator.SetFloat("Horizontal", Input.GetAxis("Horizontal"));
        //    animator.SetFloat("Vertical", Input.GetAxis("Vertical"));

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (!PauseMenu.isPaused)
        {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetFloat("Speed", movement.sqrMagnitude);
        }

        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");

       // Vector2 direction = new Vector2(xInput, yInput).normalized;

       // transform.position += new Vector3(direction.x, direction.y, 0) * moveSpeed * Time.deltaTime;

        // Track last direction if there's been movement
        if (xInput != 0 || yInput != 0)
        {
            lastDirection = new Vector2(xInput, yInput).normalized;
        }

        // Detect spacebar for dash and check if the cooldown has elapsed
        if (Input.GetKeyDown(KeyCode.Space) && !isDashing && Time.time >= nextDashTime)
        {
            StartCoroutine(Dash());
            nextDashTime = Time.time + dashCooldown; // Set the next time the player can dash
        }

        // Automatically attack every 'attackInterval' seconds.
        if (Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackInterval;
        }

    }

    private void FixedUpdate()
    {
        if (!PauseMenu.isPaused)
        {
            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;

        float startTime = Time.time;

        while (Time.time < startTime + dashDuration)
        {
            transform.position += (Vector3)(lastDirection * dashSpeed * Time.deltaTime);
            yield return null;
        }

        isDashing = false;
    }

    void Attack()
    {
        // Directional logic
        Vector3 slashDirection = DetermineSlashDirection();

        // Create offset to make it look polished
        Vector3 offset = Vector3.zero;

        if (lastDirection == Vector2.right)
        {
            offset = new Vector3(0.5f, 0.35f, 0);
        }
        else if (lastDirection == Vector2.up)
        {
            offset = new Vector3(0, 0.8f, 0);
        }
        else if (lastDirection == Vector2.left)
        {
            offset = new Vector3(-0.5f, 0.35f, 0);
        }
        else if (lastDirection == Vector2.down)
        {
            offset = new Vector3(0, -0.2f, 0);
        }
        else if (lastDirection == new Vector2(1, 1).normalized) // up + right
        {
            offset = new Vector3(0.3f, 0.6f, 0);
        }
        else if (lastDirection == new Vector2(-1, 1).normalized) // up + left
        {
            offset = new Vector3(-0.3f, 0.6f, 0);
        }
        else if (lastDirection == new Vector2(-1, -1).normalized) // down + left
        {
            offset = new Vector3(-0.3f, 0, 0);
        }
        else if (lastDirection == new Vector2(1, -1).normalized) // down + right
        {
            offset = new Vector3(0.3f, 0, 0);
        }
        

        // Instantiate the slash effect
        GameObject swoosh = Instantiate(swordSwoosh, transform.position + offset, Quaternion.Euler(slashDirection));
        swoosh.transform.SetParent(transform);

        switch (currentLevel)
        {
            case int level when level >= 10:
                if (!level10BuffApplied)
                {
                    moveSpeed++;
                    level10BuffApplied = true;
                }
                StartCoroutine(SpawnDaggers(new Vector3(0, 0.35f, 0), slashDirection - new Vector3(0, 0, 90), 4, 0.2f));
                break;

            case int level when level >= 8:
                if (!level8BuffApplied)
                {
                    attackDamage += 1.5f;
                    level8BuffApplied = true;
                }
                // If level is 8 or greater, spawn 3 daggers
                StartCoroutine(SpawnDaggers(new Vector3(0, 0.35f, 0), slashDirection - new Vector3(0, 0, 90), 3, 0.25f));
                break; // Prevents the code for lower levels from running

            case int level when level >= 5:
                if (!level5BuffApplied)
                {
                    attackInterval -= attackInterval * 0.10f;
                    level5BuffApplied = true;
                }
                // If level is 5 or greater but less than 8, spawn 2 daggers
                StartCoroutine(SpawnDaggers(new Vector3(0, 0.35f, 0), slashDirection - new Vector3(0, 0, 90), 2, 0.25f));
                break;

            case int level when level >= 3:
                // If level is 3 or greater but less than 5, spawn 1 dagger
                StartCoroutine(SpawnDaggers(new Vector3(0, 0.35f, 0), slashDirection - new Vector3(0, 0, 90), 1, 0.25f));
                break;


                default: break;
        }


        // Destroy the swoosh after a short duration (e.g., 0.5 seconds)
      //  Destroy(swoosh, 0.25f); Now handled in animator
    }

    IEnumerator SpawnDaggers(Vector3 spawnOffset, Vector3 rotationOffset, int numberOfDaggers, float delay)
    {
        for (int i = 0; i < numberOfDaggers; i++)
        {
            // Instantiate the dagger at the specified offset and rotation
            Instantiate(dagger, transform.position + spawnOffset, Quaternion.Euler(rotationOffset));

            // Wait for the specified delay before spawning the next dagger
            yield return new WaitForSeconds(delay);
        }
    }

    Vector3 DetermineSlashDirection()
    {
        // lastDirection SHOULD track what last input was from dash mechanic
        Vector2 lastInputDirection = lastDirection;

        // Convert the last input direction into Euler angles for rotation.
        if (lastInputDirection == Vector2.right) return new Vector3(0, 0, 0);
        if (lastInputDirection == new Vector2(1, 1).normalized) return new Vector3(0, 0, 45); // up = right
        if (lastInputDirection == Vector2.up) return new Vector3(0, 0, 90);
        if (lastInputDirection == new Vector2(-1, 1).normalized) return new Vector3(0, 0, 135); // up + left
        if (lastInputDirection == Vector2.left) return new Vector3(0, 0, 180);
        if (lastInputDirection == new Vector2(-1, -1).normalized) return new Vector3(0, 0, 225); // down + left
        if (lastInputDirection == Vector2.down) return new Vector3(0, 0, 270);
        if (lastInputDirection == new Vector2(1, -1).normalized) return new Vector3(0, 0, 315); // down + right
        
        // Default to right if no direction is found (or can use a previous direction).
        return Vector3.zero;
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        healthBar.fillAmount = currentHealth / 100f;
        // healthBar.fillAmount = currentHealth / 100f;
        if (currentHealth <= 0)
        {
            // Player dies, handle death logic here (like playing a death animation or restarting the level)
            // For now, just disable the Player object:
            Destroy(gameObject);
            GameManager.instance.InitiateGameOver();
            // add something for Game Over UI
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Hit a Wall");
    }

    private void OnEnable()
    {
        if (ExperienceManager.Instance != null)
        {
            // Subscribe Event
            ExperienceManager.Instance.OnExperienceChange += HandleExperienceChange;
        }
        else
        {
            Debug.LogWarning("ExperienceManager.Instance is null in OnEnable.");
        }
    }

    private void OnDisable()
    {
        if (ExperienceManager.Instance != null)
        {
            // Unsubscribing from Event 
            ExperienceManager.Instance.OnExperienceChange -= HandleExperienceChange;
        }
        else
        {
            Debug.LogWarning("ExperienceManager.Instance is null in OnDisable.");
        }
    }

    private void HandleExperienceChange(int newExperience)
    {
        currentExperience += newExperience;
        experienceBar.fillAmount = (float)currentExperience / maxExperience;
        Debug.Log("exp bar amt: " + experienceBar.fillAmount);
        if (currentExperience >= maxExperience)
        {
            LevelUp(); // Show UI element that says leveled up
        }
    }

    private void LevelUp()
    {
        experienceBar.fillAmount = 0;
        maxHealth += 25;
        currentHealth = maxHealth;  // Make random stats pool to choose from and text element that says "+_% [STAT]"
                                    // Dagger
         // Dagger.instance.DaggerDelay -= 1;

        // if statements of Random.Range AMT% buff for each buff
        // Array at Start of Ability which stat got chosen. of how many abilities/buffs there are. 
        // List integers that matches Abilities in Start Array
        // another Array [Cap 3] - 3 getting pulled from List - List pulls from ALL available Ability - pulled Ability gets deleted from pool 
        // Reset List to equal entire Array to restart process.
        // Method for all 3 buttons. public void that takes integer. 
        // In Player class, if 0, means left button, 1 middle, 2 right. What's the '1' in the list? could be '6' which could mean SPD buff
        // Choose Ability from array , save #s in array 
        // Button looks inside array for what to display
        // All 3 buttons use same method


        currentLevel++;
        GameManager.instance.IncreaseLevel(currentLevel);
        currentExperience = 0;

        if (currentLevel % 2 == 0)
        {
            maxExperience += 100;
        }
        else
        {
            maxExperience += 200;
        }

    }
}
