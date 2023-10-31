using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour // attached to Enemy's child weapon
{
    bool enemyCanDealDamage = true;
    public float enemyWeaponAttackDamage; // 5 default for basic enemies
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D character)
    {
        if (!enemyCanDealDamage) return;

        if (character.CompareTag("Player"))  // assuming your Player has the tag "Player"
        {
            Player player = character.GetComponent<Player>(); // assuming your Player has a script called "Player"
            if (player != null)
            { 
                    Debug.Log("EnemyWeapon Strike!");
                    player.TakeDamage(enemyWeaponAttackDamage); // get damage from enemy scrip[t
                    enemyCanDealDamage = false; // prevent further damage
                
            }
        }

    }
}
