using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dagger : MonoBehaviour
{
    [SerializeField] float knockBack = 1.5f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.up * Player.projectileSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
            if (collision.gameObject.CompareTag("Enemy"))  // assuming your enemies have the tag "Enemy"
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>(); // assuming your enemy has a script called "Enemy"
                if (enemy != null)
                {
                   
                    Vector2 direction = (collision.gameObject.transform.position - transform.position).normalized;
                    enemy.rb.AddForce(direction * knockBack, ForceMode2D.Impulse);
                    enemy.TakeDamage(Player.projectileDamage);
                    // canDealDamage = false; // prevent further damage
                }
            }
            Debug.Log("Damage dealt from dagger" + Player.projectileDamage.ToString());
            Destroy(gameObject);
        }
    }

