using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float speed = 10f;

    [SerializeField] GameManager manager;
    [SerializeField] GameObject laser;

    public int health = 1;
    float fireRate = 4f;

    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.tag == "EliteEnemy")
        {
            InvokeRepeating("SpawnEliteLaser", 0, fireRate);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position -= new Vector3(0, speed, 0) * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if the enemy collides with another enemy, do nothing
        if (collision.gameObject.CompareTag("EliteEnemy") || collision.gameObject.CompareTag("Enemy"))
        {
            return; 
        }
        
        
        if (collision.gameObject.tag == "Player") 
        {
            GameManager.instance.InitiateGameOver();
            Destroy(gameObject); // Destroy the enemy when it hits the player
            Destroy(collision.gameObject);
            
        }
        else
        {
            // Decrement the health of the enemy when hit
            health--;

            // Check if the enemy's health has reached zero
            if (health <= 0)
            {
                GameManager.instance.IncreaseScore(10);
                Destroy(gameObject); // Destroy the enemy if health is zero
            }

            Destroy(collision.gameObject); // Destroy the bullet/projectile
        }
    }

    public void SpawnEliteLaser()
    {
        Instantiate(laser, transform.position, Quaternion.identity);
    }
}
