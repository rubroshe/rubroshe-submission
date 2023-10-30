using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Properties")]
    [SerializeField] float moveSpeed = 0.5f;
    [SerializeField] float enemyHealth = 10f;
  //  [SerializeField] float collideInt = 1f;
    [SerializeField] public float enemyAttackDamage = 1f;  // Amount of damage enemy does to player
    [SerializeField] private float collisionAttackCooldownTimer = 0.85f; // Collision 
    [SerializeField] private const float collisionMaxCooldown = 0.85f;   // Collision

    float xPosition;
    float yPosition;
    [SerializeField] private float enemyAttackInterval = 4f; // attack interval default for Enemy (non-weapon specified)
    private float enemyNextAttackTime = 0f; // When the next attack can happen (similar to dash)

    [Header("References")]
    [SerializeField] GameObject punchSwoosh; // Swoosh texture for enemy attacks
    private Transform playerTransform; // Reference to player's transform
    private bool isAttackingCollision = false; // To ensure enemy doesn't attack too frequently
    private bool isInsideTrigger = false; // check if the player is inside the enemy's trigger
                                          //  [SerializeField] private float attackTimer; // Timer to keep track of when to attack next
                                          //  private bool initialAttack = false;

    private Vector2 previousPosition;  // Log the last direction enemy faces
    private Vector2 movementDirection;
    private GameObject player;
    private Vector3 lastSlashDirection = Vector3.zero; // Initialize with some default value.
    private float maxAttackDistance = 2.0f; // If enemy is out of range, dont attack


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        xPosition = transform.position.x; // Grab the initial x pos
        yPosition = transform.position.y; // Grab the initial y pos 
        playerTransform = player.transform; // Assumes your player object has the tag "Player"
        
        
    }

    void Update()
    {
        Vector2 currentPosition = transform.position;
        movementDirection = (currentPosition - previousPosition).normalized;
        // Debug.Log("Movement Direction: " + movementDirection);
        previousPosition = currentPosition;
        
        PursuePlayer();
        

        // Attack timer logic to check for collision 
        if (isInsideTrigger && !isAttackingCollision)
        {
            collisionAttackCooldownTimer -= Time.deltaTime;
            
            if(collisionAttackCooldownTimer <= 0)
            {
                StartCoroutine(AttackPlayerCollision(player));
                collisionAttackCooldownTimer = collisionMaxCooldown;  // Reset the timer after attacking
            }
        }

         // Automatically attack every 'attackInterval' seconds.
        if (Time.time >= enemyNextAttackTime)
        {
            if (Vector3.Distance(transform.position, playerTransform.position) <= maxAttackDistance)
            {
                Attack();
                enemyNextAttackTime = Time.time + enemyAttackInterval;
            }
        }


        // This is redundant and can be removed.
        // previousPosition = transform.position;
        
      //  Debug.Log("Current Position: " + currentPosition + ", Previous Position: " + previousPosition);

    }

    void PursuePlayer() // Make sure the enemy closes in on the Player
    {
        if (playerTransform != null)
        {
            // Move enemy towards the player
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collided object is the player
        if (other.CompareTag("Player") && this.GetComponent<Collider2D>() == GetComponent<Collider2D>())
        {
            Debug.Log("Collision Detected");
            isInsideTrigger = true;

           /* if (!initialAttack)
            {
                Debug.Log("Started Initial Attack");
                StartCoroutine(AttackPlayer(other.gameObject));
                initialAttack = true;
            }*/
            if (collisionAttackCooldownTimer == collisionMaxCooldown)
            {
                StartCoroutine(AttackPlayerCollision(player));
                collisionAttackCooldownTimer -= Time.deltaTime;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && this.GetComponent<Collider2D>() == GetComponent<Collider2D>())
        {
            Debug.Log("Collision has ended");
            isInsideTrigger = false;
           // initialAttack = false;
        }
    }

    /* void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("Trigger Stay Event Detected");
        // Check if the collided object is the player
        if (other.CompareTag("Player"))
        {
            StartCoroutine(AttackPlayer(other.gameObject));
        }
    } */

    IEnumerator AttackPlayerCollision(GameObject player) // Deals with collision damage
    {
        if(isAttackingCollision)
        {
            Debug.Log("Is already attacking");
            yield break; // Exit the coroutine if the enemy is already attacking
        }


        isAttackingCollision = true;
        // Deal damage to player here 
        Debug.Log("Attack Player!");
        player.GetComponent<Player>().TakeDamage(enemyAttackDamage);

        // Show attack animation/effect if any - maybe pixel splatter effect
        // Example: Instantiate(punchPixels, player.transform.position, Quaternion.identity);

        // yield return new WaitForSeconds(collideInt);
        isAttackingCollision = false;
    }

    public void TakeDamage(float damageAmount)
    {
        enemyHealth -= damageAmount;
        if (enemyHealth <= 0)
        {
            // Enemy dies, handle death logic here (like playing a death animation or destroying the enemy)
            Die();
        }
    }

    void Die()
    {
        // Destroy the enemy (play death animation later)
        Destroy(gameObject);
    }

    // ENEMY CAN NOW ATTACK PLAYER
    void Attack()
    {
        // Directional logic
        Vector3 enemySlashDirection = DetermineEnemySlashDirection();

        // Create offset to make it look polished
        Vector3 offset = Vector3.zero;

        if (Vector2.Distance(movementDirection, Vector2.right) < 0.255f)
        {
            Debug.Log("Right Offset");
            offset = new Vector3(0.6f, 0, 0);
        }
        else if (Vector2.Distance(movementDirection, Vector2.up) < 0.255f)
        {
            Debug.Log("Up Offset");
            offset = new Vector3(0, 0.6f, 0);
        }
        else if (Vector2.Distance(movementDirection, Vector2.left) < 0.255f)
        {
            Debug.Log("Left Offset");
            offset = new Vector3(-0.6f, 0, 0);
        }
        else if (Vector2.Distance(movementDirection, Vector2.down) < 0.255f)
        {
            Debug.Log("Down Offset");
            offset = new Vector3(0, -0.6f, 0);
        }
        else if (Vector2.Distance(movementDirection, (new Vector2(1, 1).normalized)) < 0.255f) // up + right
        {
            Debug.Log("Top-Right Offset");
            offset = new Vector3(0.3f, 0.3f, 0);
        }
        else if (Vector2.Distance(movementDirection, (new Vector2(-1, 1).normalized)) < 0.255f) // up + left
        {
            Debug.Log("Top-Left Offset");
            offset = new Vector3(-0.3f, 0.3f, 0);
        }
        else if (Vector2.Distance(movementDirection, new Vector2(-1, -1).normalized) < 0.255f) // down + left
        {
            Debug.Log("Down-Left Offset");
            offset = new Vector3(-0.3f, -0.3f, 0);
        }
        else if (Vector2.Distance(movementDirection, new Vector2(1, -1).normalized) < 0.255f) // down + right
        {
            Debug.Log("Down-Right Offset");
            offset = new Vector3(0.3f, -0.3f, 0);
        }


        // Instantiate the slash effect
        GameObject enemySwoosh = Instantiate(punchSwoosh, transform.position + offset, Quaternion.Euler(enemySlashDirection));
        enemySwoosh.transform.SetParent(transform);


        // Destroy the swoosh after a short duration (e.g., 0.5 seconds)
        Destroy(enemySwoosh, 0.5f);
    }

    Vector3 DetermineEnemySlashDirection()
    {
        // Convert the last input direction into Euler angles for rotation.
        if (Vector2.Distance(movementDirection, Vector2.right) < 0.255f) {
            Debug.Log("Right ROtation");
            lastSlashDirection = new Vector3(0, 0, 0);
            
        }
        if (Vector2.Distance(movementDirection, new Vector2(1, 1).normalized) < 0.255)
        {
            Debug.Log("Up-Right Rotation");
            return new Vector3(0, 0, 45); // up = right
        }
        if (Vector2.Distance(movementDirection, Vector2.up) < 0.255f) {
            Debug.Log("Up Rotation");
            lastSlashDirection = new Vector3(0, 0, 90);
            
        }
        if (Vector2.Distance(movementDirection, new Vector2(-1, 1).normalized) < .255f)
        {
            Debug.Log("Up-Left Rotation");
            return new Vector3(0, 0, 135); // up + left
        }
        if (Vector2.Distance(movementDirection, Vector2.left) < 0.255f)
        {
            Debug.Log("Left Rotation");
            lastSlashDirection = new Vector3(0, 0, 180);
        }
        if (Vector2.Distance(movementDirection, new Vector2(-1, -1).normalized) < 0.255f)
        {
            Debug.Log("Down-Left Rotation");
            return new Vector3(0, 0, 225); // down + left
        }
        if (Vector2.Distance(movementDirection, Vector2.down) < 0.255f)
        {
            Debug.Log("Down Rotation");
            lastSlashDirection = new Vector3(0, 0, 270);
        }
        if (Vector2.Distance(movementDirection, new Vector2(1, -1).normalized) < 0.255f)
        {
            Debug.Log("Down-Right Rotation");
            return new Vector3(0, 0, 315); // down + right
        }

        // Default to last known direction if no direction is found (or can use a previous direction).
        return lastSlashDirection;
    }
}
