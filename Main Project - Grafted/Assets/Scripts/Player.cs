using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Player : MonoBehaviour
{
    float xPosition;
    float yPosition;
    [SerializeField] float health = 30f; // Serialize for now
    [SerializeField] public float attackDamage = 1f;
    [SerializeField] GameObject sword; // Default weapon png
    [SerializeField] private GameObject swordSwoosh; // Swoosh texture prefab for sword (white)
    [SerializeField] private float attackInterval = 3f; // attack interval default for Player (non-weapon specified)
    private float nextAttackTime = 0f; // When the next attack can happen (similar to dash)


    [SerializeField] float moveSpeed = 3f;
    // private Rigidbody2D rb; not needed, just had "Horizontal" twice

    public float dashSpeed = 8f; // speed of the dash/how far player goes - 8 to 10 is good for upgrade later
    public float dashDuration = 0.12f; // how long dash effect lasts
    private Vector2 lastDirection = Vector2.right; // Default to right if Player hasn't moved yet
    private bool isDashing = false;
    [SerializeField] public float dashCooldown = 3f; // Time in secs before player can dash again
    [SerializeField] private float nextDashTime = 0f; // Time when the player can next dash


    // Start is called before the first frame update
    void Start()
    {

        xPosition = transform.position.x; // Grab the initial x pos
        yPosition = transform.position.y; // Grab the initial y pos 

    }

    // Update is called once per frame
    void Update()
    {

        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");

        Vector2 direction = new Vector2(xInput, yInput).normalized;

        transform.position += new Vector3(direction.x, direction.y, 0) * moveSpeed * Time.deltaTime;

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


        // Destroy the swoosh after a short duration (e.g., 0.5 seconds)
        Destroy(swoosh, 0.5f);
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
        health -= damageAmount;
        if (health <= 0)
        {
            // Player dies, handle death logic here (like playing a death animation or restarting the level)
            // For now, just disable the Player object:
            gameObject.SetActive(false);
            // add something for Game Over UI
        }
    }
}
