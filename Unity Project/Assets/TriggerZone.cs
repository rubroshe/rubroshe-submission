using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    [SerializeField] Player player;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.transform.localScale = new Vector3 (2, 2, 2);
        collision.gameObject.transform.GetComponent<SpriteRenderer>().color = Color.red;
        // collision.gameObject.GetComponent<Player>().UpSpeed(3);
        player.UpSpeed(20);
        player.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        collision.gameObject.transform.localScale = new Vector3(1, 1, 1);
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().UpSpeed(-3);
        }
        
    }
}
