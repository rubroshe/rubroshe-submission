using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] float spawnRate = 2f;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject enemyPrefab2;

    float xMin;
    float xMax;
    float ySpawn;
    

    // Start is called before the first frame update
    void Start()
    {
        xMin = Camera.main.ViewportToWorldPoint(new Vector3(.1f, 0, 0)).x;
        xMax = Camera.main.ViewportToWorldPoint(new Vector3(.9f, 0, 0)).x;
        ySpawn = Camera.main.ViewportToWorldPoint(new Vector3(0, 1.25f, 0)).y;
        InvokeRepeating("SpawnEnemy", 0, spawnRate);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnEnemy()
    {
        float randX = Random.Range(xMin, xMax);
        Instantiate(enemyPrefab, new Vector3(randX, ySpawn, 0), Quaternion.identity);
    }

    public void spawnEliteEnemy()
    {
        
        float randX = Random.Range(xMin, xMax);
        GameObject eliteEnemy = Instantiate(enemyPrefab2, new Vector3(randX, ySpawn, 0), Quaternion.identity);

        // eliteEnemy.GetComponent<Enemy>().health = 2; // Set the health to 2
    }

    
}

