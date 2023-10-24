using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Properties")]
    [SerializeField] float moveSpeed = 0.5f;
    [SerializeField] float enemyHealth = 10f;
    [SerializeField] float attackInt = 2f;
  //  [SerializeField] float collideInt = 1f;
    [SerializeField] float attackDamage = 1f;  // Amount of damage enemy does to player
    [SerializeField] private float attackCooldownTimer = 0.85f;
    [SerializeField] private const float maxCooldown = 0.85f;

    [Header("References")]
    [SerializeField] GameObject punchSwoosh; // Swoosh texture for enemy attacks
    private Transform playerTransform; // Reference to player's transform
    private bool isAttacking = false; // To ensure enemy doesn't attack too frequently
    private bool isInsideTrigger = false; // check if the player is inside the enemy's trigger
  //  [SerializeField] private float attackTimer; // Timer to keep track of when to attack next
    private bool initialAttack = false;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform; // Assumes your player object has the tag "Player"
    }

    void Update()
    {
        PursuePlayer();

        // Attack timer logic
        if (isInsideTrigger && !isAttacking)
        {
            attackCooldownTimer -= Time.deltaTime;
            
            if(attackCooldownTimer <= 0)
            {
                StartCoroutine(AttackPlayer(GameObject.FindGameObjectWithTag("Player")));
                attackCooldownTimer = maxCooldown;  // Reset the timer after attacking
            }
        }
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
        if (other.CompareTag("Player"))
        {
            Debug.Log("Collision Detected");
            isInsideTrigger = true;

           /* if (!initialAttack)
            {
                Debug.Log("Started Initial Attack");
                StartCoroutine(AttackPlayer(other.gameObject));
                initialAttack = true;
            }*/
            if (attackCooldownTimer == maxCooldown)
            {
                StartCoroutine(AttackPlayer(GameObject.FindGameObjectWithTag("Player")));
                attackCooldownTimer -= Time.deltaTime;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
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

    IEnumerator AttackPlayer(GameObject player) // Deals with collision damage
    {
        if(isAttacking)
        {
            Debug.Log("Is already attacking");
            yield break; // Exit the coroutine if the enemy is already attacking
        }


        isAttacking = true;
        // Deal damage to player here 
        Debug.Log("Attack Player!");
        player.GetComponent<Player>().TakeDamage(attackDamage);

        // Show attack animation/effect if any - maybe pixel splatter effect
        // Example: Instantiate(punchPixels, player.transform.position, Quaternion.identity);

        // yield return new WaitForSeconds(collideInt);
        isAttacking = false;
    }

    /*public void TakeDamage(float damageAmount)
    {
        enemyHealth -= damageAmount;
        if (enemyHealth <= 0)
        {
            // Enemy dies, handle death logic here (like playing a death animation or destroying the enemy)
            Destroy(gameObject);
        }
    }*/
}
