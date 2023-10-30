using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
   // [SerializeField] public Player player;
    bool canDealDamage = true;
    public float playerWeaponAttackDamage = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canDealDamage) return; 

        if (other.CompareTag("Enemy"))  // assuming your enemies have the tag "Enemy"
        {
            Enemy enemy = other.GetComponent<Enemy>(); // assuming your enemy has a script called "Enemy"
            if (enemy != null)
            {
                // Access the player's attack damage using GetComponent.
                Player player = GetComponentInParent<Player>();
                if (player != null)
                {

                    enemy.TakeDamage(player.attackDamage); // you'd fetch this damage value from the player's sword script
                    canDealDamage = false; // prevent further damage
                }
            }
        }

    } 
}
