using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
   // [SerializeField] public Player player;
    bool canDealDamage = true;
    public float playerWeaponAttackDamage = 1f;
    private Player player;
    [SerializeField] float knockBack;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canDealDamage) return; 

        if (other.gameObject.CompareTag("Enemy"))  // assuming your enemies have the tag "Enemy"
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>(); // assuming your enemy has a script called "Enemy"
            if (enemy != null)
            {
                Debug.Log("Player Weapon strike!");
                Vector2 direction = (other.gameObject.transform.position - player.transform.position).normalized;
                enemy.rb.AddForce(direction * knockBack, ForceMode2D.Impulse);
                enemy.TakeDamage(playerWeaponAttackDamage); 
               // canDealDamage = false; // prevent further damage
            }
        }

    } 

    public void KillSelf()
    {
        Destroy(gameObject);
    }

   
}
